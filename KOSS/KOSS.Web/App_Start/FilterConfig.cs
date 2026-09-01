using System.Web.Mvc;

namespace KOSS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // إجبار تسجيل الدخول على جميع الصفحات
            filters.Add(new AuthorizeAttribute());
        }
    }
}
