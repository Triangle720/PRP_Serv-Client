using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PRP_CLIENT.Models
{
    public class User
    {
        [Key]
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }
        public virtual Role Role { get; set; }
        public bool IsDeleted { get; set; }

    }
}
