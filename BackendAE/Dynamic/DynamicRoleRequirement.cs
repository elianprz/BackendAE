// DynamicRoleRequirement.cs
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class DynamicRoleRequirement : IAuthorizationRequirement
{
    public string RoleName { get; }
    public DynamicRoleRequirement(string roleName)
    {
        RoleName = roleName;
    }
}

public class DynamicRoleHandler : AuthorizationHandler<DynamicRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicRoleRequirement requirement)
    {
        // Obtiene los roles requeridos del endpoint
        var endpoint = context.Resource as Microsoft.AspNetCore.Http.Endpoint;
        var authorizeData = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();
        var requiredRoles = authorizeData?.Roles?.Split(',') ?? Array.Empty<string>();

        // Obtiene el rol del usuario del claim
        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (requiredRoles.Contains(userRole))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}