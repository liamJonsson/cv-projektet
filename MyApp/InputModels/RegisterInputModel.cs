using System.ComponentModel.DataAnnotations;

namespace MyApp.InputModels
{
    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Namn krävs")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Telefonnummer krävs")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mailadress krävs")]
        [EmailAddress(ErrorMessage = "Ogiltig mailadress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Adress krävs")]
        public string HomeAddress { get; set; }

        [Required(ErrorMessage = "Postnummer krävs")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Ort krävs")]
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