using System.ComponentModel.DataAnnotations;

namespace MyApp.InputModels
{
    public class RegisterInputModel
    {
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
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ ]*$", ErrorMessage = "Endast bokstäver tillåtet.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Användarnamn krävs.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖéÉ0-9._-]*$", ErrorMessage = "Innehåller otillåtna tecken.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lösenord krävs.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bekräfta lösenordet.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }
    }
}