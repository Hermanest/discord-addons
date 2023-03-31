namespace Discord.Addons.Data;

public enum ObjectLocation {
    Guild,
    User,
    Bot
}
public interface IDiscordConfigurationHelper {
    string GuildConfigKey { get; set; }
    string UserConfigKey { get; set; }
    string BotConfigKey { get; set; }

    IMemoryPool MemoryPool { get; }

    Task<object?> GetObject(string? key, Type type, ObjectLocation location);
    string GenerateLocationKey(string? key, ObjectLocation location);
}