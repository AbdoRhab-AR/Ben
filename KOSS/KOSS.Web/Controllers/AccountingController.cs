using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Helpers;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Accounting  -  مصفوفة تحليل ربحية المشاريع وإغلاق الحسابات
        // ──────────────────────────────────────────────
        public ActionResult Index()
        {
            var requests = db.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.Contracts.Select(c => c.Payments))
                .Include(r => r.WorkOrders.Select(w => w.StockIssues))
                .Include(r => r.WorkOrders.Select(w => w.InstallationOrders))
                .Include(r => r.Expenses)
                .Where(r => r.Status >= KitchenRequestStatus.ContractActive)
                .OrderByDescending(r => r.UpdatedAt)
                .ToList();

            var reports = requests.Select(r => ProfitabilityCalculator.Calculate(r)).ToList();

            ViewBag.TotalRevenue = reports.Sum(r => r.ContractRevenue);
            ViewBag.TotalCost = reports.Sum(r => r.TotalProjectCost);
            ViewBag.TotalNetProfit = reports.Sum(r => r.NetProfit);

            return View(reports);
        }

        // ──────────────────────────────────────────────
        //  GET: /Accounting/ProjectCosting/5  -  كشف تكاليف وأرباح مشروع تفصيلي
        // ──────────────────────────────────────────────
        public ActionResult ProjectCosting(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var request = db.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.Contracts.Select(c => c.Payments))
                .Include(r => r.Contracts.Select(c => c.PaymentSchedules))
                .Include(r => r.WorkOrders.Select(w => w.StockIssues.Select(si => si.Items.Select(sii => sii.ItemMaster))))
                .Include(r => r.WorkOrders.Select(w => w.InstallationOrders))
                .Include(r => r.WorkOrders.Select(w => w.QualityChecks.Select(qc => qc.SnagItems)))
                .Include(r => r.WorkOrders.Select(w => w.HandoverDocuments))
                .Include(r => r.Expenses)
                .FirstOrDefault(r => r.Id == id.Value);

            if (request == null) return HttpNotFound();

            ViewBag.Report = ProfitabilityCalculator.Calculate(request);
            ViewBag.ClosingCheck = RequestWorkflowEngine.VerifyClosingConditions(request);

            return View(request);
        }

        // ──────────────────────────────────────────────
        //  POST: /Accounting/CloseProject  -  إغلاق المشروع محاسبياً وأرشفته
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CloseProject(int kitchenRequestId, string notes)
        {
            var request = db.KitchenRequests
                .Include(r => r.Contracts.Select(c => c.Payments))
                .Include(r => r.WorkOrders.Select(w => w.QualityChecks.Select(qc => qc.SnagItems)))
                .FirstOrDefault(r => r.Id == kitchenRequestId);

            if (request == null) return HttpNotFound();

            var check = RequestWorkflowEngine.VerifyClosingConditions(request);
            if (!check.CanClose)
            {
                TempData["Error"] = $"لا يمكن إغلاق المشروع حتى استيفاء الشروط التالية: {string.Join("، ", check.PendingConditions)}";
                return RedirectToAction("ProjectCosting", new { id = kitchenRequestId });
            }

            RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.Closed, User.Identity.GetUserName(), $"إغلاق المشروع محاسبياً وتنفيذياً ومراجعة تقرير الربحية: {notes}");
            db.SaveChanges();

            TempData["Success"] = $"تم إغلاق المشروع [{request.RequestNumber}] رسمياً وترحيل كامل قيود الربحية.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
