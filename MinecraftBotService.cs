using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace MinecraftServerBot;

public class MinecraftBotService : BackgroundService
{
    private readonly ILogger<MinecraftBotService> logger;
    private readonly DiscordClient discordClient;
    private readonly MinecraftServer minecraftServer;

    public MinecraftBotService(
        ILogger<MinecraftBotService> logger,
        DiscordClient discordClient,
        MinecraftServer minecraftServer
    )
    {
        this.logger = logger;
        this.discordClient = discordClient;
        this.minecraftServer = minecraftServer;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        discordClient.MessageCreated += OnMessageCreated;
        await discordClient.ConnectAsync();
    }

    public Task OnMessageCreated(DiscordClient discordClient, MessageCreateEventArgs e)
    {
        var message = e.Message;
        var messageContent = message.Content;

        logger.LogInformation("{Author} said {Message}", message.Author.Username, messageContent);

        if (string.IsNullOrEmpty(messageContent) || !messageContent.StartsWith('/'))
            return Task.CompletedTask;

        messageContent = messageContent.ToLower().Trim();

        Task.Run(async () =>
        {
            try
            {
                switch (messageContent)
                {
                    case "/server start":
                        await message.RespondAsync("Starting server");
                        await minecraftServer.StartAsync();
                        await message.RespondAsync("Server started");
                        break;
                    case "/server stop":
                        await message.RespondAsync("Stopping server");
                        await minecraftServer.StopAsync();
                        await message.RespondAsync("Server stopped");
                        break;
                    case "/server restart":
                        await message.RespondAsync("Restarting server");
                        await minecraftServer.RestartAsync();
                        await message.RespondAsync("Server restarted");
                        break;
                    case "/server status":
                        var status = await minecraftServer.GetStatus();
                        await message.RespondAsync($"Server status is {status}");
                        break;
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        });

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        discordClient.MessageCreated -= OnMessageCreated;
        await discordClient.DisconnectAsync();
        discordClient.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.CompletedTask;
}
