using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G4_UISettingMenu : G4_UICanvas
{
    [SerializeField] ToggleButton btnSound;
    [SerializeField] ToggleButton btnMusic;
    private void Awake()
    {
        btnMusic.Switch(UserData.IsMusicOn);
        btnSound.Switch(UserData.IsSoundOn);
        btnMusic.Button.onClick.AddListener(() =>
        {
            UserData.IsMusicOn = !UserData.IsMusicOn;
            btnMusic.Switch(UserData.IsMusicOn);
            G4_SoundManager.GetInstance().EnableMusic(UserData.IsMusicOn);
        });

        btnSound.Button.onClick.AddListener(() =>
        {
            UserData.IsSoundOn = !UserData.IsSoundOn;
            btnSound.Switch(UserData.IsSoundOn);

        });
    }
    public override void Open()
    {
        base.Open();
        //G4_GameController.GetInstance().ChangeState(new WaitingState());
        Time.timeScale = 0f;
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
    public override void CloseDirectly()
    {
        Time.timeScale = 1f;
        base.CloseDirectly();
    }
}
