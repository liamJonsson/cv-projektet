using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

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


        // här ska index- action ligga
        [HttpGet]
        public IActionResult Index()
        {
            var projects = _context.Projects
                .Include(p => p.Participants)
                .ThenInclude(pu => pu.User)
       
                .ToList();


            return View(projects);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var project = _context.Projects
                .Include(p => p.Participants)
                .ThenInclude(p => p.User)
                .Include(pu => pu.Creator)
                .FirstOrDefault(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize] //Man måste vara inloggad för att komma åt Skapa projekt
        [HttpPost] //Metoden körs när vi klickar Spara i gränssnittet 
        public async Task<IActionResult> Add(Project project, IFormFile fileToUpload) //Skickar in ett project och zip-filen från formuläret
        {

            if (!ModelState.IsValid) //Om något är fel/tomt etc så går vi in i if-satsen
            {
                return View(project); //Visa formuläret igen, inget sparas i databasen
            }

            var user = await _userManager.GetUserAsync(User); //Hämta inloggad användare

            project.CreatorId = user.Id; //Sätter CreatodId på projektet till Id:t på personen som är inloggad

            if(fileToUpload != null && fileToUpload.Length > 0) //Kontrolelrar att en fil har lagt till och att den inte är tom
            {
                var fileName = Path.GetFileName(fileToUpload.FileName); //Tar fram filnamnet
                var filePath = Path.Combine("wwwroot/uploads", fileName); //Skapar en sökväg till servern

                using (var stream = new FileStream(filePath, FileMode.Create)) //Skapar filen på servern
                {
                    await fileToUpload.CopyToAsync(stream); //Kopierar innehållet från uppladdade filen och sparar den på servern
                }
                project.ZipFile = "/uploads/" + fileName; //Sparar filens !sökväg! (inte filen) i projektet
            }
            _context.Projects.Add(project); //Lägger till projektet i Entity Framework
            await _context.SaveChangesAsync(); //Sparar till databasen, SQL insert 

            ViewBag.SuccessMessage = "Projektet har lagts till!";
            return View(new Project());
        }

        // --- EDIT ---
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var project = _context.Projects.Find(id);
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project project)
        {
            var existingProject = _context.Projects
                .AsNoTracking()
                .FirstOrDefault(p => p.ProjectId == project.ProjectId);

            if(existingProject == null)
            {
                return NotFound();
            }

            project.CreatorId = existingProject.CreatorId;

            _context.Projects.Update(project);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.Find(id);
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var project = _context.Projects.Find(id);
            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
