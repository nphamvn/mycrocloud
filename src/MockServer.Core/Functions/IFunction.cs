using Microsoft.AspNetCore.Http;

namespace MockServer.Core.Functions;

public interface IFunction
{
    Task Handle(HttpContext context);
}
