using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public string Text { get; set; }
        public bool Read { get; set; }

        //Foreign keys
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }

        public int ReceiverId { get; set; }
        public virtual User Receiver { get; set; }
    }
}
