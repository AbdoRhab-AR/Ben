using System;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using KOSS.Web.Models;

namespace KOSS.Web.Helpers
{
    // ============================================================
    //  مولّد ملفات PDF - الإيصالات وأوامر الشراء
    // ============================================================
    public static class PdfHelper
    {
        // ألوان الشركة
        private static readonly BaseColor PrimaryColor  = new BaseColor(0x1a, 0x5f, 0x3c); // أخضر داكن
        private static readonly BaseColor AccentColor   = new BaseColor(0xf0, 0xa5, 0x00); // ذهبي
        private static readonly BaseColor LightGray     = new BaseColor(0xf5, 0xf5, 0xf5);

        /// <summary>
        /// ينشئ إيصال استلام دفعة بصيغة PDF مع ختم الشركة
        /// </summary>
        public static byte[] GeneratePaymentReceipt(Payment payment, Contract contract, Client client)
        {
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A5, 36, 36, 54, 36);
                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // تعريف الخطوط
                string fontPath = HttpContext.Current.Server.MapPath("~/Content/fonts/arial.ttf");
                BaseFont baseFont;
                try
                {
                    baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                }
                catch
                {
                    // احتياطي: استخدام الخط المدمج
                    baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                }

                var titleFont   = new Font(baseFont, 16, Font.BOLD, PrimaryColor);
                var headerFont  = new Font(baseFont, 12, Font.BOLD, BaseColor.WHITE);
                var normalFont  = new Font(baseFont, 10, Font.NORMAL, BaseColor.DARK_GRAY);
                var boldFont    = new Font(baseFont, 10, Font.BOLD, BaseColor.BLACK);
                var smallFont   = new Font(baseFont, 8, Font.NORMAL, BaseColor.GRAY);

                // ترويسة الشركة
                var headerTable = new PdfPTable(1) { WidthPercentage = 100 };
                var headerCell = new PdfPCell(new Phrase("شركة بن سوما للمطابخ\nKOSS - نظام إدارة العمليات", headerFont))
                {
                    BackgroundColor = PrimaryColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 12,
                    Border = Rectangle.NO_BORDER,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };
                headerTable.AddCell(headerCell);
                doc.Add(headerTable);

                doc.Add(new Paragraph(" "));

                // عنوان الإيصال
                var title = new Paragraph("إيصال استلام دفعة مالية", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10
                };
                doc.Add(title);

                // رقم الإيصال والتاريخ
                var infoTable = new PdfPTable(2) { WidthPercentage = 100 };
                infoTable.SetWidths(new float[] { 1f, 1f });
                AddInfoCell(infoTable, "رقم الإيصال:", payment.ReceiptNumber, boldFont, normalFont);
                AddInfoCell(infoTable, "التاريخ:", payment.PaidAt.ToString("yyyy/MM/dd"), boldFont, normalFont);
                AddInfoCell(infoTable, "اسم العميل:", client.Name, boldFont, normalFont);
                AddInfoCell(infoTable, "رقم العقد:", contract.ContractNumber, boldFont, normalFont);
                AddInfoCell(infoTable, "نوع الدفعة:", payment.PaymentType.ToString(), boldFont, normalFont);
                AddInfoCell(infoTable, "طريقة الدفع:", payment.PaymentMethod.ToString(), boldFont, normalFont);
                doc.Add(infoTable);

                doc.Add(new Paragraph(" "));

                // المبلغ
                var amountTable = new PdfPTable(1) { WidthPercentage = 100 };
                var amountCell = new PdfPCell(new Phrase($"المبلغ المُستلم: {payment.Amount:N3} دينار ليبي", titleFont))
                {
                    BackgroundColor = LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 15,
                    BorderColor = AccentColor,
                    BorderWidth = 2,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };
                amountTable.AddCell(amountCell);
                doc.Add(amountTable);

                doc.Add(new Paragraph(" "));

                // ملاحظات
                if (!string.IsNullOrEmpty(payment.Notes))
                {
                    doc.Add(new Paragraph($"ملاحظات: {payment.Notes}", normalFont));
                    doc.Add(new Paragraph(" "));
                }

                // توقيع المُستلم
                var sigTable = new PdfPTable(2) { WidthPercentage = 100 };
                sigTable.SetWidths(new float[] { 1f, 1f });
                var sigCell1 = new PdfPCell(new Phrase($"استُلم بواسطة:\n{payment.ReceivedBy}\n\n______________________", normalFont))
                { Border = Rectangle.NO_BORDER, Padding = 5, RunDirection = PdfWriter.RUN_DIRECTION_RTL };
                var sigCell2 = new PdfPCell(new Phrase("توقيع العميل:\n\n\n______________________", normalFont))
                { Border = Rectangle.NO_BORDER, Padding = 5, HorizontalAlignment = Element.ALIGN_LEFT };
                sigTable.AddCell(sigCell1);
                sigTable.AddCell(sigCell2);
                doc.Add(sigTable);

                // تذييل
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(
                    "هذا الإيصال صادر من نظام KOSS ويُعدّ وثيقة رسمية معتمدة من الشركة.",
                    smallFont) { Alignment = Element.ALIGN_CENTER });

                doc.Close();
                return ms.ToArray();
            }
        }

        // ============================================================
        //  مساعد إضافة خلية معلومات
        // ============================================================
        private static void AddInfoCell(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            table.AddCell(new PdfPCell(new Phrase(label, labelFont))
            {
                Border = Rectangle.BOTTOM_BORDER, BorderColor = LightGray, Padding = 5,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL, BackgroundColor = BaseColor.WHITE
            });
            table.AddCell(new PdfPCell(new Phrase(value ?? "-", valueFont))
            {
                Border = Rectangle.BOTTOM_BORDER, BorderColor = LightGray, Padding = 5,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL, BackgroundColor = BaseColor.WHITE
            });
        }

        /// <summary>
        /// ينشئ رقم إيصال تسلسلي فريد بصيغة: KOSS-YYYY-XXXXXX
        /// </summary>
        public static string GenerateReceiptNumber(int lastId)
        {
            return $"KOSS-{DateTime.Now.Year}-{(lastId + 1):D6}";
        }
    }
}
