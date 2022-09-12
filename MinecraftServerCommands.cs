using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace MinecraftServerBot;

[SlashCommandGroup("server", "Manage the Minecraft server")]
public class MinecraftServerCommands : ApplicationCommandModule
{
    private readonly ILogger<MinecraftServerCommands> logger;
    private readonly DiscordClient discordClient;
    private readonly MinecraftServer minecraftServer;

    public MinecraftServerCommands(
        ILogger<MinecraftServerCommands> logger,
        DiscordClient discordClient,
        MinecraftServer minecraftServer
    )
    {
        this.logger = logger;
        this.discordClient = discordClient;
        this.minecraftServer = minecraftServer;
    }

    [SlashCommand("start", "Starts the server")]
    public async Task Start(InteractionContext context)
    {
        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new()
        {
            Content = "Starting server ⏳"
        });

        await minecraftServer.StartAsync();

        await context.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Starting server ⏳ \nStarted server ✅")
        );
    }

    

    [SlashCommand("restart", "Restarts the server")]
    public async Task Restart(InteractionContext context)
    {
        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new()
        {
            Content = "Restarting server ⏳"
        });

        await minecraftServer.RestartAsync();

        await context.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Restarting server ⏳ \nRestarted server ✅")
        );
    }

    

    [SlashCommand("stop", "Stops the server")]
    public async Task Stop(InteractionContext context)
    {
        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new()
        {
            Content = "Stopping server ⏳"
        });

        await minecraftServer.StopAsync();

        await context.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Stopping server ⏳ \nStopped server ✅")
        );
    }

    [SlashCommand("status", "Gets the status of the server")]
    public async Task Status(InteractionContext context)
    {
        var status = await minecraftServer.GetStatus();
        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new()
        {
            Content = $"Server status is {status}"
        });
    }
}
