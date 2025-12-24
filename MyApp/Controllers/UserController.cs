using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore;        
using MyApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MyApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.Include(u => u.Address).ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var addressList = _context.Addresses
                .Select(a => new { Id = a.AddressId, DisplayText = $"{a.HomeAddress}, {a.City}" })
                .ToList();

            ViewBag.Addresses = new SelectList(addressList, "Id", "DisplayText");
            return View();
        }

        [HttpPost]
        public IActionResult Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            // Ladda adress-listan igen
            var addressList = _context.Addresses
                .Select(a => new { Id = a.AddressId, DisplayText = $"{a.HomeAddress}, {a.City}" })
                .ToList();

            ViewBag.Addresses = new SelectList(addressList, "Id", "DisplayText", user.AddressId);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // --- DELETE ---
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Hanterar inloggningen och skapar cookien
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),                    
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorMessage = "Fel användarnamn eller lösenord.";
            return View();
        }

        // Loggar ut användaren och tar bort cookien
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

