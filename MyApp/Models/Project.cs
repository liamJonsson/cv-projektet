using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string CodeLanguage { get; set; }
        public string ZipFile { get; set; }

        //Foreign key
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        //Participants in the current project
        public virtual ICollection<ProjectUser> Participants { get; set; }
    }
}
