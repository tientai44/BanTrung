using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Waiting,Playing,Pause,Win,ReadyWin,Lose
}
public class GameController : GOSingleton<GameController>
{
    public GameState State = GameState.Waiting;
    public Transform BallZone;
    public Vector3 target;
    public Transform FirstBall;
    private Vector3 intialPos_BallZone;
    bool isLoadDone=true;
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        int maxScreenHeight = 1280;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }
        target = BallZone.position;
        intialPos_BallZone = BallZone.position;
        BallPool.GetInstance().OnInit();
        SaveLoadManager.GetInstance().OnInit();
        //ChooseLevel(3);
        UIManager.GetInstance().OpenUI<UIMainMenu>();
   
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
        if (LevelManager.GetInstance().CheckWin())
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
        Debug.Log(level);
        State = GameState.Waiting;
        target = intialPos_BallZone;
        BallZone.position = target;
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
        int lv = LevelManager.CurrentLevel;
        if (SaveLoadManager.GetInstance().Data1.CurrentLv <= lv) {
            SaveLoadManager.GetInstance().Data1.CurrentLv = lv+1;
            SaveLoadManager.GetInstance().Data1.Points.Add(0);
            SaveLoadManager.GetInstance().Data1.StarNumbers.Add(0);
        }
        if (SaveLoadManager.GetInstance().Data1.Points[lv-1]<=Constants.Score)
        {
            SaveLoadManager.GetInstance().Data1.Points[lv - 1] = Constants.Score;
        }
        int star=0;
        if (Constants.Score > LevelManager.CheckPoints[0]) {
            star = 1;
        }
        if (Constants.Score > LevelManager.CheckPoints[1])
        {
            star = 2;
        }
        if (Constants.Score > LevelManager.CheckPoints[2])
        {
            star = 3;
        }
        if (SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] <= star)
        {
            SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] = star;
        }
        SaveLoadManager.GetInstance().Save();
        ChangeState(GameState.Win);
    }

    IEnumerator IEWin()
    {
        //Shooter.GetInstance().ClearBall();
        yield return StartCoroutine(LevelManager.GetInstance().FallAllBall(Time.deltaTime));
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
