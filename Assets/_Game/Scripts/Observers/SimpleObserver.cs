using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObserver : Observer
{
    private event Action action;
    public void InvokeAction()
    {
        action?.Invoke();
    }

    public void Register(Action callback)
    {
        action +=callback;  
    }

    public void Unregister(Action callback)
    {
        action -=callback;
    }
}
