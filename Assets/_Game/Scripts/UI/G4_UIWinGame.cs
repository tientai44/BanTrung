﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G4_UIWinGame : G4_UICanvas
{
    [SerializeField] Text scoreText;
    [SerializeField] Text levelText;
    [SerializeField] List<G4_Star> stars;
    float targetScore;
    float currentScore;
    float incrementRate = 100; // Tốc độ tăng giá trị

    public override void Open()
    {
        base.Open();
        G4_SoundManager.GetInstance().PlayOneShot(SoundFX.Win);
        SetLevelText(G4_LevelManager.CurrentLevel);
        //SetScoreText(G4_Constants.Score);
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
            if (currentScore > G4_LevelManager.CheckPoints[i])
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
        if(G4_LevelManager.GetInstance().levelInfors.Count >= G4_LevelManager.CurrentLevel + 1) {
            G4_GameController.GetInstance().ChooseLevel(G4_LevelManager.CurrentLevel + 1);
            Close(0f);
        }
        
    }
    public void ReplayButton()
    {
        G4_GameController.GetInstance().ChooseLevel(G4_LevelManager.CurrentLevel);
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
        G4_UIManager.GetInstance().CloseAll();
        G4_UIManager.GetInstance().OpenUI<G4_UIMainMenu>();
    }
}
