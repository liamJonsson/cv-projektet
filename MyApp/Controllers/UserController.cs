using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.InputModels;
using MyApp.Models;
using System.Xml.Serialization;



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

        // Min profil
        [HttpGet]
        public async Task<IActionResult> Index()
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

        // Registrera
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterInputModel model)
        {
            // Manuell validering för att se till så att E-post, Användarnamn och Telefonnummer är unika.
            // Identity hanterar detta också, men vi gör det här för att ge tydligare felmeddelanden direkt.
            if (ModelState.IsValid)
            {
                var existingUserWithEmail = await _userManager.FindByEmailAsync(model.Email);

                if (existingUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "Denna e-postadress används redan av ett annat konto.");
                }

                var existingUserWithName = await _userManager.FindByNameAsync(model.UserName);

                if (existingUserWithName != null)
                {
                    ModelState.AddModelError("UserName", "Användarnamnet är tyvärr upptaget.");
                }

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    var existingUserWithPhone = await _context.Users
                        .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

                    if (existingUserWithPhone != null)
                    {
                        ModelState.AddModelError("PhoneNumber", "Detta telefonnummer är redan registrerat.");
                    }
                }

                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                // Vi Skapar Adress först då User-tabellen har en Foreign Key till Address
                var newAddress = new Address
                {
                    HomeAddress = model.HomeAddress,
                    ZipCode = model.ZipCode,
                    City = model.City
                };

                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync(); // Adress får ett ID från databasen
                
                // Här mappar vi input-modellen till vår databas-modell (User) och kopplar adressen.
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
                    EmailConfirmed = true
                };

                // Skapar användaren via Identity-systemet som hanterar lösenordshashningen
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Logga in användaren direkt vid lyckad registrering
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
            // Försöker logga in användaren
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);

                // Om användaren var avaktiverad, återaktivera kontot
                if (user != null && user.Deactivated)
                {
                    user.Deactivated = false;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Välkommen tillbaka! Ditt konto har återaktiverats.";
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Fel användarnamn eller lösenord.";
            return View();
        }

        // Logga ut
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Profilvisning
        [HttpGet]
        public async Task<IActionResult> ViewProfile(int id)
        {
            var userProfile = await _context.Users.FindAsync(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            var loggedInUserId = _userManager.GetUserId(User);
            bool isOwner = loggedInUserId != null && int.Parse(loggedInUserId) == userProfile.Id;

            // Räkna endast om man tittar på någon annans profil
            if (!isOwner)
            {
                userProfile.ProfileViews++;
                await _context.SaveChangesAsync();
            }

            // Skicka vidare till profilsidan
            return RedirectToAction("Profile", new { id });
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

            //Om kontot är avaktiverat ska det inte gå att nå via direktlänk
            if (userProfile == null)
            {
                return NotFound();
            }
            // Om profilen är Privat krävs inloggning
            bool isLoggedOut = !User.Identity.IsAuthenticated;
            bool isPrivate = userProfile.Visibility == false;

            if (isPrivate && isLoggedOut)
            {
                return RedirectToAction("Login");
            }

            return View("Index", userProfile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.ParticipatingProjects)
                .ThenInclude(pu => pu.Project)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null) return NotFound();

            // Mapper databasmodellen till vår ViewModel för redigering
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
                CurrentCvImage = user.CvImage,

                // Projekt
                ParticipatingProjects = user.ParticipatingProjects
            };

            return View(model);
        }

        // Sparar och tar emot ändringarna
        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            // Om lösenordsfälten är tomma, anta att användaren inte vill byta lösenord.
            if (string.IsNullOrEmpty(model.CurrentPassword) && string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.Remove("CurrentPassword");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
            }

            var userId = _userManager.GetUserId(User);
            var userToUpdate = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.ParticipatingProjects)
                .ThenInclude(pu => pu.Project)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            
            // Samma säkerhetskontroll som i Register-metoden
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
                // Om användaren försöker byta lösenord, verifiera nuvarande lösenord
                if (!string.IsNullOrEmpty(model.CurrentPassword))
                {
                    var isPasswordCorrect = await _userManager.CheckPasswordAsync(userToUpdate, model.CurrentPassword);

                    if (!isPasswordCorrect)
                    {
                        ModelState.AddModelError("CurrentPassword", "Felaktigt nuvarande lösenord.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    // Återställ bildreferenserna vid fel så att vyn inte kraschar eller visar tomt.
                    model.CurrentProfileImage = userToUpdate.ProfileImage;
                    model.CurrentCvImage = userToUpdate.CvImage;
                    model.ParticipatingProjects = userToUpdate.ParticipatingProjects;
                    return View(model);
                }
                // Uppdatera användardata
                userToUpdate.Name = model.Name;
                userToUpdate.PhoneNumber = model.PhoneNumber;
                userToUpdate.Email = model.Email;
                userToUpdate.UserName = model.UserName;
                userToUpdate.Visibility = model.Visibility;
                userToUpdate.Skills = model.Skills;
                userToUpdate.Education = model.Education;
                userToUpdate.Experience = model.Experience;

                // Uppdatera Adress 
                if (userToUpdate.Address == null) userToUpdate.Address = new Address();
                userToUpdate.Address.HomeAddress = model.HomeAddress;
                userToUpdate.Address.ZipCode = model.ZipCode;
                userToUpdate.Address.City = model.City;

                // Filuppladdning för Profilbild
                if (model.NewProfileImageFile != null)
                {
                    string newFileName = await UploadFile(model.NewProfileImageFile);
                    userToUpdate.ProfileImage = newFileName;
                }

                else if (model.RemoveProfileImage)
                {
                    userToUpdate.ProfileImage = "default.jpg";
                }

                // Filuppladdning för CV-bild
                if (model.NewCvImageFile != null)
                {
                    string newFileName = await UploadFile(model.NewCvImageFile);
                    userToUpdate.CvImage = newFileName;
                }

                else if (model.RemoveCvImage)
                {
                    userToUpdate.CvImage = "default.jpg";
                }
                
                // Genomför lösenordsbyte via Identity
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(userToUpdate, model.CurrentPassword, model.NewPassword);

                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Återställ projektlista vid lösenordsfel också
                        model.ParticipatingProjects = userToUpdate.ParticipatingProjects;

                        return View(model);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Din profil har uppdaterats!";

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // Avaktivera konto
        [HttpPost]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Index", "Home");
            
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user != null)
            {
                user.Deactivated = true;

                await _context.SaveChangesAsync();

                // Logga ut användaren
                await _signInManager.SignOutAsync();

                // Meddelande till startsidan
                TempData["SuccessMessage"] = "Ditt konto har avaktiverats.";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> FindSimilarUsers(int id)
        {
            //Hämtar användaren man har klickat sig in på
            var selectedUser = await _context.Users.Where(u => u.Id == id)
                .Include(u => u.ParticipatingProjects)
                .ThenInclude(pu => pu.Project)
                .FirstOrDefaultAsync();

            if (selectedUser == null)
            {
                return Content("Ett fel uppstod då profilen du besöker inte kunde hittas");
            }

            //Hämtar den valda användarens programmeringsspråk
            var selectedUserCodeLanguages = selectedUser.ParticipatingProjects
                .Select(pu => pu.Project.CodeLanguage)
                .Distinct()
                .ToList();

            //Om den valda användaren inte har några programmeringsspråk returneras en tom lista till partial view
            if (!selectedUserCodeLanguages.Any())
            {
                return PartialView("_SimilarUsers", new List<User>());
            }

            //Hämtar max 3 aktiva användare som har arbetat med samma programmeringsspråk som personen man har klickat sig in på har gjort
            //Hämtar enbart offentliga profiler om man själv ej är inloggad
            var userMatches = await _context.Users
                .Include(u => u.Address)
                .Include(u => u.ParticipatingProjects)
                .ThenInclude(pu => pu.Project)
                .Where(u => u.Id != id && u.Deactivated == false && (User.Identity.IsAuthenticated || u.Visibility == true))
                .Where(u => u.ParticipatingProjects.Any(pu => selectedUserCodeLanguages.Contains(pu.Project.CodeLanguage)))
                .Take(3)
                .ToListAsync();

            return PartialView("_SimilarUsers", userMatches);
        }


        // Hjälpmetod för att spara bildfiler
        private async Task<string> UploadFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // Använder GUID för att garantera att filnamnet är unikt och undvika överskrivning
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

            // Säkerhetskoll vid export
            bool isLoggedOut = !User.Identity.IsAuthenticated;

            if (user.Visibility == false && isLoggedOut)
            {
                return NotFound();
            }

            // Mappa till DTO (Data Transfer Object) för att kontrollera exakt vilken data som exponeras i filen
            var exportData = new ProfileXmlDto
            {
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Skills = user.Skills?.Trim(),
                Education = user.Education?.Trim(),
                Experience = user.Experience?.Trim(),
                HomeAddress = user.Address?.HomeAddress,
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
                    // Gör XML:en läsbar för människor
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

    // DTO-klasser För XML-exporten
    public class ProfileXmlDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Skills { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }

        public string HomeAddress { get; set; }
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