using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class DesignController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        public ActionResult Index()
        {
            var contracts = db.Contracts
                .Include("Client").Include("Units")
                .Where(c => c.Status == ContractStatus.FeePaid ||
                            c.Status == ContractStatus.Designed)
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();
            return View(contracts);
        }

        [HttpPost, Authorize(Roles = "Designer,Executive")]
        public ActionResult MarkDesigned(int contractId, string notes)
        {
            var contract = db.Contracts.Include("Units").FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            contract.Status    = ContractStatus.Designed;
            contract.UpdatedAt = DateTime.Now;

            // تسجيل مكافأة المصمم (50 د.ل لكل وحدة مصممة)
            decimal bonus   = contract.Units.Count * 50m;
            string designer = User.Identity.GetUserName();
            var staff = db.StaffMembers.FirstOrDefault(s => s.UserId == User.Identity.GetUserId());
            if (staff != null) staff.BonusBalance += bonus;

            db.AuditLogs.Add(new AuditLog
            {
                TableName   = "Contracts",
                RecordId    = contractId,
                Action      = "StatusChange",
                OldValue    = "FeePaid",
                NewValue    = "Designed",
                Description = $"تم التصميم بواسطة {designer}. مكافأة: {bonus:N3} د.ل",
                ChangedBy   = designer,
                ChangedAt   = DateTime.Now
            });
            db.SaveChanges();

            TempData["Success"] = $"✅ تم تسجيل اكتمال التصميم. مكافأة المصمم: {bonus:N3} د.ل";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
