using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyApp.InputModels
{
    public class EditProfileViewModel
    {
        // Vanlig data
        [Required(ErrorMessage = "Du måste ange ett namn.")]
        [StringLength(50, ErrorMessage = "Namnet får vara max 50 tecken.")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Telefonnummer krävs.")]
        [Phone(ErrorMessage = "Ogiltigt telefonnummer.")]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "E-postadress krävs.")]
        [EmailAddress(ErrorMessage = "Ange en giltig e-postadress.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Gatuadress krävs.")]
        public string HomeAddress { get; set; }

        [Required(ErrorMessage = "Postnummer krävs.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Ort krävs.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Användarnamn krävs.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Måste vara mellan 3 och 20 tecken.")]
        public string UserName { get; set; }
        public bool Visibility { get; set; }

        // Lösenord

        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta nytt lösenord")]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string? ConfirmPassword { get; set; }

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
