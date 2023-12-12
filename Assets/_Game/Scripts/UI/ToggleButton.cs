using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] Sprite spriteOn;
    [SerializeField] Sprite spriteOff;

    public Button Button { get => button;}

    private void OnValidate()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void Switch(bool isOn)
    {
        if(isOn)
            image.sprite = spriteOn;
        else
            image.sprite = spriteOff;
    }
    
}
