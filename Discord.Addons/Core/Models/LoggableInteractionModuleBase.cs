using Discord.Addons.Utils;
using Discord.Interactions;
using JetBrains.Annotations;

namespace Discord.Addons;

[PublicAPI]
public abstract class LoggableInteractionModuleBase : LoggableInteractionModuleBase<IInteractionContext> {
}

[PublicAPI]
public abstract class LoggableInteractionModuleBase<T> : InteractionModuleBase<T> where T : class, IInteractionContext {
    protected virtual async Task RespondWithErrorAsync(Exception exception, bool ephemeral = false, IDiscordInteraction? interaction = null) {
        await RespondWithEmbedAsync(DiscordUtils.BuildErrorEmbed(exception), ephemeral, interaction);
    }

    protected virtual async Task RespondWithErrorAsync(
        string error,
        bool internalError = false,
        bool ephemeral = false,
        IDiscordInteraction? interaction = null) {
        await RespondWithEmbedAsync(DiscordUtils.BuildErrorEmbed(error, internalError), ephemeral, interaction);
    }

    protected virtual async Task ModifyWithErrorAsync(
        string error,
        bool internalError = false,
        IDiscordInteraction? interaction = null) {
        await ModifyWithEmbedAsync(DiscordUtils.BuildErrorEmbed(error, internalError), interaction);
    }

    protected virtual async Task ModifyWithErrorAsync(Exception exception,
        IDiscordInteraction? context = null) {
        await ModifyWithEmbedAsync(DiscordUtils.BuildErrorEmbed(exception), context);
    }

    protected virtual async Task RespondWithEmbedAsync(Embed embed, bool ephemeral = false, IDiscordInteraction? interaction = null) {
        var task = interaction is null ?
            RespondAsync(embed: embed, ephemeral: true) :
            interaction.RespondAsync(embed: embed, ephemeral: ephemeral);
        await task;
    }

    protected virtual async Task ModifyWithEmbedAsync(Embed embed, IDiscordInteraction? interaction = null) {
        var task = interaction is null ?
            ModifyOriginalResponseAsync(x => x.Embed = embed) :
            interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        await task;
    }
}