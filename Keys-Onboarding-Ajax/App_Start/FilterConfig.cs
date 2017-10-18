using System.Web;
using System.Web.Mvc;

namespace Keys_Onboarding_Ajax
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
