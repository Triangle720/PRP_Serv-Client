using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PRP_SERVER.Models
{
    public class Role
    {
        [Key]
        public long RoleId { get; set; }
        public string Name { get; set; }
    }
}
