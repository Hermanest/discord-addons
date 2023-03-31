namespace Discord.Addons.SetupModule;
/// <summary>
/// Used to specify a target for the automatic configuration module
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SetupWithCommandAttribute : Attribute {
    public SetupWithCommandAttribute(string commandName) {
        CommandName = commandName;
    }

    public string CommandName { get; }
}

