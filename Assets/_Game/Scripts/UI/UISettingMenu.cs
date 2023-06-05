using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingMenu : UICanvas
{
    public override void Open()
    {
        base.Open();
        GameController.GetInstance().State = GameState.Waiting;
    }
    public void ReplayButton()
    {
        GameController.GetInstance().ChooseLevel(LevelManager.CurrentLevel);
        Close(0f);
        
    }
    public void QuitButton()
    {
        UIManager.GetInstance().CloseAll();
        UIManager.GetInstance().OpenUI<UIMainMenu>();
    }
    public void CloseButton()
    {
        Close(0f);
        GameController.GetInstance().ChangeState(GameState.Playing,0.5f);
    }
    
}
