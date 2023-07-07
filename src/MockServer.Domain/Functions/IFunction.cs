using Microsoft.AspNetCore.Http;

namespace MockServer.Domain.Functions;

public interface IFunction
{
    Task Handle(HttpContext context);
}
