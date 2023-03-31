using Discord.Addons.Data;
using Discord.Addons.Utils;
using Discord.Interactions;
using Discord.Interactions.Builders;
using JetBrains.Annotations;

namespace Discord.Addons;

/// <summary>
/// Used to easily access guild configurations 
/// </summary>
/// <typeparam name="TConfig">Configuration model type</typeparam>
[PublicAPI]
public abstract class ConfigurableInteractionModuleBase<TConfig> : ConfigurableInteractionModuleBase<IInteractionContext, TConfig> { }

/// <summary>
/// Used to easily access guild configurations 
/// </summary>
/// <typeparam name="T">Interaction context type</typeparam>
/// <typeparam name="TConfig">Configuration model type</typeparam>
[PublicAPI]
public abstract class ConfigurableInteractionModuleBase<T, TConfig> : LoggableInteractionModuleBase<T> where T : class, IInteractionContext {
    [UsedImplicitly]
    public IDiscordConfigurationHelper ConfigHelper { get; set; } = null!;

    protected TConfig? ContextConfig {
        get => GetContextConfigAsync(Context).Result;
    }

    public override void Construct(ModuleBuilder builder, InteractionService commandService) {
        builder.AddPreconditions(new CustomPreconditionAttribute(async (x, y, z) => {
            var result = await GetPreconditionResult(x, y, z);
            if (result.IsSuccess) return result;
            await RespondWithErrorAsync(result.ErrorReason, x);
            return result;
        }));
    }

    protected virtual async Task<PreconditionResult> GetPreconditionResult(
        IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services) {
        return await GetContextConfigAsync(context) is null ?
            PreconditionResult.FromError("Invalid configuration") : 
            PreconditionResult.FromSuccess();
    }

    protected virtual async Task<TConfig?> GetContextConfigAsync(IInteractionContext? context = null) {
        return await ConfigHelper.GetContextObjectAsync<TConfig>(context ?? Context);
    }
}
