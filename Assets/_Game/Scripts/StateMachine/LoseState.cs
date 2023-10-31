using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : IState<G4_GameController>
{
    public void OnEnter(G4_GameController g4_GameController)
    {
        G4_GameController.GetInstance().Lose();
    }

    public void OnExecute(G4_GameController g4_GameController)
    {
    }

    public void OnExit(G4_GameController g4_GameController)
    {
    }
}
