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
        //ChooseLevel(3);
        G4_UIManager.GetInstance().OpenUI<G4_UIMainMenu>();
   
    }
   
    private void FixedUpdate()
    {
        if (Vector3.Distance(BallZone.position,target)<=0.1f && isLoadDone==false )
        {
            isLoadDone = true;
            if(State is G4_GameState.Waiting)
                State = G4_GameState.Playing;
        }
        
        BallZone.position = Vector3.MoveTowards(BallZone.position,target,Time.deltaTime);
    }

    public void SetLine(int time=0)// Xet lai vi tri BallZone
    {
        isLoadDone = false;
        State = G4_GameState.Waiting;
        target = intialPos_BallZone+ new Vector3(0,G4_Ball.BallRadius*2,0)*time;
        if (G4_LevelManager.GetInstance().CheckWin())
        {
            State = G4_GameState.ReadyWin;
            StartCoroutine(IEWin());
        }
        else if(G4_Shooter.GetInstance().NumBall == 0)
        {
            //StartCoroutine(IELose());
            Lose();
        }
    }
    public void ChooseLevel(int level)
    {
        Debug.Log(level);
        State = G4_GameState.Waiting;
        target = intialPos_BallZone;
        BallZone.position = target;
        G4_LevelManager.GetInstance().LoadLevel(level);
        G4_UIManager.GetInstance().OpenUI<G4_UIGamePlay>();
        //UIManager.GetInstance().GetUI<UIGamePlay>().SetScoreText(Constants.Score);
        
        
    }
    public void ChangeState(G4_GameState state)
    {
        this.State = state;
    }
    IEnumerator IEChangeState(G4_GameState state, float time)
    {
        yield return new WaitForSeconds(time);
        this.State = state;
    }
    public void ChangeState(G4_GameState state,float time)
    {
        StartCoroutine(IEChangeState(state,time));
    }
    public void Win()
    {
        G4_UIManager.GetInstance().OpenUI<G4_UIWinGame>();
        int lv = G4_LevelManager.CurrentLevel;
        if (G4_SaveLoadManager.GetInstance().Data1.CurrentLv <= lv) {
            G4_SaveLoadManager.GetInstance().Data1.CurrentLv = lv+1;
            G4_SaveLoadManager.GetInstance().Data1.Points.Add(0);
            G4_SaveLoadManager.GetInstance().Data1.StarNumbers.Add(0);
        }
        if (G4_SaveLoadManager.GetInstance().Data1.Points[lv-1]<=G4_Constants.Score)
        {
            G4_SaveLoadManager.GetInstance().Data1.Points[lv - 1] = G4_Constants.Score;
        }
        int star=0;
        if (G4_Constants.Score > G4_LevelManager.CheckPoints[0]) {
            star = 1;
        }
        if (G4_Constants.Score > G4_LevelManager.CheckPoints[1])
        {
            star = 2;
        }
        if (G4_Constants.Score > G4_LevelManager.CheckPoints[2])
        {
            star = 3;
        }
        if (G4_SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] <= star)
        {
            G4_SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] = star;
        }
        G4_SaveLoadManager.GetInstance().Save();
        ChangeState(G4_GameState.Win);
    }

    IEnumerator IEWin()
    {
        //Shooter.GetInstance().ClearBall();
        yield return StartCoroutine(G4_LevelManager.GetInstance().FallAllBall(Time.deltaTime));
        yield return StartCoroutine(G4_Shooter.GetInstance().IEClearBall(Time.deltaTime));
        yield return new WaitForSeconds(2f);
        Win();
    }
    public void Lose()
    {
        G4_UIManager.GetInstance().OpenUI<G4_UILoseGame>(); 
        ChangeState(G4_GameState.Lose);
    }
    IEnumerator IELose()
    {
        //Shooter.GetInstance().ClearBall();
        yield return StartCoroutine(G4_Shooter.GetInstance().IEClearBall(Time.deltaTime));
        yield return new WaitForSeconds(2f);
        Lose();
    }
}
