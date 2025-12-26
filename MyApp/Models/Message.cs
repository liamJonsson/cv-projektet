using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public class Message
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int MessageId { get; set; }

            [Required(ErrorMessage = "Vänligen skriv ett meddelande.")]
            public string Text { get; set; }
            public bool Read { get; set; } = false;

            [Required(ErrorMessage = "Vänligen fyll i ditt namn när du ej är inloggad.")]
            public string SenderName { get; set; }

            //Foreign keys
            public int? SenderId { get; set; }
            public virtual User Sender { get; set; }

            [Required]
            public int ReceiverId { get; set; }
            public virtual User Receiver { get; set; }
        }
    }
