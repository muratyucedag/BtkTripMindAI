using Microsoft.AspNetCore.Mvc;

namespace BtkTripMindAI.ViewComponents
{
    public class _ChatComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
