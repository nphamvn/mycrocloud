using System.Dynamic;
using System.Net;
using System.Text.Json;
using MockServer.Api.TinyFramework.DataBinding;
using MockServer.Core.Extentions;
using MockServer.Core.Repositories;
using CoreRoute = MockServer.Core.WebApplications.Route;
namespace MockServer.Api.TinyFramework;

public class ConstraintValidationMiddleware : IMiddleware
{
    private readonly IWebApplicationRouteRepository _requestRepository;
    private readonly ConstraintBuilder _constraintBuilder;
    private readonly FromQueryDataBinder _fromQueryDataBinder;
    private readonly FromHeaderDataBinder _fromHeaderDataBinder;
    private readonly FromBodyDataBinder _fromBodyDataBinder;

    public ConstraintValidationMiddleware(IWebApplicationRouteRepository requestRepository,
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
        var queries = route.RequestQueries;
        foreach (var query in queries.OrEmptyIfNull())
        {
            _fromQueryDataBinder.Query = query.Key;
            var value = _fromQueryDataBinder.Get(context);
            if (query.Constraints?.Count > 0)
            {
                List<IConstraint> constraints = BuildConstraints(query.Constraints);
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
        var headers = route.RequestHeaders;
        foreach (var header in headers)
        {
            _fromHeaderDataBinder.Name = header.Name;
            var value = _fromHeaderDataBinder.Get(context);
            if (header.Constraints?.Count > 0)
            {
                var constraints = BuildConstraints(header.Constraints);
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
        var body = route.RequestBody;
        var text = _fromBodyDataBinder.Get(context) as string;
        if (body?.Constraints?.Count > 0)
        {
            var constraints = BuildConstraints(route.RequestBody.Constraints);
            foreach (var constraint in constraints)
            {
                if (!constraint.Match(text))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync($"The body is not valid.");
                    return MiddlewareInvokeResult.End;
                }
            }
        }
        if (body?.FieldConstraints?.Count > 0)
        {
            dynamic data = JsonSerializer.Deserialize<ExpandoObject>(text);
            foreach (var field in body.FieldConstraints)
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

                List<IConstraint> constraints = BuildConstraints(field.Constraints);
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
