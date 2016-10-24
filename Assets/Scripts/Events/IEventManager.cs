public delegate void EventDelegate<T>(T e) where T : GameEvent;

public interface IEventManager
{
    void AddListener<T>(EventDelegate<T> del) where T : GameEvent;

    void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent;

    void Raise(GameEvent e);
}
