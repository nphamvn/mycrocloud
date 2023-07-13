namespace MockServer.Api.TinyFramework.DataBinding;

public interface IDataBinder
{
    object Get(HttpContext context);
}
