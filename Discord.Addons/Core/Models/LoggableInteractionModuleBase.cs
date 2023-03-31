using Discord.Interactions;
using JetBrains.Annotations;

namespace Discord.Addons;

[PublicAPI]
public abstract class LoggableInteractionModuleBase : LoggableInteractionModuleBase<IInteractionContext> { }

[PublicAPI]
public abstract class LoggableInteractionModuleBase<T> : InteractionModuleBase<T> where T : class, IInteractionContext {
    protected virtual async Task RespondWithErrorAsync(string error, IInteractionContext? context = null, bool ephemeral = false) {
        context ??= Context;
        await context.Interaction.RespondAsync(embed: BuildErrorEmbed(error), ephemeral: ephemeral);
    }

    protected virtual async Task ModifyWithErrorAsync(string error) {
        await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = BuildErrorEmbed(error));
    }

    private static Embed BuildErrorEmbed(string error) => new EmbedBuilder()
        .WithTitle("❌ Execution error!")
        .WithColor(Color.Red)
        .WithDescription(error)
        .Build();
}
