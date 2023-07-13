using Microsoft.AspNetCore.Http;

namespace WebApplication.Domain.Functions;

public interface IFunction
{
    Task Handle(HttpContext context);
}
