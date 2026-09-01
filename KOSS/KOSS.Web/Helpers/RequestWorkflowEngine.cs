using System;
using System.Collections.Generic;
using System.Linq;
using KOSS.Web.Models;

namespace KOSS.Web.Helpers
{
    // ============================================================
    //  محرك تدفق العمل والحالات الصارم (Request Workflow Engine)
    // ============================================================
    public class WorkflowCheckResult
    {
        public bool IsAllowed { get; set; }
        public string ErrorMessage { get; set; }

        public WorkflowCheckResult(bool isAllowed, string errorMessage = "")
        {
            IsAllowed = isAllowed;
            ErrorMessage = errorMessage;
        }
    }

    public class ProjectClosingCheckResult
    {
        public bool CanClose { get; set; }
        public List<string> PendingConditions { get; set; } = new List<string>();

        public ProjectClosingCheckResult(bool canClose, List<string> pending = null)
        {
            CanClose = canClose;
            if (pending != null) PendingConditions = pending;
        }
    }

    public static class RequestWorkflowEngine
    {
        // مصفوفة الانتقالات المسموحة بين الـ 24 حالة
        private static readonly Dictionary<KitchenRequestStatus, List<KitchenRequestStatus>> AllowedTransitions =
            new Dictionary<KitchenRequestStatus, List<KitchenRequestStatus>>
            {
                { KitchenRequestStatus.NewInquiry, new List<KitchenRequestStatus> { KitchenRequestStatus.Qualified, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.Qualified, new List<KitchenRequestStatus> { KitchenRequestStatus.RequestOpened, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.RequestOpened, new List<KitchenRequestStatus> { KitchenRequestStatus.AwaitingSiteVisit, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.AwaitingSiteVisit, new List<KitchenRequestStatus> { KitchenRequestStatus.SiteVisitCompleted, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.SiteVisitCompleted, new List<KitchenRequestStatus> { KitchenRequestStatus.InDesign, KitchenRequestStatus.AwaitingSiteVisit, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.InDesign, new List<KitchenRequestStatus> { KitchenRequestStatus.AwaitingDesignApproval, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.AwaitingDesignApproval, new List<KitchenRequestStatus> { KitchenRequestStatus.InPricing, KitchenRequestStatus.InDesign, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.InPricing, new List<KitchenRequestStatus> { KitchenRequestStatus.QuotationSent, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.QuotationSent, new List<KitchenRequestStatus> { KitchenRequestStatus.QuotationAccepted, KitchenRequestStatus.NegotiationOrRevision, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.NegotiationOrRevision, new List<KitchenRequestStatus> { KitchenRequestStatus.InDesign, KitchenRequestStatus.InPricing, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.QuotationAccepted, new List<KitchenRequestStatus> { KitchenRequestStatus.AwaitingContractAndDeposit, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.AwaitingContractAndDeposit, new List<KitchenRequestStatus> { KitchenRequestStatus.ContractActive, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.ContractActive, new List<KitchenRequestStatus> { KitchenRequestStatus.InPlanning, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.InPlanning, new List<KitchenRequestStatus> { KitchenRequestStatus.InManufacturing, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.InManufacturing, new List<KitchenRequestStatus> { KitchenRequestStatus.ReadyForInstallation, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.ReadyForInstallation, new List<KitchenRequestStatus> { KitchenRequestStatus.InstallationScheduled, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.InstallationScheduled, new List<KitchenRequestStatus> { KitchenRequestStatus.InInstallation, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.InInstallation, new List<KitchenRequestStatus> { KitchenRequestStatus.ReadyForHandover, KitchenRequestStatus.AwaitingSnagResolution, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.AwaitingSnagResolution, new List<KitchenRequestStatus> { KitchenRequestStatus.ReadyForHandover, KitchenRequestStatus.InInstallation, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.ReadyForHandover, new List<KitchenRequestStatus> { KitchenRequestStatus.HandoverCompleted, KitchenRequestStatus.AwaitingSnagResolution, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.HandoverCompleted, new List<KitchenRequestStatus> { KitchenRequestStatus.AwaitingFinalBalance, KitchenRequestStatus.Closed, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.AwaitingFinalBalance, new List<KitchenRequestStatus> { KitchenRequestStatus.Closed, KitchenRequestStatus.CancelledOrRejected } },
                { KitchenRequestStatus.Closed, new List<KitchenRequestStatus>() }, // حالة نهائية
                { KitchenRequestStatus.CancelledOrRejected, new List<KitchenRequestStatus>() } // حالة نهائية
            };

        // ============================================================
        //  التحقق من إمكانية الانتقال وشروط العمل الإلزامية
        // ============================================================
        public static WorkflowCheckResult CanTransition(KitchenRequest request, KitchenRequestStatus targetStatus)
        {
            if (request == null) return new WorkflowCheckResult(false, "الطلب غير موجود.");

            if (request.Status == targetStatus) return new WorkflowCheckResult(true, "");

            // 1. فحص مصفوفة الانتقالات
            if (!AllowedTransitions.ContainsKey(request.Status) || !AllowedTransitions[request.Status].Contains(targetStatus))
            {
                return new WorkflowCheckResult(false, $"لا يُسمح بالانتقال المباشر من [{request.Status}] إلى [{targetStatus}]. يجب اتباع تسلسل العمل المعتمد.");
            }

            // 2. فحص الشروط الفنية والتجارية الخاصة بكل مرحلة
            switch (targetStatus)
            {
                case KitchenRequestStatus.InDesign:
                    bool hasApprovedVisit = request.SiteVisits != null && request.SiteVisits.Any(v => v.Status == SiteVisitStatus.Approved);
                    if (!hasApprovedVisit)
                        return new WorkflowCheckResult(false, "شرط إلزامي: لا يمكن الانتقال إلى مرحلة التصميم إلا بعد اعتماد المعاينة الميدانية ورفع القياسات.");
                    break;

                case KitchenRequestStatus.InPricing:
                    bool hasApprovedDesign = request.DesignVersions != null && request.DesignVersions.Any(d => d.Status == DesignVersionStatus.ApprovedByCustomer || d.Status == DesignVersionStatus.InternalReview);
                    if (!hasApprovedDesign)
                        return new WorkflowCheckResult(false, "شرط إلزامي: لا يمكن التسعير إلا بناءً على إصدار تصميم معتمد.");
                    break;

                case KitchenRequestStatus.AwaitingContractAndDeposit:
                    bool hasAcceptedQuo = request.Quotations != null && request.Quotations.Any(q => q.Status == QuotationStatus.Accepted);
                    if (!hasAcceptedQuo)
                        return new WorkflowCheckResult(false, "شرط إلزامي: يجب قبول عرض السعر أولاً قبل إنشاء العقد.");
                    break;

                case KitchenRequestStatus.ContractActive:
                    var contract = request.ActiveContract ?? (request.Contracts != null ? request.Contracts.FirstOrDefault() : null);
                    if (contract == null)
                        return new WorkflowCheckResult(false, "شرط إلزامي: لا يوجد عقد مسجل لهذا الطلب.");
                    if (contract.TotalPaid < contract.RequiredDeposit && contract.RequiredDeposit > 0)
                        return new WorkflowCheckResult(false, $"شرط إلزامي: لتفعيل العقد يجب استلام دفعة العربون المطلوبة ({contract.RequiredDeposit:N3} د.ل). المدفوع حالياً: {contract.TotalPaid:N3} د.ل.");
                    break;

                case KitchenRequestStatus.InManufacturing:
                    var wo = request.CurrentWorkOrder ?? (request.WorkOrders != null ? request.WorkOrders.FirstOrDefault() : null);
                    if (wo == null)
                        return new WorkflowCheckResult(false, "شرط إلزامي: يجب إصدار أمر التنفيذ وخطة المواد قبل بدء التصنيع.");
                    break;

                case KitchenRequestStatus.ReadyForInstallation:
                    var curWo = request.CurrentWorkOrder ?? (request.WorkOrders != null ? request.WorkOrders.FirstOrDefault() : null);
                    bool passedQc = curWo != null && curWo.QualityChecks != null && curWo.QualityChecks.Any(q => q.Passed);
                    if (!passedQc)
                        return new WorkflowCheckResult(false, "شرط إلزامي: يجب اجتياز فحص الجودة والمطابقة بالمصنع قبل الإعلان عن الجاهزية للتركيب.");
                    break;

                case KitchenRequestStatus.HandoverCompleted:
                    var currentWo = request.CurrentWorkOrder ?? (request.WorkOrders != null ? request.WorkOrders.FirstOrDefault() : null);
                    bool hasHandover = currentWo != null && currentWo.HandoverDocuments != null && currentWo.HandoverDocuments.Any(h => h.CustomerAccepted);
                    if (!hasHandover)
                        return new WorkflowCheckResult(false, "شرط إلزامي: يجب توقيع محضر التسليم النهائي من العميل أولاً.");
                    break;

                case KitchenRequestStatus.Closed:
                    var check = VerifyClosingConditions(request);
                    if (!check.CanClose)
                        return new WorkflowCheckResult(false, $"لا يمكن إغلاق المشروع حتى استيفاء الشروط التالية: {string.Join("، ", check.PendingConditions)}");
                    break;
            }

            return new WorkflowCheckResult(true, "");
        }

        // ============================================================
        //  فحص شروط الإغلاق الـ 9 للمشروع
        // ============================================================
        public static ProjectClosingCheckResult VerifyClosingConditions(KitchenRequest request)
        {
            var pending = new List<string>();

            var contract = request.ActiveContract ?? (request.Contracts != null ? request.Contracts.FirstOrDefault() : null);
            if (contract != null && contract.RemainingBalance > 0.01m)
            {
                pending.Add($"يوجد رصيد متبقٍ غير مسدد للعميل ({contract.RemainingBalance:N3} د.ل)");
            }

            var wo = request.CurrentWorkOrder ?? (request.WorkOrders != null ? request.WorkOrders.FirstOrDefault() : null);
            if (wo == null || wo.Status != WorkOrderStatus.Completed)
            {
                pending.Add("أمر التنفيذ غير مكتمل نهائياً");
            }

            if (wo != null && wo.QualityChecks != null)
            {
                var openSnags = wo.QualityChecks.SelectMany(q => q.SnagItems ?? new List<SnagItem>()).Count(s => !s.IsResolved);
                if (openSnags > 0)
                {
                    pending.Add($"توجد ({openSnags}) ملاحظات ونواقص غير معالجة");
                }
            }

            return new ProjectClosingCheckResult(pending.Count == 0, pending);
        }

        // ============================================================
        //  تنفيذ الانتقال وتسجيل السجل التاريخي
        // ============================================================
        public static void Transition(KossDbContext db, KitchenRequest request, KitchenRequestStatus targetStatus, string userName, string reason)
        {
            var oldStatus = request.Status;
            request.Status = targetStatus;
            request.UpdatedAt = DateTime.Now;

            // تسجيل في السجل التاريخي
            db.RequestStatusHistories.Add(new RequestStatusHistory
            {
                KitchenRequestId = request.Id,
                OldStatus = oldStatus,
                NewStatus = targetStatus,
                Notes = reason,
                ChangedBy = userName,
                ChangedAt = DateTime.Now
            });

            // تسجيل في AuditLog العام
            db.AuditLogs.Add(new AuditLog
            {
                TableName = "KitchenRequest",
                RecordId = request.Id,
                Action = "StatusTransition",
                OldValue = oldStatus.ToString(),
                NewValue = targetStatus.ToString(),
                Description = $"تغيير حالة الطلب {request.RequestNumber}: {reason}",
                ChangedBy = userName,
                ChangedAt = DateTime.Now
            });
        }
    }
}
