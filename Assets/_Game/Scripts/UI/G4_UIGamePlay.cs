using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G4_UIGamePlay : G4_UICanvas
{
    [SerializeField] private Text scroreText;
    [SerializeField] private Image missionImg;
    [SerializeField] private Text MissionProcessTxt;
    [SerializeField] private Text numBallText;
    [SerializeField] private Button SwitchButton;
    [SerializeField] private Image imgFill;
    [SerializeField] private List<G4_Star> stars = new List<G4_Star>();
    [SerializeField] private Sprite ClearBallSprite;
    [SerializeField] private Sprite RescueSprite;
    [SerializeField] private Sprite CollectFlowerSprite;
    private float targetFill;
    private void OnEnable()
    {
        ObserverManager.GetInstance().Register(EventName.ScoreChange, () =>
        {
            SetScoreText(G4_GameController.GetInstance().Score);
        });
    }
    private void OnDisable()
    {
        ObserverManager.GetInstance()?.Unregister(EventName.ScoreChange, () =>
        {
            SetScoreText(G4_GameController.GetInstance().Score);
        });
    }
    public override void Open()
    {
        base.Open();
        //SetScoreText(G4_Constants.Score);
        foreach (G4_Star star in stars)
        {
            star.PlayAnim("Disappear");
        }
        imgFill.fillAmount = 0;
    }
    private void Update()
    {
        imgFill.fillAmount = Mathf.Lerp(imgFill.fillAmount, targetFill, Time.deltaTime*5f);
    }
    public void SetShooterPosition()
    {
        Vector2 shooterPoint = Camera.main.ScreenToWorldPoint(SwitchButton.transform.position);
        G4_Shooter.GetInstance().TF.position = shooterPoint;
        
    }
    public void SetScoreText(int score)
    {
        scroreText.text = score.ToString();
        targetFill = (float)score/G4_LevelManager.CheckPoints[2];
        if(score < G4_LevelManager.CheckPoints[0])
        {
            targetFill = (float)score / G4_LevelManager.CheckPoints[1] * 1 / 3;
        }
        else if (score < G4_LevelManager.CheckPoints[1])
        {
            targetFill = (float)score / G4_LevelManager.CheckPoints[1]*2/3;

        }
        else 
        {
            targetFill = (float)score / G4_LevelManager.CheckPoints[2];
            if (targetFill > 1)
            {
                targetFill = 1;
            }
        }
        for (int i = 0; i < stars.Count; i++)
        {
            if (score > G4_LevelManager.CheckPoints[i])
            {
                stars[i].PlayAnim("Appear");
            }
        }

    }
    public void SetNumBall(int num)
    {
        numBallText.text = num.ToString();
    }
    public void SwitchBallButton()
    {
        G4_Shooter.GetInstance().SwitchBall();
    }
    public void TripleModeButton()
    {
        G4_Shooter.GetInstance().EnableTripleMode();
    }
    public void FullColorModeButton()
    {
        G4_Shooter.GetInstance().EnableMode(ShooterMode.FullColor);
    }
    public void FireBallModeButton()
    {
        G4_Shooter.GetInstance().EnableMode(ShooterMode.FireBall);
    }
    public void BombModeButton()
    {
        G4_Shooter.GetInstance().EnableMode(ShooterMode.Bomb);
    }
    public void SettingButton()
    {
        //if(G4_GameController.GetInstance().State != G4_GameState.Playing)
        //{
        //    return;
        //}
        G4_UIManager.GetInstance().OpenUI<G4_UISettingMenu>();
    }
    public void SetMissionProcess(string txt)
    {
        MissionProcessTxt.text = txt;
    }
    public void SetMissionImg(G4_LevelType type)
    {
        if (type is G4_LevelType.CollectFlower)
        {
            missionImg.sprite = CollectFlowerSprite;
        }
        if(type is G4_LevelType.ClearBall)
        {
            missionImg.sprite= ClearBallSprite;
        }
        if(type is G4_LevelType.SaveRabbit)
        {
            missionImg.sprite = RescueSprite;
        }
    }

}
