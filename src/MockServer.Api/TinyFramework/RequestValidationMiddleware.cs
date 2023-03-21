using System.Dynamic;
using System.Net;
using System.Text.Json;
using MockServer.Api.TinyFramework.DataBinding;
using MockServer.Core.Extentions;
using MockServer.Core.Repositories;
using CoreRoute = MockServer.Core.WebApplications.Route;
namespace MockServer.Api.TinyFramework;

public class RequestValidationMiddleware : IMiddleware
{
    private readonly IWebApplicationRouteRepository _requestRepository;
    private readonly ConstraintBuilder _constraintBuilder;
    private readonly FromQueryDataBinder _fromQueryDataBinder;
    private readonly FromHeaderDataBinder _fromHeaderDataBinder;
    private readonly FromBodyDataBinder _fromBodyDataBinder;

    public RequestValidationMiddleware(IWebApplicationRouteRepository requestRepository,
            ConstraintBuilder constraintBuilder,
            FromQueryDataBinder fromQueryDataBinder,
            FromHeaderDataBinder fromHeaderDataBinder,
            FromBodyDataBinder fromBodyDataBinder
            )
    {
        _requestRepository = requestRepository;
        _constraintBuilder = constraintBuilder;
        _fromQueryDataBinder = fromQueryDataBinder;
        _fromHeaderDataBinder = fromHeaderDataBinder;
        _fromBodyDataBinder = fromBodyDataBinder;
    }
    
    public async Task<MiddlewareInvokeResult> InvokeAsync(HttpContext context)
    {
        var route = context.Items[typeof(CoreRoute).Name] as CoreRoute;
        var queries = route.RequestQueryValidationItems;
        foreach (var query in queries.OrEmptyIfNull())
        {
            _fromQueryDataBinder.Query = query.Key;
            var value = _fromQueryDataBinder.Get(context);
            if (query.Attributes?.Count > 0)
            {
                List<IConstraint> constraints = BuildConstraints(query.Attributes.Select(attr => string.IsNullOrEmpty(attr.Parameter)? attr.Name : $"{attr.Name}({attr.Parameter})").ToList());
                foreach (var constraint in constraints)
                {
                    if (!constraint.Match(value))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The '{query.Key}' parameter is not valid.");
                        return MiddlewareInvokeResult.End;
                    }
                }
            }
        }
        var headers = route.RequestHeaderValidationItems;
        foreach (var header in headers.OrEmptyIfNull())
        {
            _fromHeaderDataBinder.Name = header.Name;
            var value = _fromHeaderDataBinder.Get(context);
            if (header.Attributes?.Count > 0)
            {
                var constraints = BuildConstraints(header.Attributes.Select(attr => string.IsNullOrEmpty(attr.Parameter) ? attr.Name : $"{attr.Name}({attr.Parameter})").ToList());
                foreach (var constraint in constraints)
                {
                    if (!constraint.Match(value))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The '{header.Name}' header is not valid.");
                        return MiddlewareInvokeResult.End;
                    }
                }
            }
        }
        var body = route.RequestBodyValidationItems;
        var text = _fromBodyDataBinder.Get(context) as string;
        dynamic data = JsonSerializer.Deserialize<ExpandoObject>(text);
        foreach (var field in body.OrEmptyIfNull())
        {
            // Split the field name into its path components
            string[] path = field.Field.Split('.');
            // Traverse the path to the nested field
            dynamic fieldData = data;
            foreach (string fieldName in path)
            {
                if (!ExpandoObjectHelper.PropertyExist(fieldData, fieldName))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync($"The body is not valid.");
                    return MiddlewareInvokeResult.End;
                }
                if (fieldData is ExpandoObject obj)
                {
                    ((IDictionary<string, object>)obj).TryGetValue(fieldName, out fieldData);
                }
                //fieldData = fieldData[fieldName];
            }

            List<IConstraint> constraints = BuildConstraints(field.Attributes.Select(attr => string.IsNullOrEmpty(attr.Parameter) ? attr.Name : $"{attr.Name}({attr.Parameter})").ToList());
            foreach (var constraint in constraints)
            {
                if (!constraint.Match(fieldData))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync($"The body is not valid.");
                    return MiddlewareInvokeResult.End;
                }
            }
        }
        return MiddlewareInvokeResult.Next;
    }

    private List<IConstraint> BuildConstraints(List<string> constraints)
    {
        var _constraints = new List<IConstraint>();
        foreach (var constraintText in constraints)
        {
            _constraints.Add(_constraintBuilder.ResolveConstraint(constraintText));
        }
        return _constraints;
    }
}
