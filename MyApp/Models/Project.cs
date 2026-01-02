using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CodeLanguage { get; set; } = string.Empty;
        public string? ZipFile { get; set; }

        //Foreign key
        public int CreatorId { get; set; }
        public virtual User? Creator { get; set; }

        //Participants in the current project
        public virtual ICollection<ProjectUser> Participants { get; set; } = new List<ProjectUser>();
    }
}
