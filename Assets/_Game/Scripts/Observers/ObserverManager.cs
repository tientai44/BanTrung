using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventName
{
    ScoreChange
}
public class ObserverManager : G4_GOSingleton<ObserverManager>
{
    List<Observer> observers = new List<Observer>();
    private void Awake()
    {
        OnInit();
    }
    public void OnInit()
    {
        Observer scoreChangeObserver = new SimpleObserver();
        observers.Add(scoreChangeObserver);
    }

    public void Register(EventName eventName,Action action)
    {
        observers[(int)eventName].Register(action);
    }
    public void Unregister(EventName eventName, Action action)
    {
        observers[(int)eventName].Unregister(action);
    }
    public void InvokeEvent(EventName eventName)
    {
        observers[(int)eventName].InvokeAction();
    }
}
