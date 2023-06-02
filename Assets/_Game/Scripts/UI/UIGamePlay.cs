using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : UICanvas
{
    [SerializeField] private Text scroreText;
    [SerializeField] private Text numBallText;
    [SerializeField] private Button SwitchButton;
    public override void Open()
    {
        base.Open();
        SetScoreText(Constants.Score);
    }
    public void SetShooterPosition()
    {
        Vector2 shooterPoint = Camera.main.ScreenToWorldPoint(SwitchButton.transform.position);
        Shooter.GetInstance().TF.position = shooterPoint;
    }
    public void SetScoreText(int score)
    {
        scroreText.text = score.ToString();
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
}
