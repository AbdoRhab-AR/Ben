using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        public string Username { get; set; }
        
        [Required]
        public string Action { get; set; } // Create, Update, Delete, Login, Logout, View
        
        [Required]
        public string EntityName { get; set; }
        
        public string EntityId { get; set; }
        
        public string Description { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        public string IpAddress { get; set; }
    }
}
