using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users
                //Gör att varje user får tillgång till ParticipatingProjects
                .Include(u => u.ParticipatingProjects)
                //och därefter Projects i den tabellen
                    .ThenInclude(pp => pp.Project)
                .ToList();

            return View(users);
        }
    }
}
