using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using JetBrains.Annotations;

namespace Discord.Addons;

[PublicAPI]
public class ListedCacheBuilder<T, TOwner> where T : class where TOwner : class {
    public ListedCacheBuilder(IServiceCollection? collection, bool bindAsService = true) {
        this.collection = collection;
        _bindAsService = bindAsService;
    }

    public readonly IServiceCollection? collection;
    private readonly bool _bindAsService;
    private readonly List<T> _cache = new();
    
    public ListedCacheBuilder<T, TOwner> Add(T obj) {
        if (_cache.Contains(obj)) return this;
        if (_bindAsService && collection is not null) {
            if (obj is Type type) collection.AddSingleton(type);
            else collection.AddSingleton(obj);
        }
        _cache.Add(obj);
        return this;
    }

    public ListedCache<T, TOwner> Build() {
        return new ListedCache<T, TOwner>(_cache);
    }
}

[PublicAPI]
public class ListedCache<T, [UsedImplicitly] TOwner> where TOwner : class {
    public ListedCache(IEnumerable<T> enumerable) {
        Cache = enumerable.ToArray();
    }

    public IReadOnlyList<T>? Cache { get; }
}