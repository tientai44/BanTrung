﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWinGame : UICanvas
{
    [SerializeField] Text scoreText;
    [SerializeField] Text levelText;
    [SerializeField] List<Star> stars;
    float targetScore;
    float currentScore;
    float incrementRate = 100; // Tốc độ tăng giá trị

    public override void Open()
    {
        base.Open();
        SetLevelText(LevelManager.CurrentLevel);
        SetScoreText(Constants.Score);
    }

    private void Update()
    {
        // Nếu giá trị hiện tại chưa đạt giá trị cuối cùng
        if (currentScore < targetScore)
        {
            // Tăng giá trị hiện tại theo tốc độ tăng dần
            currentScore += incrementRate * Time.deltaTime;
        }
        else
        {
            // Giá trị đã đạt giá trị cuối cùng
            currentScore = targetScore;
            
        }
        for(int i = 0; i < stars.Count; i++)
        {
            if (currentScore > LevelManager.CheckPoints[i])
            {
                stars[i].PlayAnim("Appear");
            }
        }
      
        scoreText.text = ((int)currentScore).ToString();

    }

    public void SetScoreText(int score)
    {
        currentScore = 0;
        targetScore = score;
        incrementRate = score / 5;
    }
    public void SetLevelText(int level)
    {
        levelText.text = level.ToString();
    }

    public void ContinueButton()
    {
        if(LevelManager.GetInstance().levelInfors.Count >= LevelManager.CurrentLevel + 1) {
            GameController.GetInstance().ChooseLevel(LevelManager.CurrentLevel + 1);
            Close(0f);
        }
        
    }
    public void ReplayButton()
    {
        GameController.GetInstance().ChooseLevel(LevelManager.CurrentLevel);
        Close(0f);
    }
    public override void CloseDirectly()
    {
        for (int i = 0; i < stars.Count; i++)
        {
   
            stars[i].PlayAnim("Disappear");
            
        }
        base.CloseDirectly();
    }
    public void CloseButton()
    {
        UIManager.GetInstance().CloseAll();
        UIManager.GetInstance().OpenUI<UIMainMenu>();
    }
}
