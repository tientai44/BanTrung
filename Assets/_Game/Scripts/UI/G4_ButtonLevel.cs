using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class G4_ButtonLevel : MonoBehaviour
{
    Button btn;
    [SerializeField] Image buttonImg;
    [SerializeField] int level;
    [SerializeField] TextMeshProUGUI txtLevel;
    [SerializeField] List<G4_Star> stars;
    [SerializeField] Sprite LockSprite;
    [SerializeField] Sprite UnLockSprite;
    private bool isLock = true;
    private void OnEnable()
    {
        SetUp(level);
    }
    public void SetUp(int lv)
    {
        btn = GetComponent<Button>();
        txtLevel.text = lv.ToString();
        isLock = G4_SaveLoadManager.GetInstance().Data1.CurrentLv < lv;
        if(isLock)
        {
            foreach(G4_Star star in stars)
            {
                star.gameObject.SetActive(false);
            }
            buttonImg.sprite=LockSprite;
            return; 
        }
        buttonImg.sprite = UnLockSprite;
        foreach (G4_Star star in stars)
        {
            star.gameObject.SetActive(true);
        }
        btn.onClick.AddListener(() => TaskOnClick(lv));
        if (G4_SaveLoadManager.GetInstance().Data1.StarNumbers.Count >= lv)
        {
            for(int i = 0; i < 3; i++)
            {
               
                if (G4_SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] > i)
                {
                    stars[i].PlayAnim("Appear");
                }
            }
           
        }
    }
    public void TaskOnClick(int lv)
    {
        //GameController.GetInstance().ChooseLevel(lv);
        //UIManager.GetInstance().CloseUI<UIMainMenu>();
        G4_UIManager.GetInstance().OpenUI<G4_UILevelInformation>();
        G4_UIManager.GetInstance().GetUI<G4_UILevelInformation>().SetUp(lv);
    }
   
}
