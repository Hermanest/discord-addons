using Discord.Addons.Data;
using JetBrains.Annotations;

namespace Discord.Addons.Utils;

[PublicAPI]
public static class MemoryPoolExtensions {
    public static async Task<bool> AllocateObject(this IMemoryPool provider, Type type, string? key = null) {
        var instance = Activator.CreateInstance(type);
        if (instance == null) return false;
        await provider.AddObject(key, instance, true);
        return true;
    }
}
