using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Waiting,Playing,Pause,Win,ReadyWin,Lose
}
public class GameController : GOSingleton<GameController>
{
    public GameState State;
    public Transform BallZone;
    public Vector3 target;
    public Transform FirstBall;
    private Vector3 intialPos_BallZone;
    bool isLoadDone;
    private void Awake()
    {
        target = BallZone.position;
        intialPos_BallZone = BallZone.position;
        BallPool.GetInstance().OnInit();
        SaveLoadManager.GetInstance().OnInit();
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
        target = intialPos_BallZone+ new Vector3(0,Ball.BallRadius*2,0)*time;
        if (LevelManager.numBallColor[BallColor.Red] + LevelManager.numBallColor[BallColor.Green]+ LevelManager.numBallColor[BallColor.Blue]==0)
        {
            State = GameState.ReadyWin;
            StartCoroutine(IEWin());
        }
        else if(Shooter.GetInstance().NumBall == 0)
        {
            //StartCoroutine(IELose());
            Lose();
        }
    }
    public void ChooseLevel(int level)
    {
        State = GameState.Waiting;
        LevelManager.GetInstance().LoadLevel(level);
        UIManager.GetInstance().OpenUI<UIGamePlay>();
        //UIManager.GetInstance().GetUI<UIGamePlay>().SetScoreText(Constants.Score);
        
        
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
        UIManager.GetInstance().OpenUI<UIWinGame>();
        
        ChangeState(GameState.Win);
    }

    IEnumerator IEWin()
    {
        //Shooter.GetInstance().ClearBall();
        yield return StartCoroutine(Shooter.GetInstance().IEClearBall(Time.deltaTime*10f));
        yield return new WaitForSeconds(2f);
        Win();
    }
    public void Lose()
    {
        UIManager.GetInstance().OpenUI<UILoseGame>(); 
        ChangeState(GameState.Lose);
    }
    IEnumerator IELose()
    {
        //Shooter.GetInstance().ClearBall();
        yield return StartCoroutine(Shooter.GetInstance().IEClearBall(Time.deltaTime));
        yield return new WaitForSeconds(2f);
        Lose();
    }
}
