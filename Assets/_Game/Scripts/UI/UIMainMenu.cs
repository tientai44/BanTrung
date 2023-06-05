using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : UICanvas
{
    [SerializeField] Transform viewport;
    [SerializeField] ButtonLevel buttonLevelPrefab;
    protected override void OnInit()
    {
        Debug.Log(LevelManager.GetInstance().levelInfors.Count);
        for(int i = 0; i < LevelManager.GetInstance().levelInfors.Count; i++)
        {
            ButtonLevel buttonLV = Instantiate(buttonLevelPrefab,viewport);
            buttonLV.SetUp(i+1);
        }
    }
    public override void Open()
    {
        GameController.GetInstance().State = GameState.Waiting;
        base.Open();
    }
}
