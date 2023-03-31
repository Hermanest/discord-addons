using Microsoft.Extensions.Logging;

namespace Discord.Addons.Data;

public class MemoryPool : INotifiableMemoryPool, IDisposable {
    public MemoryPool(
        IDataProvider dataProvider,
        ILogger<MemoryPool>? logger = null) {
        _data = dataProvider;
        _logger = logger;
    }

    public event Func<string, object, Task> ItemAddedEvent {
        add => _itemAddedSubs.Add(value);
        remove => _itemAddedSubs.Remove(value);
    }

    public event Func<string, object, Task> ItemGrabbedEvent {
        add => _itemGrabbedSubs.Add(value);
        remove => _itemGrabbedSubs.Remove(value);
    }

    public event Func<string, object, Task> ItemReleasedEvent {
        add => _itemReleasedSubs.Add(value);
        remove => _itemReleasedSubs.Remove(value);
    }

    private readonly List<Func<string, object, Task>> _itemAddedSubs = new();
    private readonly List<Func<string, object, Task>> _itemGrabbedSubs = new();
    private readonly List<Func<string, object, Task>> _itemReleasedSubs = new();

    private readonly Dictionary<string, object> _items = new();
    private readonly IDataProvider _data;
    private readonly ILogger? _logger;

    public async Task<int> Release(Func<string, object, bool>? selector = null) {
        var count = 0;
        foreach (var item in _items.ToArray()) {
            if (!(selector?.Invoke(item.Key, item.Value) ?? true)) continue;
            await _data.Save(item.Value, item.Key);
            foreach (var item2 in _itemReleasedSubs) 
                await item2(item.Key, item.Value);
            _items.Remove(item.Key);
            count++;
        }
        return count;
    }

    public async Task AddObject(string? key, object obj, bool saveImmediate) {
        if (obj is null) throw new ArgumentNullException(nameof(obj));
        _items[key = FormatKey(key, obj.GetType().Name)] = obj;
        if (saveImmediate) await _data.Save(obj, key);
        foreach (var item in _itemAddedSubs) await item(key, obj);
    }

    public async Task<object?> GetObject(string? key, Type type, bool defOnFail) {
        key = FormatKey(key, type.Name);
        if (!_items.TryGetValue(key, out var obj)) {
            _logger?.LogDebug("Initiated cache loading");
            obj = await _data.Read(key, type);
            if (obj is null && defOnFail) {
                try {
                    obj = Activator.CreateInstance(type);
                } catch { }
            }
        }
        if (obj is null) return obj;
        _items[key] = obj;
        foreach (var item in _itemGrabbedSubs) await item(key, obj);
        return obj;
    }

    public void Dispose() {
        _items.Clear();
        _itemAddedSubs.Clear();
        _itemGrabbedSubs.Clear();
        _itemReleasedSubs.Clear();
    }
    
    private static string FormatKey(string? key, string type) {
        return Path.Combine(key!, type);
    }
}