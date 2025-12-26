using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;
using MyApp.InputModels;

namespace MyApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ApplicationDbContext _context;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.Include(u => u.Address).ToList();
            return View(users);
        }

        // Registrera
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterInputModel model)
        {
            if (ModelState.IsValid)
            {
                var newAddress = new Address
                {
                    HomeAddress = model.HomeAddress,
                    ZipCode = model.ZipCode,
                    City = model.City
                };
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync(); // Adress får ett ID från databasen
                var user = new User
                {
                    // Identity
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,

                    // User fält
                    Name = model.Name,
                    AddressId = newAddress.AddressId,
                    ProfileImage = "default.jpg",
                    Visibility = false,
                    Deactivated = false
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // Inlogg
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Identitys "SignInManager" sköter allt: 
            // kollar användaren, hashar lösenordet, sätter cookien.
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Fel användarnamn eller lösenord.";
            return View();
        }

        // Logga ut
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}