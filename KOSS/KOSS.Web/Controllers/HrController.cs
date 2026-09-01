using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize(Roles = "Executive,Finance")]
    public class HrController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Hr  -  قائمة الموظفين والأداء
        // ──────────────────────────────────────────────
        public ActionResult Index()
        {
            var staff = db.StaffMembers
                .Where(s => s.IsActive)
                .OrderBy(s => s.Role)
                .ToList();
            return View(staff);
        }

        // ──────────────────────────────────────────────
        //  POST: /Hr/LogBonus  -  إضافة مكافأة يدوية
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogBonus(int staffId, decimal amount, string reason)
        {
            var staff = db.StaffMembers.Find(staffId);
            if (staff == null) return HttpNotFound();

            staff.BonusBalance += amount;

            db.AuditLogs.Add(new AuditLog
            {
                TableName   = "StaffMembers",
                RecordId    = staffId,
                Action      = "BonusAdded",
                NewValue    = amount.ToString("N3"),
                Description = $"مكافأة يدوية: {reason}",
                ChangedBy   = User.Identity.GetUserName(),
                ChangedAt   = DateTime.Now
            });
            db.SaveChanges();

            TempData["Success"] = $"تمت إضافة مكافأة {amount:N3} د.ل لـ {staff.FullName}.";
            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  POST: /Hr/LogLiability  -  تسجيل مسؤولية مالية
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogLiability(int staffId, decimal amount, string reason)
        {
            var staff = db.StaffMembers.Find(staffId);
            if (staff == null) return HttpNotFound();

            staff.LiabilityBalance += amount;

            db.AuditLogs.Add(new AuditLog
            {
                TableName   = "StaffMembers",
                RecordId    = staffId,
                Action      = "LiabilityAdded",
                NewValue    = amount.ToString("N3"),
                Description = $"مسؤولية مالية: {reason}",
                ChangedBy   = User.Identity.GetUserName(),
                ChangedAt   = DateTime.Now
            });
            db.SaveChanges();

            TempData["Warning"] = $"تم تسجيل مسؤولية {amount:N3} د.ل على {staff.FullName}: {reason}";
            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  GET: /Hr/PayrollReport  -  تقرير الرواتب
        // ──────────────────────────────────────────────
        public ActionResult PayrollReport()
        {
            var staff = db.StaffMembers.Where(s => s.IsActive).ToList();
            return View(staff);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
