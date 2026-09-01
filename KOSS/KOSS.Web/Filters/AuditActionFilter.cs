using Microsoft.AspNetCore.Mvc.Filters;
using KOSS.Web.Models;
using System;

namespace KOSS.Web.Filters
{
    public class AuditActionFilter : IActionFilter
    {
        private readonly AppDbContext _context;

        public AuditActionFilter(AppDbContext context)
        {
            _context = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var username = context.HttpContext.User.Identity.Name;
                var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
                var action = context.RouteData.Values["action"]?.ToString() ?? "";
                var url = context.HttpContext.Request.Path;

                if (controller == "AuditLogs") return;

                if (context.HttpContext.Request.Method == "GET")
                {
                    var log = new AuditLog
                    {
                        Username = username,
                        Action = "View",
                        EntityName = controller,
                        Description = $"تصفح {controller}/{action} - الرابط: {url}",
                        Timestamp = DateTime.Now,
                        IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString()
                    };

                    _context.AuditLogs.Add(log);
                    _context.SaveChanges();
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
