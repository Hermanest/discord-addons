using Discord.Addons.Utils;
using Discord.Interactions;
using JetBrains.Annotations;

namespace Discord.Addons;

[PublicAPI]
public abstract class LoggableInteractionModuleBase : LoggableInteractionModuleBase<IInteractionContext> { }

[PublicAPI]
public abstract class LoggableInteractionModuleBase<T> : InteractionModuleBase<T> where T : class, IInteractionContext {
    protected virtual async Task RespondWithErrorAsync(Exception exception, bool ephemeral = false, IInteractionContext? context = null) {
        await RespondWithEmbedAsync(DiscordUtils.BuildErrorEmbed(exception), ephemeral, context);
    }
    
    protected virtual async Task RespondWithErrorAsync(string error,
        bool internalError = false, bool ephemeral = false, IInteractionContext? context = null) {
        await RespondWithEmbedAsync(DiscordUtils.BuildErrorEmbed(error, internalError), ephemeral, context);
    }

    protected virtual async Task ModifyWithErrorAsync(string error, bool internalError = false) {
        await ModifyWithEmbedAsync(DiscordUtils.BuildErrorEmbed(error, internalError));
    }
    
    protected virtual async Task ModifyWithErrorAsync(Exception exception) {
        await ModifyWithEmbedAsync(DiscordUtils.BuildErrorEmbed(exception));
    }

    private async Task RespondWithEmbedAsync(Embed embed, bool ephemeral = false, IInteractionContext? context = null) {
        await (context ?? Context).Interaction.RespondAsync(embed: embed, ephemeral: ephemeral);
    }
    
    private async Task ModifyWithEmbedAsync(Embed embed) {
        await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
    }
}
