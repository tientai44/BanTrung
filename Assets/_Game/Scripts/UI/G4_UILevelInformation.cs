using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G4_UILevelInformation : G4_UICanvas
{
    [SerializeField] Text levelText;
    [SerializeField] List<G4_Star> stars;
    [SerializeField] Text missionText;
    [SerializeField] Button playButton;
    [SerializeField] Text scoreText;
    public void SetUp(int lv)
    {
        levelText.text = lv.ToString();
        missionText.text = G4_LevelScriptTableObject.descriptions[G4_LevelManager.GetInstance().levelInfors[lv-1].LevelType];
        if (G4_SaveLoadManager.GetInstance().Data1.StarNumbers.Count >= lv)
        {
            for (int i = 0; i < 3; i++)
            {
                stars[i].gameObject.SetActive(true);
                if (G4_SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] > i)
                {
                    stars[i].PlayAnim("Appear");
                }
            }

        }
        if (G4_SaveLoadManager.GetInstance().Data1.Points.Count >= lv)
        {
           scoreText.text = G4_SaveLoadManager.GetInstance().Data1.Points[lv-1].ToString();
        }
        else
        {
            scoreText.text = "0";
        }
        playButton.onClick.AddListener(() => TaskOnClick(lv));
    }
    public void TaskOnClick(int lv)
    {
        G4_GameController.GetInstance().ChooseLevel(lv);
        G4_UIManager.GetInstance().CloseUI<G4_UIMainMenu>();
        G4_UIManager.GetInstance().CloseUI<G4_UILevelInformation>();
    }
    public void CloseButton()
    {
        G4_UIManager.GetInstance().OpenUI<G4_UIMainMenu>();

        G4_UIManager.GetInstance().CloseUI<G4_UIGamePlay>();
        G4_UIManager.GetInstance().CloseUI<G4_UILevelInformation>();
    }
}
