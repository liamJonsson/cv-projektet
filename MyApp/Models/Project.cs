using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i en titel.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vänligen fyll i en beskrivning.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vänligen fyll i ett startdatum.")]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i ett programmeringsspråk.")]
        public string CodeLanguage { get; set; } = string.Empty;
        public string? ZipFile { get; set; }

        //Foreign key
        public int CreatorId { get; set; }
        public virtual User? Creator { get; set; }

        //Participants in the current project
        public virtual ICollection<ProjectUser> Participants { get; set; } = new List<ProjectUser>();
    }
}
