using Azure.Identity;
using DSharpPlus;
using MinecraftServerBot;
using Azure.ResourceManager;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        var configuration = builder.Configuration;

        if (builder.HostingEnvironment.IsDevelopment())
        {
            services.AddTransient<ArmClient>(_ => new ArmClient(new ClientSecretCredential(
                tenantId: configuration["Azure:TenantId"],
                clientId: configuration["Azure:ClientId"],
                clientSecret: configuration["Azure:ClientSecret"]
            )));
        }
        else
        {
            services.AddTransient<ArmClient>(_ => new ArmClient(new DefaultAzureCredential()));
        }

        services.AddTransient<MinecraftServer>();

        services.AddSingleton<DiscordClient>(serviceProvider => new DiscordClient(new DiscordConfiguration
        {
            Token = builder.Configuration["DiscordBot:Token"],
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        }));

        services.AddHostedService<MinecraftBotService>();
    })
    .Build();

await host.RunAsync();
