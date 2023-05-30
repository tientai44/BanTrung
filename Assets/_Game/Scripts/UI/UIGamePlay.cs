using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : UICanvas
{
    [SerializeField] private Text scroreText;

    public void SetScoreText(int score)
    {
        scroreText.text = score.ToString();
    }
}
