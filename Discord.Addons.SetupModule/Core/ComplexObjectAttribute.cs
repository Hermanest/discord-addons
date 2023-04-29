using JetBrains.Annotations;

namespace Discord.Addons.SetupModule;
/// <summary>
/// Used to specify a target for the automatic configuration module
/// </summary>
[AttributeUsage(AttributeTargets.Class), PublicAPI]
public class ComplexObjectAttribute : Attribute {
    public ComplexObjectAttribute(string name) => Name = name;

    public string Name { get; }
}

