using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class PermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionAuthorizeAttribute(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.HasClaim(c => c.Type == "Permission" && c.Value == _permission))
        {
            context.Result = new ForbidResult();
        }
    }
}