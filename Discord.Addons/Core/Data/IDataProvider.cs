namespace Discord.Addons.Data;

public interface IDataProvider {
    Task Save(object data, string key);
    Task<object?> Read(string key, Type type);
}
