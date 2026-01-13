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
        public async Task<IActionResult> Index()
        {
            var users = _context.Users
                .Where(u => u.Deactivated == false)
                .Include(u => u.ParticipatingProjects)
                    .ThenInclude(pp => pp.Project)
                //Frågan körs inte här, den byggs så att den kan byggas vidare senare
                .AsQueryable();

            bool isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                //Här byggs frågan på med users vars visibility är true och endast dem
                users = users.Where(u => u.Visibility == true);
            }
            //Här returneras users som en lista asynkront
            return View(await users.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var users = _context.Users
                .Where(u => u.Deactivated == false)
                .Include(u => u.ParticipatingProjects)
                    .ThenInclude(pp => pp.Project)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                /*terms blir en lista av orden som sökningen består av, varje ord splittras
                vid mellanslag*/
                var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                //För varje ord i terms kollas en användares namn och skills
                foreach (var term in terms)
                {
                    users = users.Where(u =>
                        u.Name.Contains(term) ||
                        (u.Skills != null && u.Skills.Contains(term))
                    );
                }
            }

            bool isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            //Om man inte är inloggad så behöver ens visibility vara true för att komma med i sökningen
            if (!isLoggedIn)
            {
                users = users.Where(u => u.Visibility == true);
            }

            //Skickar med query till vyn för att kunna använda den där
            ViewData["SearchQuery"] = query;

            var userList = await users.ToListAsync();

            return View("Index", userList);
        }
    }
}