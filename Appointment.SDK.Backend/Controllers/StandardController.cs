
using Microsoft.AspNetCore.Mvc;

namespace Appointment.SDK.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class StandardController : ControllerBase
    {
        [HttpGet]
        public string HelloWorld()
        {
            return $"Hello world! Soy {GetType().Name}";
        }
    }
}
