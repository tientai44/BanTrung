using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : UICanvas
{
    [SerializeField] Transform viewport;
    [SerializeField] ButtonLevel buttonLevelPrefab;
    List<ButtonLevel> buttonLevels = new List<ButtonLevel>();
    protected override void OnInit()
    {
        Debug.Log(LevelManager.GetInstance().levelInfors.Count);
        for(int i = 0; i < LevelManager.GetInstance().levelInfors.Count; i++)
        {
            ButtonLevel buttonLV = Instantiate(buttonLevelPrefab,viewport);
            buttonLV.SetUp(i+1);
            buttonLevels.Add(buttonLV);
        }
    }
    public override void Open()
    {
        GameController.GetInstance().State = GameState.Waiting;
        BallPool.GetInstance().ClearAllObjectActive();
        base.Open();
        for (int i = 0; i < buttonLevels.Count; i++)
        {
            buttonLevels[i].SetUp(i + 1);
            
        }
    }
}
