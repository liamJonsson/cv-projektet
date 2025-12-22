using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId {  get; set; }
        public string HomeAddress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
