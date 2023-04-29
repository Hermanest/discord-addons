using System.Reflection;
using Discord.Addons.Data;
using Discord.Interactions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Addons.Utils; 

[PublicAPI]
public static class BasicInstallationExtensions {
    #region ByType

    public static ListedCacheBuilder<Type, InteractionHandleService> Add<TType>(
        this ListedCacheBuilder<Type, InteractionHandleService> builder) where TType : InteractionModuleBase<IInteractionContext> {
        return builder.AddType<InteractionHandleService, TType>();
    }
    
    public static ListedCacheBuilder<Type, ActivationService> Add<TType>(
        this ListedCacheBuilder<Type, ActivationService> builder) where TType : IActivatable {
        return builder.AddType<ActivationService, TType>();
    }
    
    public static ListedCacheBuilder<Type, TCacheOwner> AddType<TCacheOwner, TType>(
        this ListedCacheBuilder<Type, TCacheOwner> builder) where TCacheOwner : class {
        return builder.Add(typeof(TType));
    }

    #endregion
    
    #region FromAssembly
    
    public static ListedCacheBuilder<Type, InteractionHandleService> AddAssemblyModules(
        this ListedCacheBuilder<Type, InteractionHandleService> builder, Assembly assembly) {
        return builder.AddAssemblyTypesToCache<InteractionHandleService, InteractionModuleBase<IInteractionContext>>(assembly);
    }
    
    public static ListedCacheBuilder<Type, ActivationService> AddAssemblyServices(
        this ListedCacheBuilder<Type, ActivationService> builder, Assembly assembly) {
        return builder.AddAssemblyTypesToCache<ActivationService, IActivatable>(assembly);
    }
    
    public static ListedCacheBuilder<Type, TCacheOwner> AddAssemblyTypesToCache<TCacheOwner, TAsmType>(
        this ListedCacheBuilder<Type, TCacheOwner> builder, Assembly assembly) where TCacheOwner : class {
        foreach (var item in assembly.GetTypes().Where(static x => x
            .IsAssignableTo(typeof(TAsmType)))) {
            builder.Add(item);
        }
        return builder;
    }
    
    #endregion

    #region Activator

    public static void ActivateServices(this IServiceProvider provider) {
        provider.GetRequiredService<ActivationService>().Activate();
    }
    
    public static void AddActivator(
        this IServiceCollection collection,
        Action<ListedCacheBuilder<Type, ActivationService>> callback) {
        var cacheBuilder = new ListedCacheBuilder<Type, ActivationService>(collection);
        callback(cacheBuilder);
        collection.AddSingleton(cacheBuilder.Build());
        collection.AddSingleton<ActivationService>();
    }  
    
    #endregion
    
    public static ListedCacheBuilder<Type, ActivationService> AddModules(
        this ListedCacheBuilder<Type, ActivationService> serviceCacheBuilder,
        Action<ListedCacheBuilder<Type, InteractionHandleService>> callback) {
        var serviceCollection = serviceCacheBuilder.collection!;
        var cacheBuilder = new ListedCacheBuilder<Type, InteractionHandleService>(serviceCollection, false);
        callback(cacheBuilder);
        serviceCollection.AddSingleton(cacheBuilder.Build());
        serviceCacheBuilder.Add<InteractionHandleService>();
        return serviceCacheBuilder;
    }
    
    public static ListedCacheBuilder<Type, ActivationService> AddCleaner(
        this ListedCacheBuilder<Type, ActivationService> builder) {
        return builder.Add<PoolCleanerService>();
    }
    
    public static void AddConfigurationServices(this IServiceCollection collection, string configPath) {
        collection.AddSingleton(new DataProvider.Cache(configPath));
        collection.AddSingleton<IDataProvider, DataProvider>();
        
        collection.AddSingleton<IMemoryPool, MemoryPool>();
        collection.AddSingleton<INotifiableMemoryPool, MemoryPool>();
        
        collection.AddSingleton<IDiscordConfigurationHelper, DiscordConfigurationHelper>();
    }
}