using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : IState<G4_GameController>
{
    public void OnEnter(G4_GameController g4_GameController)
    {
        G4_GameController.GetInstance().Win();     
    }

    public void OnExecute(G4_GameController g4_GameController)
    {
        
    }

    public void OnExit(G4_GameController g4_GameController)
    {
        
    }
}
