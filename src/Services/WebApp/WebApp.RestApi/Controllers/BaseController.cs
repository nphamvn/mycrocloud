using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.RestApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BaseController : ControllerBase;