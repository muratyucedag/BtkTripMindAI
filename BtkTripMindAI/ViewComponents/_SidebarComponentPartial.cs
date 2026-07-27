using Microsoft.AspNetCore.Mvc;

namespace BtkTripMindAI.ViewComponents
{
    public class _SidebarComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
