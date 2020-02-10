using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PRP_SERVER.Models
{
    public class Contact
    {
        [Key]
        public long ContactId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsDeleted { get; set; }
    }
}
