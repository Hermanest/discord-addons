using System.Timers;
using Discord.Addons.Data;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Discord.Addons;

[PublicAPI]
public class PoolCleanerService : IActivatable, IDisposable {
    public PoolCleanerService(
        INotifiableMemoryPool memoryPool,
        ILogger<PoolCleanerService>? logger = null) {
        _memoryPool = memoryPool;
        _logger = logger;
    }

    public double CleanIntervalMillis {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public double initIntervalMillis = TimeSpan.FromHours(1).TotalMilliseconds;
    public TimeSpan usageTimeout = TimeSpan.FromMinutes(30);

    private readonly Timer _timer = new();
    private readonly Dictionary<string, DateTime> _timedItems = new();

    private readonly INotifiableMemoryPool _memoryPool;
    private readonly ILogger? _logger;

    public void Activate() {
        _memoryPool.ItemGrabbedEvent += HandleItemGrabbed;
        _memoryPool.ItemReleasedEvent += HandleItemReleased;
        _timer.Elapsed += HandleTimeElapsed;
        _timer.Interval = initIntervalMillis;
        _timer.AutoReset = true;
        _timer.Start();
    }

    public void Dispose() {
        _memoryPool.ItemGrabbedEvent -= HandleItemGrabbed;
        _memoryPool.ItemReleasedEvent -= HandleItemReleased;
        _timer.Dispose();
    }

    public void Clean() {
        _logger?.LogDebug("Initiated pool cleaning");
        var timeNow = DateTime.Now;
        var count = _memoryPool.Release((x, y) => _timedItems
            .TryGetValue(x, out var time) && timeNow.Subtract(time) >= usageTimeout).Result;
        _logger?.LogInformation("Cleaning finished. Cleaned " + count + " items");
    }

    private Task HandleItemGrabbed(string key, object item) {
        _timedItems[key] = DateTime.Now;
        return Task.CompletedTask;
    }

    private Task HandleItemReleased(string key, object item) {
        _timedItems.Remove(key);
        return Task.CompletedTask;
    }

    private void HandleTimeElapsed(object? sender, ElapsedEventArgs e) {
        Clean();
    }
}