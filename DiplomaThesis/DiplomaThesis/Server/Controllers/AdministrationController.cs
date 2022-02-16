using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaThesis.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AdministrationController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddRole()
        {
            return Ok();
        }
    }
}