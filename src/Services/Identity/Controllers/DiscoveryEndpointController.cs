using Identity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    public class DiscoveryEndpointController : Controller
    {
        // .well-known/openid-configuration
        [HttpGet("~/.well-known/openid-configuration")]
        public JsonResult GetConfiguration()
        {
            var response = new DiscoveryResponse
            {
                issuer = "https://localhost:5024",
                authorization_endpoint = "https://localhost:5024/Home/Authorize",
                token_endpoint = "https://localhost:5024/Home/Token",
                token_endpoint_auth_methods_supported = new string[] { "client_secret_basic", "private_key_jwt" },
                token_endpoint_auth_signing_alg_values_supported = new string[] { "RS256", "ES256" },

                acr_values_supported = new string[] {"urn:mace:incommon:iap:silver", "urn:mace:incommon:iap:bronze"},
                response_types_supported = new string[] { "code", "code id_token", "id_token", "token id_token" },
                subject_types_supported = new string[] { "public", "pairwise" },

                userinfo_encryption_enc_values_supported = new string[] { "A128CBC-HS256", "A128GCM" },
                id_token_signing_alg_values_supported = new string[] { "RS256", "ES256", "HS256" , "SHA256" },
                id_token_encryption_alg_values_supported = new string[] { "RSA1_5", "A128KW" },
                id_token_encryption_enc_values_supported = new string[] { "A128CBC-HS256", "A128GCM" },
                request_object_signing_alg_values_supported = new string[] { "none", "RS256", "ES256" },
                display_values_supported = new string[] { "page", "popup" },
                claim_types_supported = new string[] { "normal", "distributed" },
                jwks_uri = "https://localhost:5024/jwks.json",
                scopes_supported = new string[] { "openid", "profile", "email", "address", "phone", "offline_access" },
                claims_supported = new string[] { "sub", "iss", "auth_time", "acr", "name", "given_name",
                    "family_name", "nickname", "profile", "picture", "website", "email", "email_verified",
                    "locale", "zoneinfo" },
                claims_parameter_supported = true,
                service_documentation = "https://localhost:5024/connect/service_documentation.html",
                ui_locales_supported = new string[] { "en-US", "en-GB", "en-CA", "fr-FR", "fr-CA" },
                introspection_endpoint = "https://localhost:5024/Introspections/TokenIntrospect"
            };

            return Json(response);
        }

        // jwks.json
        [HttpGet("~/jwks.json")]
        public string Jwks()
        {
            //string path = "jwks.json";
            //return File(path, System.Net.Mime.MediaTypeNames.Application.Json);
            return """
                   {
                     "keys": [
                       {
                         "alg": "RSA256",
                         "e": "AQAB",
                         "n": "xGKygpqTutBE3DJBmyomzwF1FHE5HWk8fxZmrireXbkRyxYuHEV4Ss8XB9ePmvloan3A4k3Rc+ZNR6SNZM2Oz0Pioxbb7bmV2/ODsvr8LlWRBnjlDP1N4ypgrkZw7JwGyT28aMUjjwMaiXRJzwysZtRdiG3jHywBboVKkbfmKoE=",
                         "kty": "RSA",
                         "kid": "xGKygpqTutBE3DJBmyomzwF1FHE5HWk8fxZmrireXbkRyxYuHEV4Ss8XB9ePmvloan3A4k3Rc+ZNR6SNZM2Oz0Pioxbb7bmV2/ODsvr8LlWRBnjlDP1N4ypgrkZw7JwGyT28aMUjjwMaiXRJzwysZtRdiG3jHywBboVKkbfmKoE="
                       }
                     ]
                   }
                   """;
        }
    }
}