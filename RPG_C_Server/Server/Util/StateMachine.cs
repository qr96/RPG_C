using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine<T> where T : Enum
{
    T nowState;

    Dictionary<T, Action<T>> onEnterEvent = new Dictionary<T, Action<T>>();
    Dictionary<T, Action> onUpdateEvent = new Dictionary<T, Action> ();
    Dictionary<T, Action> onLeaveEvent = new Dictionary<T, Action>();

    public T GetState()
    {
        return nowState;
    }

    public void SetState(T state)
    {
        if (onLeaveEvent.ContainsKey(nowState))
        {
            onLeaveEvent[nowState]?.Invoke();
        }
        if (onEnterEvent.ContainsKey(state))
        {
            onEnterEvent[state]?.Invoke(nowState);
        }
        nowState = state;
    }

    public void SetEvent(T state, Action<T> onEnter, Action onUpdate = null, Action onLeave = null)
    {
        if (onEnter != null)
            onEnterEvent.Add(state, onEnter);
        if (onUpdate != null)
            onUpdateEvent.Add(state, onUpdate);
        if (onLeave != null)
            onLeaveEvent.Add(state, onLeave);
    }

    public void Update()
    {
        if (onUpdateEvent.ContainsKey(nowState))
        {
            onUpdateEvent[nowState]?.Invoke();
        }
    }
}
