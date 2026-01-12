using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
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
                .Where(u => u.Deactivated == false)
                .Include(u => u.ParticipatingProjects)
                    .ThenInclude(pp => pp.Project)
                .AsQueryable();

            bool isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                users = users.Where(u => u.Visibility == true);
            }

            return View(users.ToList());
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            var users = _context.Users
                .Where(u => u.Deactivated == false)
                .Include(u => u.ParticipatingProjects)
                    .ThenInclude(pp => pp.Project)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in terms)
                {
                    users = users.Where(u =>
                        u.Name.Contains(term) ||
                        (u.Skills != null && u.Skills.Contains(term))
                    );
                }
            }

            bool isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                users = users.Where(u => u.Visibility == true);
            }

            ViewData["SearchQuery"] = query;

            return View("Index", users.ToList());
        }

    }
}
