using Discord.Addons.Data;
using Discord.Addons.Utils;
using Discord.Interactions;
using Discord.Interactions.Builders;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Discord.Addons.SetupModule.Utils;
using JetBrains.Annotations;

namespace Discord.Addons.SetupModule;

[PublicAPI]
public class SetupCommandModule : LoggableInteractionModuleBase {
    public SetupCommandModule(
        ListedCache<Type, SetupCommandModule> cache,
        InteractionHandleService interactionService,
        IDiscordConfigurationHelper configService,
        IServiceProvider services,
        ILogger<SetupCommandModule>? logger = null) {
        if (cache.Cache is null)
            throw new ArgumentNullException(nameof(cache));
        this.cache = cache.Cache.ToArray();
        interactionHandle = interactionService;
        this.configService = configService;
        this.services = services;
        this.logger = logger;
        interactionHandle.InteractionCreatedEvent += HandleInteractionCreated;
    }

    ~SetupCommandModule() {
        interactionHandle.InteractionCreatedEvent -= HandleInteractionCreated;
    }

    protected readonly Dictionary<string, MethodInfo> pseudoCtorCache = new();
    protected readonly Type[] cache;
    protected readonly InteractionHandleService interactionHandle;
    protected readonly IServiceProvider services;
    protected readonly IDiscordConfigurationHelper configService;
    protected readonly ILogger? logger;
    protected string setupCommandName = null!;

    public sealed override void Construct(ModuleBuilder builder, InteractionService commandService) {
        builder.WithGroupName("setup");
        builder.WithDefaultMemberPermissions(GuildPermission.ManageGuild);
        logger?.LogDebug("Constructing slash commands");
        foreach (var type in cache) {
            if (type.GetCustomAttribute<ComplexObjectAttribute>() is not { } attr) return;
            try {
                logger?.LogTrace($"Building {type}");
                var name = attr.Name;
                if (pseudoCtorCache.ContainsKey(name)) {
                    throw new ArgumentException("An item with the same command name is already exists", nameof(name));
                }
                builder.AddSlashCommand(x => {
                    if (!this.BuildComplexSlashCommand(name, type, x, interactionHandle
                            .InteractionService, services, out var ctor)) {
                        throw new ModuleConstructionException(name);
                    }
                    pseudoCtorCache.Add(name, ctor!);
                    x.WithName(name);
                    HandleSlashCommandBuilt(x, type);
                });
            } catch (Exception ex) {
                logger?.LogError(ex.ToString());
            }
        }
        HandleModuleConstructed(builder);
        setupCommandName = builder.SlashGroupName;
        logger?.LogDebug("Construction completed");
    }

    protected virtual void HandleModuleConstructed(ModuleBuilder builder) {
        builder.WithDescription("Settings group");
    }

    protected virtual void HandleSlashCommandBuilt(Interactions.Builders.SlashCommandBuilder builder, Type type) {
        builder.WithDescription($"Allows to modify {type.Name}'s settings");
    }

    protected virtual async Task<bool> HandleInteractionCreated(SocketInteraction interaction) {
        if (interaction is not SocketSlashCommand command
            || command.CommandName != setupCommandName) return false;

        await interaction.DeferAsync(true);
        var subCommand = command.Data.Options.FirstOrDefault();
        if (!pseudoCtorCache.TryGetValue(subCommand!.Name, out var mtd)) {
            await ModifyWithErrorAsync("Invalid config specified");
            return true;
        }

        var accessKey = configService.GenerateLocationKey(
            interaction.GuildId!.Value.ToString(), ObjectLocation.Guild);
        var conf = await configService.MemoryPool
            .GetObject(accessKey, mtd.DeclaringType!);
        if (conf == null) {
            await ModifyWithErrorAsync("Failed to read configuration from the pool", true);
            return true;
        }

        var oldSnapshot = ReflectionUtils.CreateSnapshot(conf);
        if (!TryInvokePseudoCtor(conf, mtd, subCommand.Options)) {
            await ModifyWithErrorAsync("Failed to invoke complex constructor", true);
            return true;
        }

        var embed = BuildReportEmbed(mtd.DeclaringType!.Name,
            ReflectionUtils.CreateReport(oldSnapshot,
                ReflectionUtils.CreateSnapshot(conf)),
            out var nothingModified);

        if (!nothingModified)
            await configService.MemoryPool.AddObject(accessKey, conf!);

        await interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        return true;
    }

    protected static bool TryInvokePseudoCtor(object obj, MethodInfo mtd,
        IReadOnlyCollection<SocketSlashCommandDataOption> options) {
        try {
            var mtdParameters = mtd.GetParameters();
            var parameters = new object?[mtdParameters.Length];
            foreach (var item in options) {
                var idx = Array.FindIndex(mtdParameters, x => x.Name == item.Name);
                parameters[idx] = item.Value;
            }
            mtd.Invoke(obj, parameters);
            return true;
        } catch {
            return false;
        }
    }

    protected static Embed BuildReportEmbed(string typeName,
        IEnumerable<ReflectionUtils.PropertyReport> report, out bool nothingModified) {
        var builder = new EmbedBuilder();
        return builder
            .WithTitle($"Modification report ({typeName})")
            .WithColor(Color.Blue)
            .WithFields(report.Select(x => new EmbedFieldBuilder()
                .WithName(x.Name)
                .WithValue($"{x.OldValue} -> {x.NewValue}")))
            .WithDescription((nothingModified = builder.Fields.Count == 0)
                ? "Nothing was modified" : string.Empty)
            .Build();
    }
}