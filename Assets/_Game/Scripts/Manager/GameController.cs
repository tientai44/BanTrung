using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Waiting,Playing,Pause,Win
}
public class GameController : GOSingleton<GameController>
{
    public GameState State;
    public Transform BallZone;
    public Vector3 target;
    private Vector3 intialPos_BallZone;
    bool isLoadDone;
    private void Awake()
    {
        target = BallZone.position;
        intialPos_BallZone = BallZone.position;
        BallPool.GetInstance().OnInit();
        ChooseLevel(3);
   
    }
   
    private void FixedUpdate()
    {
        if (Vector3.Distance(BallZone.position,target)<=0.1f && isLoadDone==false )
        {
            isLoadDone = true;
            if(State is GameState.Waiting)
                State = GameState.Playing;
        }
        
        BallZone.position = Vector3.MoveTowards(BallZone.position,target,Time.deltaTime);
    }

    public void SetLine(int time=0)// Xet lai vi tri BallZone
    {
        isLoadDone = false;
        State = GameState.Waiting;
        target = intialPos_BallZone+ new Vector3(0,0.5f,0)*time;
        if (LevelManager.numBallColor[BallColor.Red] + LevelManager.numBallColor[BallColor.Green]+ LevelManager.numBallColor[BallColor.Blue]==0)
        {
            Win();
        }
    }
    public void ChooseLevel(int level)
    {
        State = GameState.Waiting;
        LevelManager.GetInstance().LoadLevel(level);
        UIManager.GetInstance().OpenUI<UIGamePlay>();
        Shooter.GetInstance().OnInit();
    }
    public void ChangeState(GameState state)
    {
        this.State = state;
    }
    IEnumerator IEChangeState(GameState state, float time)
    {
        yield return new WaitForSeconds(time);
        this.State = state;
    }
    public void ChangeState(GameState state,float time)
    {
        StartCoroutine(IEChangeState(state,time));
    }
    public void Win()
    {
        ChangeState(GameState.Win);
    }
}
