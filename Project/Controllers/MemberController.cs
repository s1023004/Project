using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult register(String userName,String password,String realName,String email)
        {
            return View();
        }
    }
}
