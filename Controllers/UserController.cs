using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{

    [HttpGet("")]
    public ActionResult Test()
    {
        return Ok(new { success = true, message = "It works lol" });
    }
}