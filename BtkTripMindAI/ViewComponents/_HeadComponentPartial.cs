using Microsoft.AspNetCore.Mvc;

namespace BtkTripMindAI.ViewComponents
{
    public class _HeadComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
