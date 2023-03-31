using JetBrains.Annotations;

namespace Discord.Addons.Utils; 

[PublicAPI]
public static class ExpressionExtensions {
    public static IList<T> AppendIf<T>(this IList<T> list, bool condition, Func<T> factory) {
        if (!condition) return list;
        list.Add(factory());
        return list;
    }
    
    public static IList<T> AppendIf<T>(this IList<T> list, Func<bool> predicate, Func<T> factory) {
        return AppendIf(list, predicate(), factory);
    }
    
    public static T If<T>(this T t, bool condition, Action<T> action) {
        if (!condition) return t;
        action(t);
        return t;
    }
    
    public static T If<T>(this T t, Func<bool> predicate, Action<T> action) {
        return If(t, predicate(), action);
    }
}