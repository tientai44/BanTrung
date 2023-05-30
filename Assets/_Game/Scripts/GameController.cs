using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : GOSingleton<GameController>
{
    public Transform BallZone;
    public Vector3 target;
    private Vector3 intialPos_BallZone;
    private void Awake()
    {
        target = BallZone.position;
        intialPos_BallZone = BallZone.position;
    }
   
    private void FixedUpdate()
    {
        BallZone.position = Vector3.MoveTowards(BallZone.position,target,1f*Time.deltaTime);
    }

    public void SetLine(int time=1)
    {
        target = intialPos_BallZone+ new Vector3(0,0.5f,0)*time;
        
    }
   
}
