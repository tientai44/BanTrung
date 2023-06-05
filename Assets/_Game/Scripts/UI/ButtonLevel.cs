using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevel : MonoBehaviour
{
    Button btn;
    [SerializeField] Text txtLevel;
 

    public void SetUp(int lv)
    {
        btn = GetComponent<Button>();
        txtLevel.text = lv.ToString();
        btn.onClick.AddListener(()=>TaskOnClick(lv));
    }
    public void TaskOnClick(int lv)
    {
        GameController.GetInstance().ChooseLevel(lv);
        UIManager.GetInstance().CloseUI<UIMainMenu>();
    }
   
}
