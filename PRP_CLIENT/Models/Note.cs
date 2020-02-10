using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace PRP_CLIENT.Models
{
    public class Note
    {
        [Key]
        public long NoteId { get; set; }
        public string Content { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsDeleted { get; set; }
    }
}
