using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public delegate void EventHandler(EventType type,Message data);

public enum EventType
{
    GameFail,
    GameWin
}

public class Message
{
    public string message;
    public Message(string message)
    {
        this.message = message;
    }
}

public class EventManager
{
    private static EventManager instance;
    private Dictionary<EventType, EventHandler> eventDic = new Dictionary<EventType, EventHandler>();

    public static EventManager GetInstance()
    {
        if (instance == null)
        {
            instance = new EventManager();
        }

        return instance;
    }

    public void AddListener(EventType type,EventHandler eventHandler)
    {
        if (!eventDic.ContainsKey(type))
        {
            eventDic.Add(type,eventHandler);
            return;
        }

        Delegate[] hDelegates = eventDic[type].GetInvocationList();
        if (Array.IndexOf(hDelegates, eventHandler) == -1)
        {
            eventDic[type] += eventHandler;
        }
    }

    public void RemoveListener(EventType type)
    {
        if (!eventDic.ContainsKey(type))
        {
            MDebug.LogError("Don't have any Listener");
        }
        eventDic.Remove(type);
    }

    public void RemoveListener(EventType type, EventHandler eventHandler)
    {
        if (!eventDic.ContainsKey(type))
        {
            MDebug.LogError("Don't have any Listener");
        }
        
        Delegate[] hDelegates = eventDic[type].GetInvocationList();
        if (Array.IndexOf(hDelegates, eventHandler) != -1)
        {
            eventDic[type] -= eventHandler;
        }
        
    }

    public void Dispatcher(EventType type, Message data)
    {
        if (!eventDic.ContainsKey(type))
        {
            MDebug.LogError("Don't have any Listener");
        }
        eventDic[type].Invoke(type,data);
    }

    public void ClearDic()
    {
        eventDic.Clear();
    }
}
