﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace MycroCloud.WebMvc.Authentication;

public class DevAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Header { get; set; }
    public Dictionary<string, List<Claim>> KeyClaims { get; set; }
}