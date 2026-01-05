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

        [HttpGet]
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

        [HttpGet]
        public IActionResult Search(string query)
        {
            var users = _context.Users
                .Include(u => u.ParticipatingProjects)
                    .ThenInclude(pp => pp.Project)
                //Denna gör så att databasen avvaktas med att köra tills att vi är klara med frågan
                .AsQueryable();

            bool isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (!string.IsNullOrWhiteSpace(query))
            {
                //Splittar upp sökningarna där det är mellanrum, Remove... gör så att tomma strängar inte tas med
                var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var term in terms)
                {
                    users = users.Where(u =>
                        u.Name.Contains(term) ||
                        (u.Skills != null && u.Skills.Contains(term))
                    );
                }
            }

            if (!isLoggedIn)
            {
                users = users.Where(u => u.Visibility == true);
            }

            return View("Index", users.ToList());
        }

    }
}
