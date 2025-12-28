using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.InputModels;
using MyApp.Models;


namespace MyApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
                    Visibility = true,
                    Deactivated = false,
                    Cv = "", // Databasen kräver ett värde
                    EmailConfirmed = true
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

        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.ParticipatingProjects)
                    .ThenInclude(pu => pu.Project)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Visa andras profiler
        [AllowAnonymous]
        [HttpGet("User/Profile/{id}")]
        public async Task<IActionResult> Profile(int id)
        {
            var userProfile = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.ParticipatingProjects)
                    .ThenInclude(pu => pu.Project)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userProfile == null)
            {
                return NotFound();
            }
            bool isLoggedOut = !User.Identity.IsAuthenticated;
            bool isPrivate = userProfile.Visibility == false;
            if (isPrivate && isLoggedOut)
            {
                return View("PrivateProfile");
            }
            return View("MyPage", userProfile);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null) return NotFound();
            var model = new EditProfileViewModel
            {
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                UserName = user.UserName,
                Visibility = user.Visibility,

                // Adress
                HomeAddress = user.Address?.HomeAddress,
                ZipCode = user.Address?.ZipCode,
                City = user.Address?.City,

                // CV
                Skills = user.Skills,
                Education = user.Education,
                Experience = user.Experience,
                CurrentProfileImage = user.ProfileImage,
                CurrentCvImage = user.CvImage
            };
            return View(model);
        }

        // Sparar och tar emot ändringarna
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            var userToUpdate = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (userToUpdate != null)
            {
                userToUpdate.Name = model.Name;
                userToUpdate.PhoneNumber = model.PhoneNumber;
                userToUpdate.Email = model.Email;
                userToUpdate.UserName = model.UserName;
                userToUpdate.Visibility = model.Visibility;
                userToUpdate.Skills = model.Skills;
                userToUpdate.Education = model.Education;
                userToUpdate.Experience = model.Experience;

                if (userToUpdate.Address == null) userToUpdate.Address = new Address();
                userToUpdate.Address.HomeAddress = model.HomeAddress;
                userToUpdate.Address.ZipCode = model.ZipCode;
                userToUpdate.Address.City = model.City;

                if (model.NewProfileImageFile != null)
                {
                    string newFileName = await UploadFile(model.NewProfileImageFile);
                    userToUpdate.ProfileImage = newFileName;
                }
                else if (model.RemoveProfileImage)
                {
                    userToUpdate.ProfileImage = "default.jpg";
                }

                if (model.NewCvImageFile != null)
                {
                    string newFileName = await UploadFile(model.NewCvImageFile);
                    userToUpdate.CvImage = newFileName;
                }
                else if (model.RemoveCvImage)
                {
                    userToUpdate.CvImage = null;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("MyPage");
            }
            return View(model);
        }

        // Avaktivera konto
        [HttpGet]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user != null)
            {
                user.Deactivated = true;
                user.Visibility = false;
                await _context.SaveChangesAsync();
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        // Hjälpmetod för att spara filer
        private async Task<string> UploadFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}