using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G4_UISettingMenu : G4_UICanvas
{
    public override void Open()
    {
        base.Open();
        //G4_GameController.GetInstance().ChangeState(new WaitingState());
    }
    public void ReplayButton()
    {
        G4_GameController.GetInstance().ChooseLevel(G4_LevelManager.CurrentLevel);
        Close(0f);
        
    }
    public void QuitButton()
    {
        G4_UIManager.GetInstance().CloseAll();
        G4_UIManager.GetInstance().OpenUI<G4_UIMainMenu>();
    }
    public void CloseButton()
    {
        Close(0f);
        //G4_GameController.GetInstance().ChangeState(G4_GameState.Playing,0.5f);
    }
    
}
