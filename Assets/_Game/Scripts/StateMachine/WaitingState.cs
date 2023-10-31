using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingState : IState<G4_GameController>
{
    public void OnEnter(G4_GameController g4_GameController)
    {

    }

    public void OnExecute(G4_GameController g4_GameController)
    {
        g4_GameController.FixBallZone();
    }

    public void OnExit(G4_GameController g4_GameController)
    {
    }
}
