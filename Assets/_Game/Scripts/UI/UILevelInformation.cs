using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelInformation : UICanvas
{
    [SerializeField] Text levelText;
    [SerializeField] List<Star> stars;
    [SerializeField] Text missionText;
    [SerializeField] Button playButton;
    [SerializeField] Text scoreText;
    public void SetUp(int lv)
    {
        levelText.text = lv.ToString();
        missionText.text = LevelScriptTableObject.descriptions[LevelManager.GetInstance().levelInfors[lv-1].LevelType];
        if (SaveLoadManager.GetInstance().Data1.StarNumbers.Count >= lv)
        {
            for (int i = 0; i < 3; i++)
            {
                stars[i].gameObject.SetActive(true);
                if (SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] > i)
                {
                    stars[i].PlayAnim("Appear");
                }
            }

        }
        if (SaveLoadManager.GetInstance().Data1.Points.Count >= lv)
        {
           scoreText.text = SaveLoadManager.GetInstance().Data1.Points[lv-1].ToString();
        }
        else
        {
            scoreText.text = "0";
        }
        playButton.onClick.AddListener(() => TaskOnClick(lv));
    }
    public void TaskOnClick(int lv)
    {
        GameController.GetInstance().ChooseLevel(lv);
        UIManager.GetInstance().CloseUI<UIMainMenu>();
        UIManager.GetInstance().CloseUI<UILevelInformation>();
    }
    public void CloseButton()
    {
        UIManager.GetInstance().OpenUI<UIMainMenu>();

        UIManager.GetInstance().CloseUI<UIGamePlay>();
        UIManager.GetInstance().CloseUI<UILevelInformation>();
    }
}
