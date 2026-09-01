using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KOSS.Web.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string Username { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        public string Password { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; } = "Executive"; // Executive, Sales, Designer, Production, Installation, Accounting

        public List<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
