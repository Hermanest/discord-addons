using Discord.Addons.Data;
using JetBrains.Annotations;

namespace Discord.Addons.Utils;

[PublicAPI]
public static class DiscordConfigurationExtensions {
    #region Generics

    public static async Task<T?> GetContextObjectAsync<T>(this IDiscordConfigurationHelper helper, IInteractionContext context) {
        return (T?)await GetContextObject(helper, context, typeof(T));
    }

    public static async Task<T?> GetGuildObject<T>(this IDiscordConfigurationHelper helper, ulong guildId) {
        return (T?)await GetGuildObject(helper, typeof(T), guildId);
    }

    public static async Task<T?> GetBotObject<T>(this IDiscordConfigurationHelper helper) {
        return (T?)await GetBotObject(helper, typeof(T));
    }

    public static async Task<bool> AllocateObject<T>(this IDiscordConfigurationHelper helper, 
        string? key = null, ObjectLocation location = ObjectLocation.Guild) {
        return await helper.MemoryPool.AllocateObject(typeof(T), helper.GenerateLocationKey(key, location));
    }

    #endregion

    public static async Task<object?> GetContextObject(this IDiscordConfigurationHelper helper, IInteractionContext context, Type type) {
        return await GetGuildObject(helper, type, context.Guild.Id);
    }

    public static async Task<object?> GetGuildObject(this IDiscordConfigurationHelper helper, Type type, ulong guildId) {
        return await helper.GetObject(guildId.ToString(), type, ObjectLocation.Guild);
    }

    public static async Task<object?> GetBotObject(this IDiscordConfigurationHelper helper, Type type) {
        return await helper.GetObject(null, type, ObjectLocation.Bot);
    }

    public static async Task<bool> AllocateObject(this IDiscordConfigurationHelper helper, 
        Type type, string? key = null, ObjectLocation location = ObjectLocation.Guild) {
        return await helper.MemoryPool.AllocateObject(type, helper.GenerateLocationKey(key, location));
    }
}
