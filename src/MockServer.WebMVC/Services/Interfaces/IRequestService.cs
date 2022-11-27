using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockServer.WebMVC.Models.Request;

namespace MockServer.WebMVC.Services.Interfaces;

public interface IRequestService
{
    Task Create(string projectName, CreateRequestViewModel request);
}
