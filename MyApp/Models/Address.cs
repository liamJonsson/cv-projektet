using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Adress är obligatorisk.")]
        [StringLength(100, ErrorMessage = "Namnet får max vara 100 tecken.")]
        public string HomeAddress { get; set; }

        [Required(ErrorMessage = "Postnummer är obligatoriskt.")]
        [RegularExpression(@"^\d{3}\s?\d{2}$", ErrorMessage = "Postnummer ska vara i formatet 12345 eller 123 45.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Ort är obligatorisk.")]
        [StringLength(50, ErrorMessage = "Staden får max ha 50 tecken.")]
        public string City { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
