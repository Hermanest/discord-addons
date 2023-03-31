namespace Discord.Addons.Data; 

public interface INotifiableMemoryPool : IMemoryPool {
    event Func<string, object, Task> ItemAddedEvent;
    event Func<string, object, Task> ItemGrabbedEvent;
    event Func<string, object, Task> ItemReleasedEvent;
}