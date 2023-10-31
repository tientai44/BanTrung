using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum G4_GameState
{
    Waiting,Playing,Pause,Win,ReadyWin,Lose
}
public class G4_GameController : G4_GOSingleton<G4_GameController>
{
    public G4_GameState State = G4_GameState.Waiting;
    public Transform BallZone;
    public Vector3 target;
    public Transform FirstBall;
    private Vector3 intialPos_BallZone;
    bool isLoadDone=true;
    private IState<G4_GameController> currentState;
    private int score;
    public IState<G4_GameController> CurrentState { get => currentState;}
    public int Score { get => score; }

    private event Action onScoreChange;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 60;
        int maxScreenHeight = 1280;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }
        target = BallZone.position;
        intialPos_BallZone = BallZone.position;
        G4_BallPool.GetInstance().OnInit();
        G4_SaveLoadManager.GetInstance().OnInit();
        ResetScore();
        //ChooseLevel(3);
        G4_UIManager.GetInstance().OpenUI<G4_UIMainMenu>();
   
    }
    private void FixedUpdate()
    {
        if(currentState!=null)
            currentState.OnExecute(this);
    }
    public void FixBallZone()
    {
        if (Vector3.Distance(BallZone.position, target) <= 0.1f && isLoadDone == false)
        {
            isLoadDone = true;
            ChangeState(new PlayingState());
        }

        BallZone.position = Vector3.MoveTowards(BallZone.position, target, Time.deltaTime);
    }
    public void UpScore(int newScore) {
        score += newScore;
        ObserverManager.GetInstance().InvokeEvent(EventName.ScoreChange);
    }
    public void ResetScore()
    {
        score = 0;
        ObserverManager.GetInstance().InvokeEvent(EventName.ScoreChange);
    }
    public void SetLine(int time=0)// Xet lai vi tri BallZone
    {
        isLoadDone = false;
        ChangeState(new WaitingState());
        target = intialPos_BallZone+ new Vector3(0,G4_Ball.BallRadius*2,0)*time;
        if (G4_LevelManager.GetInstance().CheckWin())
        {
            ChangeState(new WinState());
        }
        else if(G4_Shooter.GetInstance().NumBall == 0)
        {
            //StartCoroutine(IELose());
            ChangeState(new LoseState());
        }
    }
    public void ChooseLevel(int level)
    {
        Debug.Log(level);
        target = intialPos_BallZone;
        BallZone.position = target;
        ChangeState(new WaitingState());
        G4_LevelManager.GetInstance().LoadLevel(level);
        G4_UIManager.GetInstance().OpenUI<G4_UIGamePlay>();
        //UIManager.GetInstance().GetUI<UIGamePlay>().SetScoreText(Constants.Score);
        
        
    }
    public void ChangeState(IState<G4_GameController> state)
    {
        if(currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = state;
        currentState.OnEnter(this);
    }
    IEnumerator IEChangeState(IState<G4_GameController> state, float time)
    {
        yield return new WaitForSeconds(time);
        ChangeState(state);
    }
    public void ChangeState(IState<G4_GameController> state,float time)
    {
        StartCoroutine(IEChangeState(state,time));
    }
    public void Win()
    {
        StartCoroutine(IEWin());
    }

    IEnumerator IEWin()
    {
        //Shooter.GetInstance().ClearBall();
        yield return StartCoroutine(G4_LevelManager.GetInstance().FallAllBall(Time.deltaTime));
        yield return StartCoroutine(G4_Shooter.GetInstance().IEClearBall(Time.deltaTime));
        yield return new WaitForSeconds(2f);
        G4_UIManager.GetInstance().OpenUI<G4_UIWinGame>();
        int lv = G4_LevelManager.CurrentLevel;
        if (G4_SaveLoadManager.GetInstance().Data1.CurrentLv <= lv)
        {
            G4_SaveLoadManager.GetInstance().Data1.CurrentLv = lv + 1;
            G4_SaveLoadManager.GetInstance().Data1.Points.Add(0);
            G4_SaveLoadManager.GetInstance().Data1.StarNumbers.Add(0);
        }
        if (G4_SaveLoadManager.GetInstance().Data1.Points[lv - 1] <= score)
        {
            G4_SaveLoadManager.GetInstance().Data1.Points[lv - 1] = score;
        }
        int star = 0;
        if (score > G4_LevelManager.CheckPoints[0])
        {
            star = 1;
        }
        if (score > G4_LevelManager.CheckPoints[1])
        {
            star = 2;
        }
        if (score > G4_LevelManager.CheckPoints[2])
        {
            star = 3;
        }
        if (G4_SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] <= star)
        {
            G4_SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] = star;
        }
        G4_SaveLoadManager.GetInstance().Save();
    }
    public void Lose()
    {
        StartCoroutine(IELose());
    }
    IEnumerator IELose()
    {
        //Shooter.GetInstance().ClearBall();
        yield return StartCoroutine(G4_Shooter.GetInstance().IEClearBall(Time.deltaTime));
        yield return new WaitForSeconds(2f);
        G4_UIManager.GetInstance().OpenUI<G4_UILoseGame>();
    }
}
