using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;
using System.Security.Claims;

namespace MyApp.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProjectController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //Vi använder Entity Framework för att hämta projekt från databasen.
            //1. .Include(p => p.Creator) hämtar information om den som skapade projektet.
            // 2. .Include(p => p.Participants).ThenInclude(pu => pu.User) hämtar listan på deltagare
            var projects = _context.Projects
                .Where(p => p.CreatorId != null) // Ändrat till null-koll om det är en sträng
                .Include(p => p.Creator)
                .Include(p => p.Participants)
                    .ThenInclude(pu => pu.User)
                .ToList();

            return View(projects);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize] //Man måste vara inloggad för att komma åt Skapa projekt
        [HttpPost] //Metoden körs när vi klickar Spara i gränssnittet 
        public async Task<IActionResult> Add(Project project)
        {
            if (!ModelState.IsValid) //Om något är fel/tomt etc så går vi in i if-satsen
            {
                return View(project); //Visa formuläret igen, inget sparas i databasen
            }

            var user = await _userManager.GetUserAsync(User); //Hämta inloggad användare

            project.CreatorId = user.Id; //Sätter CreatodId på projektet till Id:t på personen som är inloggad

            project.Participants.Add(new ProjectUser //Lägger till skaparen i deltagar-listan
            {
                UserId = user.Id
            });

            _context.Projects.Add(project); //Lägger till projektet i Entity Framework
            await _context.SaveChangesAsync(); //Sparar till databasen, SQL insert 


            //Rensa formuläret och ladda om sidan
            TempData["SuccessMessage"] = "Projektet har skapats.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Find(id) hjälper att hitta primärnyckel  
            var project = _context.Projects.Find(id);
            //Felhantring - om id inte hittas eller om man skriver fel i Url så returnerar den NotFound  
            if (project == null) return NotFound();

            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project project)
        {
            if (!ModelState.IsValid) // kontrollerar om det finns valideringsfel 
            {
                return View(project);
            }

            var dbProject = _context.Projects.FirstOrDefault(p => p.ProjectId == project.ProjectId);// hämtar projekt id  från databasen för att  ändra på rätt rad

            if (dbProject != null) 
            {
                dbProject.Title = project.Title;
                dbProject.Description = project.Description;
                dbProject.CodeLanguage = project.CodeLanguage;
                dbProject.StartDate = project.StartDate; 

                _context.SaveChanges(); // sparar ändringarna i databasen
                TempData["SuccessMessage"] = "Projektet har uppdaterats!";
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null) return NotFound();
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var project = _context.Projects.Find(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize] // endast inloggade användare får gå med
        public async Task<IActionResult> JoinProject(int id)
        {
            // 1. Hämta ID som en sträng från Identity
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return Challenge();
            }

            // Konvertera strängen till en int
            // Vi använder int.Parse eftersom vi vet att ID:t ska finnas om man är inloggad
            int userIdInt = int.Parse(userIdString);

            // 3. Kontrollera om kopplingen redan finns (nu med int mot int)
            var exists = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == id && pu.UserId == userIdInt);

            if (!exists)
            {
                // 4. Skapa kopplingen
                var projectUser = new ProjectUser
                {
                    ProjectId = id,
                    UserId = userIdInt // Nu matchar typerna!
                };

                _context.ProjectUsers.Add(projectUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Du har gått med i projektet!";
            }

            return RedirectToAction(nameof(Index));
        }
    

    [HttpPost]
        [Authorize]
        public async Task<IActionResult> LeaveProject(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Challenge(); // Om ID inte hittas, tvingas användaren logga in igen

            int userIdInt = int.Parse(userIdString);

            // Hitta kopplingen i databasen
            var projectUser = await _context.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == id && pu.UserId == userIdInt); // hittar rad där projektid och användarid matchar

            if (projectUser != null)
            {
                _context.ProjectUsers.Remove(projectUser);
                await _context.SaveChangesAsync();

                // Bekräftelsemeddelande
                TempData["SuccessMessage"] = "Du har nu lämnat projektet.";
            }

            return RedirectToAction(nameof(Index));
        }

    }

}