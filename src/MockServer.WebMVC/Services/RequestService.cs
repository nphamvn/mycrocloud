using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.WebMVC.Extentions;
using MockServer.WebMVC.Models.Request;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Services;

public class RequestService : IRequestService
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper mapper;

    public RequestService(IHttpContextAccessor contextAccessor,
    IRequestRepository requestRepository,
    IMapper mapper)
    {
        this.contextAccessor = contextAccessor;
        _requestRepository = requestRepository;
        this.mapper = mapper;
    }
    public async Task Create(string projectName, CreateRequestViewModel request)
    {
        var user = contextAccessor.HttpContext.User.GetLoggedInUser<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));

        var existing = await _requestRepository.FindRequest(user.Id, projectName, request.Method, request.Path);

        if (existing == null)
        {
            var mapped = mapper.Map<Core.Entities.Request>(request);
            await _requestRepository.Create(user.Id, projectName, mapped);
        }
    }
}
