using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyApp.InputModels
{
    public class EditProfileViewModel
    {
        // Vanlig data
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string HomeAddress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public string UserName { get; set; }
        public bool Visibility { get; set; }

        // CV-texter
        public string? Skills { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }

        // Bilder
        public string? CurrentProfileImage { get; set; }
        public string? CurrentCvImage { get; set; }

        [Display(Name = "Profilbild")]
        public IFormFile? NewProfileImageFile { get; set; }

        [Display(Name = "CV-bild")]
        public IFormFile? NewCvImageFile { get; set; }

        public bool RemoveProfileImage { get; set; }
        public bool RemoveCvImage { get; set; }
    }
}
