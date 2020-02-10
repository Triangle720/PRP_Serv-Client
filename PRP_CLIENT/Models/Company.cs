using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PRP_CLIENT.Models
{
    public class Company
    {
        [Key]
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Nip { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public long BranchId { get; set; }
        public Company Branch { get; set; }
        public bool IsDeleted { get; set; }
    }
}
