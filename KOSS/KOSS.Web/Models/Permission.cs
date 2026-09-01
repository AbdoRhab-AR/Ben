using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}
