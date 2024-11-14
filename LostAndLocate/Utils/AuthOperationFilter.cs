using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LostAndLocate.Utils;

/// <summary>
/// Extension class for adding authentication for Swagger
/// </summary>
public sealed class AuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext ctx)
    {
        if (ctx.ApiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return;

        // If not [AllowAnonymous] and [Authorize] on either the endpoint or the controller...
        if (!ctx.ApiDescription.CustomAttributes().Any(a => a is AllowAnonymousAttribute)
            && (ctx.ApiDescription.CustomAttributes().Any(a => a is AuthorizeAttribute)
                || descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() is not null))
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                }] = Array.Empty<string>()
            });
        }
    }
}