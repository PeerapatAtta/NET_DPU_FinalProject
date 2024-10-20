using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Helpers;

// Class for adding authorization to swagger documentation
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    // Apply method to add authorization to swagger documentation
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the method or controller has authorize attribute
        var hasAuthorize =
                context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() || // Controller has authorize attribute
                context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any(); // Method has authorize attribute
       
        // If it has authorize attribute, add 401 response and security requirement
        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" }); // Add 401 response to swagger documentation
            
            // Add security requirement to swagger documentation
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                // Define security scheme and requirement
                new OpenApiSecurityRequirement
                {
                    {
                        // Define security scheme
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>() // Define requirement
                    }
                }
            };
        }
    }
}
