using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
