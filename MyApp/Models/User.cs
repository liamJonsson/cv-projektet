using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class User
    {
        //UserId
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        //Name
        [Required(ErrorMessage = "Vänligen ange ett namn.")]
        public string Name { get; set; }

        //Profile image
        [DataType(DataType.ImageUrl)]
        public string ProfileImage { get; set; }

        //Phone number
        [Required(ErrorMessage = "Vänligen ange ett mobilnummer.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        //Email
        [Required(ErrorMessage = "Vänligen ange en mejladress.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //CV - Filename for the CV
        public string Cv { get; set; }

        //Username
        [Required(ErrorMessage = "Vänligen ange ett användarnamn.")]
        public string UserName { get; set; }

        //Password
        [Required(ErrorMessage = "Vänligen ange ett lösenord.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool Visibility { get; set; } = false;
        public bool Deactivated { get; set; } = false;

        //Foreign key - AdressId
        [Required]
        public int AddressId { get; set; }
        public virtual Address Address { get; set; }

        //Projects the current user is participating in
        public virtual ICollection<ProjectUser> ParticipatingProjects { get; set; } = new List<ProjectUser>();
    }
}
