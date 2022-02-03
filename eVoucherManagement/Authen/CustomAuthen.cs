using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Authen
{
    public class AuthorizationHeaderRequirement : IAuthorizationRequirement
    {
    }
    public class AuthorizationHeaderHandler : AuthorizationHandler<AuthorizationHeaderRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationHeaderRequirement requirement)
        {
            var userid = "";
            var country_code = "";
            var ph_no = "";
            var user_role = "";

            if (_httpContextAccessor != null)
            {
                if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    var Control = _httpContextAccessor.HttpContext.Request.Path;
                    string ControllerName = Control.HasValue ? Control.Value : "";
                    foreach (var item in context.User.Claims)
                    {
                        if (item.Type == "userid")
                        {
                            userid = item.Value;
                        }
                        else if (item.Type == "country_code")
                        {
                            country_code = item.Value;
                        }
                        else if (item.Type == "ph_no")
                        {
                            ph_no = item.Value;
                        }
                        else if (item.Type == "user_role")
                        {
                            user_role = item.Value;
                        }
                    }
                    bool FlagValidate = false;
                    switch (user_role)
                    {
                        case "Shop": //--- Shop
                            FlagValidate = ShopAuthen.ValidateToken(ControllerName);
                            break;
                        case "User": //--- User
                            FlagValidate = UserAuthen.ValidateToken(ControllerName);
                            break;
                    }
                    if (FlagValidate)
                    {
                        context.Succeed(requirement);
                    }
                    return;
                }
            }
            return;
        }
    }
}
