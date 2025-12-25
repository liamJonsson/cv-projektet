using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
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
                .Include(p => p.Creator)
                .Include(p => p.Participants)
                .ThenInclude(pu => pu.User)
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
            ViewBag.Creators = new SelectList(_context.Users, "UserId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Add(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // --- EDIT ---
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var project = _context.Projects.Find(id);
            ViewBag.Creators = new SelectList(_context.Users, "UserId", "Name", project.CreatorId);
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project project)
        {
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
