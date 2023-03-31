using Discord.Interactions;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Discord.Addons;

[PublicAPI]
public class InteractionHandleService : IActivatable, IDisposable {
    public InteractionHandleService(
        ListedCache<Type, InteractionHandleService> cache,
        DiscordSocketClient client,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        ILogger<InteractionHandleService>? logger = null) {
        if (cache.Cache == null)
            throw new ArgumentNullException(nameof(cache));
        DiscordSocketClient = client;
        InteractionService = interactionService;
        this.cache = cache.Cache.ToArray();
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public DiscordSocketClient DiscordSocketClient { get; }
    public InteractionService InteractionService { get; }
    public IReadOnlyList<Type> Modules => cache;

    public event Func<SocketInteraction, Task<bool>> InteractionCreatedEvent {
        add => interactionCreatedSubs.Add(value);
        remove => interactionCreatedSubs.Remove(value);
    }

    protected readonly List<Func<SocketInteraction, Task<bool>>> interactionCreatedSubs = new();
    protected readonly Type[] cache;
    protected readonly IServiceProvider serviceProvider;
    protected readonly ILogger? logger;

    public void Activate() {
        DiscordSocketClient.Ready += HandleClientReady;
        DiscordSocketClient.InteractionCreated += HandleInteractionCreated;
    }

    public void Dispose() {
        DiscordSocketClient.Ready -= HandleClientReady;
        DiscordSocketClient.InteractionCreated -= HandleInteractionCreated;
    }

    protected virtual async Task InstallModulesAsync() {
        logger?.LogDebug("Initiated modules installation");
        foreach (var type in cache) {
            try {
                await InteractionService.AddModuleAsync(type, serviceProvider);
                logger?.LogTrace("Installed " + type);
            } catch (Exception ex) {
                logger?.LogError("Failed to install module:\r\n" + ex);
            }
        }
        await InteractionService.RegisterCommandsGloballyAsync();
        logger?.LogDebug("Installation completed");
    }

    protected async Task HandleClientReady() {
        await InstallModulesAsync();
    }

    protected virtual async Task HandleInteractionCreated(SocketInteraction interaction) {
        foreach (var item in interactionCreatedSubs) {
            if (await item(interaction)) return;
        }
        await InteractionService.ExecuteCommandAsync(
            new SocketInteractionContext(DiscordSocketClient, interaction), serviceProvider);
    }
}