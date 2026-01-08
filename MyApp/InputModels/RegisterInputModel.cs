using System.ComponentModel.DataAnnotations;

namespace MyApp.InputModels
{
    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Namn krävs")]
        [StringLength(50, ErrorMessage = "Namnet får vara max 50 tecken.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ\s-]+$", ErrorMessage = "Namnet får endast innehålla bokstäver.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Telefonnummer krävs")]
        [Phone(ErrorMessage = "Ogiltigt telefonnummer.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Telefonnumret måste vara mellan 10 och 20 tecken.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mailadress krävs")]
        [EmailAddress(ErrorMessage = "Ogiltig mailadress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Adress krävs")]
        public string HomeAddress { get; set; }

        [Required(ErrorMessage = "Postnummer krävs")]
        [RegularExpression(@"^\d{3}\s?\d{2}$", ErrorMessage = "Ange ett giltigt postnummer (5 siffror).")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Ort krävs")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ\s-]+$", ErrorMessage = "Ort får endast innehålla bokstäver.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Användarnamn krävs")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lösenord krävs")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bekräfta lösenordet")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }
    }
}