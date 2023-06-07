using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevel : MonoBehaviour
{
    Button btn;
    [SerializeField] Text txtLevel;
    [SerializeField] List<Star> stars;
    [SerializeField] Image LockImg;
    private bool isLock = true;
    public void SetUp(int lv)
    {
        btn = GetComponent<Button>();
        txtLevel.text = lv.ToString();
        isLock = SaveLoadManager.GetInstance().Data1.CurrentLv < lv;
        if(isLock)
        {
            foreach(Star star in stars)
            {
                star.gameObject.SetActive(false);
            }
            LockImg.gameObject.SetActive(true);
            return; 
        }
        LockImg.gameObject.SetActive(false);
        foreach (Star star in stars)
        {
            star.gameObject.SetActive(true);
        }
        btn.onClick.AddListener(() => TaskOnClick(lv));
        if (SaveLoadManager.GetInstance().Data1.StarNumbers.Count >= lv)
        {
            for(int i = 0; i < 3; i++)
            {
               
                if (SaveLoadManager.GetInstance().Data1.StarNumbers[lv - 1] > i)
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
        UIManager.GetInstance().OpenUI<UILevelInformation>();
        UIManager.GetInstance().GetUI<UILevelInformation>().SetUp(lv);
    }
   
}
