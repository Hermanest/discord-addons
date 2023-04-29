using Discord.Addons.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Addons.SetupModule.Utils; 

[PublicAPI]
public static class SetupModuleInstallationExtensions {
    public static ListedCacheBuilder<Type, SetupCommandModule> Add<TType>(
        this ListedCacheBuilder<Type, SetupCommandModule> builder) {
        return builder.AddType<SetupCommandModule, TType>();
    }
    
    public static ListedCacheBuilder<Type, InteractionHandleService> AddConfigSetupModule(
        this ListedCacheBuilder<Type, InteractionHandleService> interactionCacheBuilder, 
        Action<ListedCacheBuilder<Type, SetupCommandModule>> callback) {
        var serviceCollection = interactionCacheBuilder.collection!;
        var cacheBuilder = new ListedCacheBuilder<Type, SetupCommandModule>(serviceCollection, false);
        callback(cacheBuilder);
        serviceCollection.AddSingleton(cacheBuilder.Build());
        interactionCacheBuilder.Add<SetupCommandModule>();
        return interactionCacheBuilder;
    }
}