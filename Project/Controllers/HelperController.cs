using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class HelperController : Controller
    {
        public IActionResult document([FromQueryAttribute(Name="id")] String helpId)
        {
            return Content(helpId.ToString());
        }
    }
}
