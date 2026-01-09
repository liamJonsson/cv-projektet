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

            if (!string.IsNullOrWhiteSpace(query))
            {
                //Här byggs frågan på med ett where villkor
                users = users.Where(u => u.Name.Contains(query));
            }
            //Och här körs faktiskt databasen, inte innan alls
            return View("Index", users.ToList());
        }

    }
}
