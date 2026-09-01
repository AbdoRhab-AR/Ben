using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class InquiriesController : Controller
    {
        private readonly AppDbContext _context;

        public InquiriesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(InquiryStatus? status, string search)
        {
            var query = _context.CustomerInquiries
                .Include(i => i.Customer)
                .Include(i => i.ConvertedKitchenRequest)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(i => i.Customer.Name.Contains(search) || i.Customer.Phone.Contains(search) || i.Location.Contains(search));

            var list = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();
            ViewBag.Status = status;
            ViewBag.Search = search;
            return View(list);
        }

        public async Task<IActionResult> Create(int? customerId)
        {
            ViewBag.Customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
            var model = new CustomerInquiry { CustomerId = customerId ?? 0 };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerInquiry model, string customerName, string customerPhone, string customerDistrict)
        {
            if (model.CustomerId == 0)
            {
                if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerPhone))
                {
                    ModelState.AddModelError("", "يرجى تحديد عميل مسجل أو إدخال اسم ورقم هاتف العميل الجديد.");
                }
                else
                {
                    var existing = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == customerPhone.Trim());
                    if (existing != null)
                    {
                        model.CustomerId = existing.Id;
                    }
                    else
                    {
                        var newCust = new Customer
                        {
                            Name = customerName.Trim(),
                            Phone = customerPhone.Trim(),
                            District = customerDistrict,
                            CreatedBy = User.Identity?.Name ?? "Admin"
                        };
                        _context.Customers.Add(newCust);
                        await _context.SaveChangesAsync();
                        model.CustomerId = newCust.Id;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.CreatedBy = User.Identity?.Name ?? "Admin";
                model.Status = InquiryStatus.New;

                _context.CustomerInquiries.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تسجيل الاستفسار بنجاح.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertToRequest(int inquiryId)
        {
            var inq = await _context.CustomerInquiries.Include(i => i.Customer).FirstOrDefaultAsync(i => i.Id == inquiryId);
            if (inq == null) return NotFound();

            var req = new KitchenRequest
            {
                RequestNumber = $"REQ-{DateTime.Now.Year}-{new Random().Next(10000, 99999)}",
                CustomerId = inq.CustomerId,
                Location = inq.Location ?? inq.Customer.District ?? "طرابلس",
                LayoutType = inq.PreferredLayout ?? KitchenLayoutType.Straight,
                Status = KitchenRequestStatus.AwaitingSiteVisit,
                Notes = $"تحويل من استفسار رقم #{inq.Id}. {inq.Notes}",
                CreatedBy = User.Identity?.Name ?? "Admin"
            };

            _context.KitchenRequests.Add(req);
            await _context.SaveChangesAsync();

            inq.Status = InquiryStatus.ConvertedToRequest;
            inq.ConvertedKitchenRequestId = req.Id;

            _context.RequestStatusHistories.Add(new RequestStatusHistory
            {
                KitchenRequestId = req.Id,
                OldStatus = KitchenRequestStatus.NewInquiry,
                NewStatus = KitchenRequestStatus.AwaitingSiteVisit,
                ChangedBy = User.Identity?.Name ?? "Admin",
                Notes = "تم فتح الطلب وتحويله من استفسار بيعي رسمي."
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"تم تحويل الاستفسار إلى طلب مطبخ رسمي رقم: {req.RequestNumber}";
            return RedirectToAction("Details", "Requests", new { id = req.Id });
        }
    }
}
