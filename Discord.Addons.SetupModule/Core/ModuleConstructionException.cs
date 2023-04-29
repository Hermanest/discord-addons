using JetBrains.Annotations;

namespace Discord.Addons.SetupModule; 

[PublicAPI]
public class ModuleConstructionException : Exception {
    public ModuleConstructionException(string moduleName) : base($"Failed to construct the module {moduleName}") {}
    public ModuleConstructionException(string? message, Exception? innerException) : base(message, innerException) {}
}