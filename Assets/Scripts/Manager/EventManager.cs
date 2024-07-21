using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();

    public static void AddEvent(string eventName, Action listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = listener;
        }
        else
        {
            eventDictionary[eventName] += listener;
        }
    }

    public static void RemoveEvnet(string eventName, Action listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }

    public static void TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
