using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WebApplication.Domain.Interfaces;

namespace WebApplication.Domain.Services;

public class ApiKeyService : IApiKeyService
{
    private const int _lengthOfKey = 32;
    public string GenerateApiKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(_lengthOfKey);
        return Convert.ToBase64String(bytes);
    }
}
