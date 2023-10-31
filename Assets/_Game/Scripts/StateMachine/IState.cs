using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T> where T : Component
{
    public void OnExecute(T obj);
    public void OnEnter(T obj);
    public void OnExit(T obj);


 }
