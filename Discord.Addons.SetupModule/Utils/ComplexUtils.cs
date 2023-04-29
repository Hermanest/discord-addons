using System.Reflection;
using Discord.Addons.Utils;
using Discord.Interactions;
using JetBrains.Annotations;

namespace Discord.Addons.SetupModule.Utils;

[PublicAPI]
public static class ComplexUtils {
    public static (MethodInfo ctor, ComplexObjectCtorAttribute attr)? GetComplexConstructor(Type type) {
        var attr = default(ComplexObjectCtorAttribute);
        if (type.GetMember<MethodInfo>(x => (attr = x.GetCustomAttribute
                <ComplexObjectCtorAttribute>()) is not null) is not { } ctor) return null;
        return (ctor, attr!);
    }

    public static bool BuildComplexSlashCommand(
        this IInteractionModuleBase module,
        string name, Type type,
        Interactions.Builders.SlashCommandBuilder builder,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        out MethodInfo? complexCtor
    ) {
        complexCtor = default;
        if (GetComplexConstructor(type) is not { } ctor) return false;
        try {
            DiscordUtils.BuildSlashCommand(builder, _
                => module, ctor.ctor, interactionService, serviceProvider);
            complexCtor = ctor.ctor;
            return true;
        } catch (Exception) {
            return false;
        }
    }
}