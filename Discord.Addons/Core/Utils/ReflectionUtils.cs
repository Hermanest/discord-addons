using System.Reflection;
using JetBrains.Annotations;

namespace Discord.Addons.Utils {
    [PublicAPI]
    public static class ReflectionUtils {
        public record PropertySnapshot(string Name, string? Value);
        public record PropertyReport(string Name, string? OldValue, string? NewValue);

        public const BindingFlags RequiredFlags = BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags DefaultFlags = RequiredFlags | BindingFlags.Instance;
        public const BindingFlags StaticFlags = RequiredFlags | BindingFlags.Static;
        public const BindingFlags UniversalFlags = RequiredFlags | BindingFlags.Instance | BindingFlags.Static;

        public static T? GetMember<T>(this Type type, Func<T, bool>? comparator = null,
            BindingFlags flags = UniversalFlags) where T : MemberInfo {
            return type.GetMembers(flags).FirstOrDefault(x => x is T t && (comparator?.Invoke(t) ?? true)) as T;
        }

        public static IEnumerable<PropertySnapshot> CreateSnapshot(object obj,
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) =>
            obj.GetType().GetProperties(flags)
                .Where(x => x.CanRead && x.CanWrite)
                .Select(x => new PropertySnapshot(
                    x.Name, x.GetValue(obj)?.ToString() ?? "empty")).ToArray();

        public static IEnumerable<PropertyReport> CreateReport(
            IEnumerable<PropertySnapshot> oldSnapshot,
            IEnumerable<PropertySnapshot> newSnapshot) => oldSnapshot
            .Select(x => new PropertyReport(x.Name, x.Value,
                newSnapshot.FirstOrDefault(y => y.Name == x.Name).Value))
            .Where(x => x.OldValue != x.NewValue);
    }
}