using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace KOSS.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            System.Data.Entity.Database.SetInitializer<KOSS.Web.Models.KossDbContext>(null);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
