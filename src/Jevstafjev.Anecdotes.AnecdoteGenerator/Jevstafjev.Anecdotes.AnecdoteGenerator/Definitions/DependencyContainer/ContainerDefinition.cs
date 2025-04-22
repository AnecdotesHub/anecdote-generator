using Jevstafjev.Anecdotes.AnecdoteGenerator.Definitions.Base;
using OpenAI;
using OpenAI.Chat;

namespace Jevstafjev.Anecdotes.AnecdoteGenerator.Definitions.DependencyContainer;

public class ContainerDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(_ =>
        {
            var apiKey = builder.Configuration["OpenAI:ApiKey"];
            return new ChatClient("gpt-3.5-turbo", apiKey);
        });
    }
}
