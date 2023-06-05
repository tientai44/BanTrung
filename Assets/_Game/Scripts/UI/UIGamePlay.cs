using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : UICanvas
{
    [SerializeField] private Text scroreText;
    [SerializeField] private Image missionImg;
    [SerializeField] private Text MissionProcessTxt;
    [SerializeField] private Text numBallText;
    [SerializeField] private Button SwitchButton;
    [SerializeField] private Image imgFill;
    [SerializeField] private List<Star> stars = new List<Star>();
    private float targetFill;
    public override void Open()
    {
        base.Open();
        SetScoreText(Constants.Score);
        foreach (Star star in stars)
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
        Shooter.GetInstance().TF.position = shooterPoint;
        
    }
    public void SetScoreText(int score)
    {
        scroreText.text = score.ToString();
        targetFill = (float)score/LevelManager.CheckPoints[2];
        if(score < LevelManager.CheckPoints[0])
        {
            targetFill = (float)score / LevelManager.CheckPoints[1] * 1 / 3;
        }
        else if (score < LevelManager.CheckPoints[1])
        {
            targetFill = (float)score / LevelManager.CheckPoints[1]*2/3;

        }
        else 
        {
            targetFill = (float)score / LevelManager.CheckPoints[2];
            if (targetFill > 1)
            {
                targetFill = 1;
            }
        }
        for (int i = 0; i < stars.Count; i++)
        {
            if (score > LevelManager.CheckPoints[i])
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
        Shooter.GetInstance().SwitchBall();
    }
    public void TripleModeButton()
    {
        Shooter.GetInstance().EnableTripleMode();
    }
    public void FullColorModeButton()
    {
        Shooter.GetInstance().EnableFullColorBall();
    }
    public void SettingButton()
    {
        if(GameController.GetInstance().State != GameState.Playing)
        {
            return;
        }
        UIManager.GetInstance().OpenUI<UISettingMenu>();
    }
    public void SetMissionProcess(string txt)
    {
        MissionProcessTxt.text = txt;
    }
}
