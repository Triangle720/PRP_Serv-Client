using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PRP_CLIENT.Models
{
    public class Branch
    {
        [Key]
        public long BranchId { get; set; }
        public string Name { get; set; }
    }
}
