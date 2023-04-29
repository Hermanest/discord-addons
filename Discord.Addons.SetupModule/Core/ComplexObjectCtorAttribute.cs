using JetBrains.Annotations;

namespace Discord.Addons.SetupModule;

[AttributeUsage(AttributeTargets.Method), PublicAPI]
public class ComplexObjectCtorAttribute : Attribute { }