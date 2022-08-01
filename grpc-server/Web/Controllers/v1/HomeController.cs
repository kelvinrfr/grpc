using Microsoft.AspNetCore.Mvc;

namespace grpc_server.Web.Controllers.v1
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return new JsonResult("working");
        }
    }
}
