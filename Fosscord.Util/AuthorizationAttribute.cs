using System.Net;
using System.Security.Claims;
using Fosscord.API.Classes;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fosscord.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class)]  
public class TokenAuthorizationAttribute : Attribute, IAuthorizationFilter  
{
        public void OnAuthorization(AuthorizationFilterContext filterContext)  
        {  
             
            if (filterContext != null)  
            {  
                Microsoft.Extensions.Primitives.StringValues authTokens;  
                filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out authTokens);  
  
                var _token = authTokens.FirstOrDefault();

                if (_token != null)
                {
                    string authToken = _token;
                    var jwtAuthenticationManager = new JwtAuthenticationManager();
                    try
                    {
                        var user = jwtAuthenticationManager.GetUserFromToken(authToken, out ClaimsPrincipal claimsPrincipal);
                        filterContext.HttpContext.User = claimsPrincipal;
                        if (user == null)
                        {
                            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;  
                            filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Unauthorized";
                            filterContext.Result = new JsonResult(new
                                {
                                    message = "401: Unauthorized",
                                    code = 0,
                                }
                            );
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;  
                        filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Unauthorized";
                        filterContext.Result = new JsonResult(new
                            {
                                message = "401: Unauthorized",
                                code = 0,
                            }
                        );
                    }
                }
                else  
                {  
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;  
                    filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Unauthorized";
                    filterContext.Result = new JsonResult(new
                        {
                            message = "401: Unathorized",
                            code = 0,
                        }
                    );
                }  
            }  
        }
}