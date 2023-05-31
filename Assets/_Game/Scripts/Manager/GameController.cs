using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Waiting,Playing,Pause
}
public class GameController : GOSingleton<GameController>
{
    public GameState State;
    public Transform BallZone;
    public Vector3 target;
    private Vector3 intialPos_BallZone;
    private void Awake()
    {
        target = BallZone.position;
        intialPos_BallZone = BallZone.position;
        BallPool.GetInstance().OnInit();
        ChooseLevel(2);
    }
   
    private void FixedUpdate()
    {
        if (Vector3.Distance(BallZone.position,target)<=0.1f)
        {
            State = GameState.Playing;
        }
        else
        {
            State = GameState.Waiting;
        }
        BallZone.position = Vector3.MoveTowards(BallZone.position,target,1f*Time.deltaTime);
    }

    public void SetLine(int time=0)
    {
        target = intialPos_BallZone+ new Vector3(0,0.5f,0)*time;
        
    }
    public void ChooseLevel(int level)
    {
        State = GameState.Waiting;
        LevelManager.GetInstance().LoadLevel(level);
        UIManager.GetInstance().OpenUI<UIGamePlay>();
        Shooter.GetInstance().OnInit();
    }
}
