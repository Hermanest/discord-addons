using Discord.Addons.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Discord.Addons.Data;

[PublicAPI]
public class DataProvider : IDataProvider {
    public class Cache {
        public Cache(string? path) {
            this.path = path;
        }

        public readonly string? path;
    }

    public DataProvider(Cache cache, ILogger<DataProvider>? logger = null) : this(logger) {
        this.path = cache.path;
    }

    public DataProvider(ILogger<DataProvider>? logger = null) {
        _logger = logger;
    }

    public string? path;

    private readonly ILogger? _logger;

    private string GeneratePath(string name) {
        return PlatformTools.CombinePath(path!, name + ".json");
    }

    private void CreateDirectoryIfNeeded(string fileDestination) {
        var dir = PlatformTools.GetDirectoryName(fileDestination);
        if (Directory.Exists(dir)) return;
        _logger?.LogTrace("Attempting to create directory: " + dir);
        Directory.CreateDirectory(dir!);
    }

    public async Task Save(object data, string key) {
        key = GeneratePath(key);
        CreateDirectoryIfNeeded(key);
        _logger?.LogTrace("Attempting to serialize: " + key);
        var ser = JsonConvert.SerializeObject(data);
        _logger?.LogTrace("Completed. Attempting to write...");
        await File.WriteAllTextAsync(key, ser);
    }

    public async Task<object?> Read(string key, Type type) {
        key = GeneratePath(key);
        _logger?.LogTrace("Attempting to read: " + key);
        if (!File.Exists(key)) {
            _logger?.LogTrace("Read failed. File does not exists!");
            return null;
        }
        var text = await File.ReadAllTextAsync(key);
        _logger?.LogTrace("Read completed, content size: " + text.Length);
        return JsonConvert.DeserializeObject(text, type);
    }
}