using System.Net;
using MockServer.Api.TinyFramework.DataBinding;
using MockServer.Core.Repositories;

namespace MockServer.Api.TinyFramework;

public class ConstraintValidationMiddleware : IMiddleware
{
    private readonly IRequestRepository _requestRepository;
    private readonly ConstraintBuilder _constraintBuilder;

    public ConstraintValidationMiddleware(IRequestRepository requestRepository,
            ConstraintBuilder constraintBuilder)
    {
        _requestRepository = requestRepository;
        _constraintBuilder = constraintBuilder;
    }
    
    public async Task<MiddlewareInvokeResult> InvokeAsync(Request request)
    {
        var context = request.HttpContext;
        var queries = await _requestRepository.GetRequestQueries(request.Id);
        var queryBinder = new FromQueryDataBinder();
        foreach (var param in queries)
        {
            queryBinder.Query = param.Key;
            var value = queryBinder.Get(context);
            if (!string.IsNullOrEmpty(param.Constraints))
            {
                var constraintsList = param.Constraints.Split(":").ToList();
                List<IConstraint> constraints = BuildConstraints(constraintsList);
                bool matchexactly = constraintsList.Contains("matchexactly");
                if (matchexactly)
                {
                    constraints.Add(MatchExactlyConstraint.Create(param.Value));
                }
                foreach (var constraint in constraints)
                {
                    if (!constraint.Match(value, out string message))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The '{param.Key}' parameter is not valid.");
                        await context.Response.WriteAsync(message);
                        return MiddlewareInvokeResult.End;
                    }
                }
            }
        }
        var headers = await _requestRepository.GetRequestHeaders(request.Id);
        var headerBinder = new FromHeaderDataBinder();
        foreach (var header in headers)
        {
            headerBinder.Name = header.Name;
            var value = headerBinder.Get(context);
            if (!string.IsNullOrEmpty(header.Constraints))
            {
                var constraintsList = header.Constraints.Split(":").ToList();
                bool matchexactly = constraintsList.Contains("matchexactly");
                List<IConstraint> constraints = BuildConstraints(constraintsList);
                if (matchexactly)
                {
                    constraints.Add(MatchExactlyConstraint.Create(header.Value));
                }
                foreach (var constraint in constraints)
                {
                    if (!constraint.Match(value, out string message))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The '{header.Name}' header is not valid.");
                        await context.Response.WriteAsync(message);
                        return MiddlewareInvokeResult.End;
                    }
                }
            }
        }
        var body = await _requestRepository.GetRequestBody((int)request.Id);
        var bodyBinder = new FromBodyDataBinder();
        var text = bodyBinder.Get(context);
        if (!string.IsNullOrEmpty(body.Constraints))
        {
            var constraintsList = body.Constraints.Split(":").ToList();
            bool matchexactly = constraintsList.Contains("matchexactly");
            List<IConstraint> constraints = BuildConstraints(constraintsList);
            if (matchexactly)
            {
                constraints.Add(MatchExactlyConstraint.Create(body.Text));
            }
            foreach (var constraint in constraints)
            {
                if (!constraint.Match(text, out string message))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync($"The body is not valid.");
                    await context.Response.WriteAsync(message);
                    return MiddlewareInvokeResult.End;
                }
            }
        }

        context.Items[nameof(Request)] = new Request()
        {
            Id = request.Id,
            Type = request.Type
        };
        return MiddlewareInvokeResult.Next;
    }

    private List<IConstraint> BuildConstraints(List<string> constraints)
    {
        foreach (var constraintText in constraints)
        {
            _constraintBuilder.AddResolvedConstraint(constraintText);
        }
        return _constraintBuilder.Build();
    }
}
