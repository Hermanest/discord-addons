namespace Discord.Addons.Data;

public interface IMemoryPool {
    /// <summary>
    /// Used to get an instance of an object of the specified type
    /// </summary>
    /// <param name="key">Access key. Leave null to get the global object</param>
    /// <param name="type">Object type</param>
    /// <param name="returnDefaultOnFail">Return default instance of an object</param>
    /// <returns>Object from cache if completed and null if not</returns>
    Task<object?> GetObject(string? key, Type type, bool returnDefaultOnFail = true);
    /// <summary>
    /// Used to add or replace(if already exists) the instance of an object
    /// </summary>
    /// <param name="key">Access key. Leave null to write as global object</param>
    /// <param name="obj">Object instance</param>
    /// <param name="saveImmediate"></param>
    /// <returns>True if completed and False if not</returns>
    Task AddObject(string? key, object obj, bool saveImmediate = true);
    /// <summary>
    /// Releases pool objects
    /// </summary>
    /// <returns></returns>
    Task<int> Release(Func<string, object, bool>? selector = null);
}