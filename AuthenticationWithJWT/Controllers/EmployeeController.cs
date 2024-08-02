using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationWithJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    

    public class EmployeeController : ControllerBase
    {
        [HttpGet("GetData")]
        [Authorize]

        public IActionResult GetData()
        {
            int a=10;
            int b=20;
            int c;
            c = a + b;
            string response="The total sum"+" "+ c;
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
