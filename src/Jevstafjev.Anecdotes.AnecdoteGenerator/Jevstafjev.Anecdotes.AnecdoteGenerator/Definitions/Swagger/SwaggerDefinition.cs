using Jevstafjev.Anecdotes.AnecdoteGenerator.Definitions.Base;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Jevstafjev.Anecdotes.AnecdoteGenerator.Definitions.Swagger;

public class SwaggerDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AnecdoteGenerator",
                Version = "1.0.0"
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://localhost:10001/connect/authorize"),
                        TokenUrl = new Uri("https://localhost:10001/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "AnecdoteGenerator", "Default scope" }
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        In = ParameterLocation.Cookie,
                        Type = SecuritySchemeType.OAuth2

                    },
                    new List<string>()
                }
            });
        });
    }

    public override void ConfigureApplication(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "AnecdoteGenerator v1.0.0");
            options.DocumentTitle = "AnecdoteGenerator";
            options.DefaultModelExpandDepth(0);
            options.DefaultModelRendering(ModelRendering.Model);
            options.DefaultModelsExpandDepth(0);
            options.DocExpansion(DocExpansion.None);
            options.OAuthScopeSeparator(" ");
            options.OAuthClientId("generator-swagger-id");
            options.OAuthClientSecret("secret");
            options.OAuthAppName("Anecdotes");
            options.OAuth2RedirectUrl("https://localhost:4001/swagger/oauth2-redirect.html");
            options.EnablePersistAuthorization();
        });
    }
}
