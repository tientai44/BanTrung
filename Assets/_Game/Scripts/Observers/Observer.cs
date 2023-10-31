using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Observer 
{
    void Register(Action callback);

    void Unregister(Action callback);

    void InvokeAction();

}
