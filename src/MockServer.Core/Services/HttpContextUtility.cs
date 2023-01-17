using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MockServer.Core.Services;
public interface IHttpContextUtility
{
    object Get(string source, HttpContext context);
}
public class HttpContextUtility: IHttpContextUtility {
    public object Get(string source, HttpContext context)
    {
        var builder = new Builder();
        var binder = builder.Build(source);
        return binder.Get(context);
    }
}
public class Builder {
    public IFromContext Build(string name) {
        throw new NotImplementedException(nameof(IFromContext));
    }
}
public interface IFromContext {
    object Get(HttpContext context);
}
public class FromHeader: FromHeaderAttribute, IFromContext {
    public FromHeader(string name)
    {
        Name = name;
    }
    public object Get(HttpContext context)
    {
        var value = context.Request.Headers[Name];
        return value;
    }
}