using Ardalis.Result;
using Jevstafjev.Anecdotes.AnecdoteGenerator.Definitions.Base;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using System.Text.Json;

namespace Jevstafjev.Anecdotes.AnecdoteGenerator.Endpoints;

public class AnecdoteEndpoints : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapAnecdoteEndpoints();
    }
}

internal static class AnecdoteEndpointsExtensions
{
    public static void MapAnecdoteEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/anecdotes/").WithTags("Anecdote");

        group.MapGet("generate", async ([FromServices] ChatClient client) =>
        {
            var prompt = "Generate a funny short anecdote and a suitable title for it. Return JSON in the format: { \"title\": \"Funny title\", \"content\": \"Actual anecdote\", \"tags\": \"tag1,tag2\" }";

            var completion = await client.CompleteChatAsync(prompt);
            var json = completion.Value.Content.First().Text;

            var generationResult = JsonSerializer.Deserialize<GenerationResult>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (generationResult is null)
            {
                return Result.Error();
            }

            return Result.Success(generationResult);
        })
            .RequireAuthorization(AppData.DefaultPolicyName)
            .RequireAuthorization(x => x.RequireRole(AppData.AdministratorRoleName))
            .Produces(200)
            .ProducesProblem(401)
            .WithOpenApi();
    }
}

internal class GenerationResult
{
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Tags { get; set; } = null!;
}