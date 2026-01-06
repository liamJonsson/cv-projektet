using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(Project project)
        {
            
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            var user = await _userManager.GetUserAsync(User);
            project.CreatorId = user.Id;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            ModelState.Clear();
            ViewBag.SuccessMessage = "Projektet har lagts till!";
            return View(new Project());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null) return NotFound();

            ViewBag.Creators = new SelectList(_context.Users, "Id", "Name", project.CreatorId);
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            var dbProject = _context.Projects.FirstOrDefault(p => p.ProjectId == project.ProjectId);

            if (dbProject != null)
            {
                dbProject.Title = project.Title;
                dbProject.Description = project.Description;
                dbProject.CodeLanguage = project.CodeLanguage;
                dbProject.StartDate = project.StartDate; // Lägg till denna rad!
                dbProject.ZipFile = project.ZipFile;

                _context.SaveChanges();
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
        [Authorize]
        public async Task<IActionResult> JoinProject(int id)
        {
            // 1. Hämta ID som en sträng från Identity
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return Challenge();
            }

            // 2. Konvertera strängen till en int
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
            if (string.IsNullOrEmpty(userIdString)) return Challenge();

            int userIdInt = int.Parse(userIdString);

            // Hitta kopplingen i databasen
            var projectUser = await _context.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == id && pu.UserId == userIdInt);

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