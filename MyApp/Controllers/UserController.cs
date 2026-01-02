using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using System.IO;
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
            var loggedInUserId = _userManager.GetUserId(User);

            if (loggedInUserId != null && int.Parse(loggedInUserId) != userProfile.Id)
            {
                userProfile.ProfileViews++;
                await _context.SaveChangesAsync();
            }

            if (isPrivate && isLoggedOut)
            {
                return RedirectToAction("Login");
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
                var existingUserWithEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserWithEmail != null && existingUserWithEmail.Id != userToUpdate.Id)
                {
                    ModelState.AddModelError("Email", "Denna e-postadress används redan av ett annat konto.");
                }

                var existingUserWithName = await _userManager.FindByNameAsync(model.UserName);
                if (existingUserWithName != null && existingUserWithName.Id != userToUpdate.Id)
                {
                    ModelState.AddModelError("UserName", "Användarnamnet är tyvärr upptaget.");
                }

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    var existingUserWithPhone = await _context.Users
                        .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != userToUpdate.Id);

                    if (existingUserWithPhone != null)
                    {
                        ModelState.AddModelError("PhoneNumber", "Detta telefonnummer är redan registrerat.");
                    }
                }
                if (!ModelState.IsValid)
                {
                    model.CurrentProfileImage = userToUpdate.ProfileImage;
                    model.CurrentCvImage = userToUpdate.CvImage;
                    return View(model);
                }
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
                    userToUpdate.CvImage = "default.jpg";
                }
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(userToUpdate, model.CurrentPassword, model.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
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

        // Hjälpmetod för att spara bildfiler
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

        // Exportera profil
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExportProfile(int id)
        {
            var user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.ParticipatingProjects)
                .ThenInclude(pu => pu.Project)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();
            bool isLoggedOut = !User.Identity.IsAuthenticated;
            if (user.Visibility == false && isLoggedOut)
            {
                return Forbid(); // Eller NotFound()
            }
            var exportData = new ProfileXmlDto
            {
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Skills = user.Skills?.Trim(),
                Education = user.Education?.Trim(),
                Experience = user.Experience?.Trim(),
                ZipCode = user.Address?.ZipCode,
                City = user.Address?.City
            };
            if (user.ParticipatingProjects != null)
            {
                foreach (var projUser in user.ParticipatingProjects)
                {
                    exportData.Projects.Add(new ProjectXmlDto
                    {
                        Title = projUser.Project.Title,
                        Description = projUser.Project.Description,
                        CodeLanguage = projUser.Project.CodeLanguage
                    });
                }
            }
            var serializer = new XmlSerializer(typeof(ProfileXmlDto));

            using (var stream = new MemoryStream())
            {
                var settings = new System.Xml.XmlWriterSettings
                {
                    Indent = true,             
                    Encoding = System.Text.Encoding.UTF8
                };

                using (var writer = System.Xml.XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, exportData);
                }
                var content = stream.ToArray();
                return File(content, "application/xml", $"Profil_{user.UserName}.xml");
            }
        }
    }

    // För XML-exporten
    public class ProfileXmlDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Skills { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }

        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public List<ProjectXmlDto> Projects { get; set; } = new List<ProjectXmlDto>();
    }

    public class ProjectXmlDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CodeLanguage { get; set; }
    }
}