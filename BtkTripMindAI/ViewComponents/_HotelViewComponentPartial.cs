using Microsoft.AspNetCore.Mvc;

namespace BtkTripMindAI.ViewComponents
{
    public class _HotelViewComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
