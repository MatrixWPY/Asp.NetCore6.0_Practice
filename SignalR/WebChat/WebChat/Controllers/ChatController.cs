using Microsoft.AspNetCore.Mvc;

namespace WebChat.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
