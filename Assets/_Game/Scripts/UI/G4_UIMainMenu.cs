using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G4_UIMainMenu : G4_UICanvas
{
    [SerializeField] Transform viewport;
    [SerializeField] G4_ButtonLevel buttonLevelPrefab;
    List<G4_ButtonLevel> buttonLevels = new List<G4_ButtonLevel>();
    protected override void OnInit()
    {
        //Debug.Log(G4_LevelManager.GetInstance().levelInfors.Count);
        //for(int i = 0; i < G4_LevelManager.GetInstance().levelInfors.Count; i++)
        //{
        //    G4_ButtonLevel buttonLV = Instantiate(buttonLevelPrefab,viewport);
        //    buttonLV.SetUp(i+1);
        //    buttonLevels.Add(buttonLV);
        //}
    }
    public override void Open()
    {
        G4_GameController.GetInstance().State = G4_GameState.Waiting;
        G4_BallPool.GetInstance().ClearAllObjectActive();
        base.Open();
        //for (int i = 0; i < buttonLevels.Count; i++)
        //{
        //    buttonLevels[i].SetUp(i + 1);
            
        //}
    }
}
