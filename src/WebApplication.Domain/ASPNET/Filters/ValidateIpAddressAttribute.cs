using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Domain.ASPNET.Filters
{
    public class ValidateIpAddressAttribute : TypeFilterAttribute
    {
        public ValidateIpAddressAttribute() : base(typeof(ValidateIpAddressFilter))
        {
        }

        private class ValidateIpAddressFilter : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                ValidateIpAddress(context);
                if (context.Result == null)
                    await next();
            }

            private void ValidateIpAddress(ActionExecutingContext context)
            {
                //get action and controller names
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;
                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;

                //don't validate on the 'Access denied' page
                if (controllerName.Equals("Security", StringComparison.InvariantCultureIgnoreCase) &&
                    actionName.Equals("AccessDenied", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                //get allowed IP addresses
                var ipAddresses = new string[10];

                //there are no restrictions
                if (ipAddresses == null || !ipAddresses.Any())
                    return;

                //whether current IP is allowed
                var currentIp = "_webHelper.GetCurrentIpAddress();";

                if (ipAddresses.Any(ip => ip.Equals(currentIp, StringComparison.InvariantCultureIgnoreCase)))
                    return;

                //redirect to 'Access denied' page
                context.Result = new RedirectToActionResult("AccessDenied", "Security", context.RouteData.Values);
            }
        }
    }
}
