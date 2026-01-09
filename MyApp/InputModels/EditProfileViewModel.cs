using System.ComponentModel.DataAnnotations;
using MyApp.Models;
namespace MyApp.InputModels
{
    public class EditProfileViewModel
    {
        // Vanlig data
        [Required(ErrorMessage = "Namn krävs.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ ]*$", ErrorMessage = "Endast bokstäver tillåtet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Telefonnummer krävs.")]
        [StringLength(16, ErrorMessage = "Max 16 tecken tillåtet.")]
        [RegularExpression(@"^[0-9+ ]*$", ErrorMessage = "Endast siffror tillåtet.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mailadress krävs.")]
        [EmailAddress(ErrorMessage = "Ogiltig mailadress.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Adress krävs.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ0-9 ]*$", ErrorMessage = "Endast bokstäver och siffror tillåtet.")]
        public string HomeAddress { get; set; }

        [Required(ErrorMessage = "Postnummer krävs.")]
        [StringLength(5, ErrorMessage = "Max 5 tecken tillåtet.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Endast siffror tillåtet.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Ort krävs.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ0-9._-]*$", ErrorMessage = "Innehåller otillåtna tecken.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Användarnamn krävs.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ0-9._-]*$", ErrorMessage = "Innehåller otillåtna tecken.")]
        public string UserName { get; set; }
        public bool Visibility { get; set; }

        // Lösenord

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
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

        public ICollection<ProjectUser>? ParticipatingProjects { get; set; }
    }
}
