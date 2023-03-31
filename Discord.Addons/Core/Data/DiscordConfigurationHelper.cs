using Microsoft.Extensions.Logging;

namespace Discord.Addons.Data;

public class DiscordConfigurationHelper : IDiscordConfigurationHelper {
    public DiscordConfigurationHelper(
        IMemoryPool provider, 
        ILogger<DiscordConfigurationHelper>? logger = null) {
        MemoryPool = provider;
        _logger = logger;
    }

    public string GuildConfigKey { get; set; } = "GUILDS\\";
    public string UserConfigKey { get; set; } = "USERS\\";
    public string BotConfigKey { get; set; } = "BOT\\";

    public IMemoryPool MemoryPool { get; }

    private readonly ILogger? _logger; 

    public async Task<object?> GetObject(string? key, Type type, ObjectLocation location = ObjectLocation.Guild) {
        return await MemoryPool.GetObject(GenerateLocationKey(key, location), type);
    }

    public string GenerateLocationKey(string? key = null, ObjectLocation location = ObjectLocation.Guild) {
        var prefix = location switch {
            ObjectLocation.Guild => GuildConfigKey,
            ObjectLocation.User => UserConfigKey,
            ObjectLocation.Bot => BotConfigKey,
            _ => string.Empty
        };
        return prefix + key;
    }
}
