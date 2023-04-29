using JetBrains.Annotations;

namespace Discord.Addons.Utils; 

[PublicAPI]
public static class CollectionExtensions {
    public static IEnumerable<T> Iterate<T>(this IEnumerable<T> collection, Action<T> iterator) {
        foreach (var item in collection) iterator(item);
        return collection;
    }

    public static IDictionary<U, T> AddRange<U, T>(this IDictionary<U, T> dictionary, IEnumerable<KeyValuePair<U, T>> collection) {
        foreach (var item in collection) dictionary.Add(item);
        return dictionary;
    }
}