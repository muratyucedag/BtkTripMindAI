using Microsoft.AspNetCore.Mvc;

namespace BtkTripMindAI.ViewComponents
{
    public class _RouteViewComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
