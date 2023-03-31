namespace Discord.Addons;
/// <summary>
/// Used for activation service to specify a class for automatic launch
/// </summary>
public interface IActivatable {
    void Activate() { }
}