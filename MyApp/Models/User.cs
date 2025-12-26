using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyApp.Models
{
    public class User : IdentityUser<int>
    {
        //Name
        [Required(ErrorMessage = "Vänligen ange ett namn.")]
        public string Name { get; set; }
        //Profile image
        [DataType(DataType.ImageUrl)]
        public string ProfileImage { get; set; }
        //CV - Filename for the CV
        public string Cv { get; set; }
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
