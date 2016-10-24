using UnityEngine;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

/// <summary>
/// http://www.willrmiller.com/?p=87
/// </summary>
public class EventManager: View, IEventManager
{
    private Dictionary<System.Type, System.Delegate> delegates = new Dictionary<System.Type, System.Delegate>();

    public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        if (delegates.ContainsKey(typeof(T)))
        {
            System.Delegate tempDel = delegates[typeof(T)];

            delegates[typeof(T)] = System.Delegate.Combine(tempDel, del);
        }
        else
        {
            delegates[typeof(T)] = del;
        }
    }

    public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        if (delegates.ContainsKey(typeof(T)))
        {
            var currentDel = System.Delegate.Remove(delegates[typeof(T)], del);

            if (currentDel == null)
            {
                delegates.Remove(typeof(T));
            }
            else
            {
                delegates[typeof(T)] = currentDel;
            }
        }
    }

    public void Raise(GameEvent e)
    {
        if (e == null)
        {
            Debug.Log("Invalid event argument: " + e.GetType().ToString());
            return;
        }

        if (delegates.ContainsKey(e.GetType()))
        {
            delegates[e.GetType()].DynamicInvoke(e);
        }
    }
}
