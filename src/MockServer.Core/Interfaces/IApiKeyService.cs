using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockServer.Core.Interfaces;

public interface IApiKeyService
{
    string GenerateApiKey();
}
