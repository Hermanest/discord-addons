using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discord.Addons;

[PublicAPI]
public class ActivationService : IDisposable {
    public ActivationService(
        ListedCache<Type, ActivationService> cache,
        IServiceProvider services,
        ILogger<ActivationService>? logger = null) {
        if (cache.Cache == null)
            throw new ArgumentNullException(nameof(cache));
        _cache = cache.Cache.ToArray();
        _provider = services;
        _logger = logger;
    }
    
    public IReadOnlyList<IActivatable> Activated => _activated;

    private readonly List<IActivatable> _activated = new();
    private readonly Type[] _cache;
    private readonly IServiceProvider _provider;
    private readonly ILogger? _logger;

    public void Activate() {
        _logger?.LogDebug("Initiated activation");
        foreach (var type in _cache) {
            try {
                var service = _provider.GetRequiredService(type);
                if (service is not IActivatable activatable ||
                    _activated.Contains(activatable)) continue;
                activatable.Activate();
                _activated.Add(activatable);
                _logger?.LogTrace("Installed " + type);
            } catch (Exception ex) {
                _logger?.LogError("An error occured during installation:\r\n" + ex);
            }
        }
        _logger?.LogDebug("Activation completed");
    }
    
    public void Dispose() {
        _activated.Clear();
    }
}
