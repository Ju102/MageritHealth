using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MageritHealth.Filters
{
    public class AuthorizeUsersAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                // Capture the intended destination before redirecting to Login
                string controller = context.RouteData.Values["controller"]?.ToString();
                string action = context.RouteData.Values["action"]?.ToString();
                var id = context.RouteData.Values["id"];

                ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
                var tempData = provider.LoadTempData(context.HttpContext);

                if (controller != null && action != null)
                {
                    tempData["controller"] = controller;
                    tempData["action"] = action;
                }
                
                if (id != null)
                {
                    tempData["id"] = id.ToString();
                }

                provider.SaveTempData(context.HttpContext, tempData);
                
                context.Result = GetRoute("Account", "Login");
            }
            
            // If the user is authenticated, we leave context.Result null
            // so the request pipeline can continue executing the requested action.
        }

        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            RouteValueDictionary route = new RouteValueDictionary(new
            {
                controller = controller,
                action = action
            });

            return new RedirectToRouteResult(route);
        }
    }
}
