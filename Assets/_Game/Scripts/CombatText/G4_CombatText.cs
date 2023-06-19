using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G4_CombatText : MonoBehaviour
{
    [SerializeField] private Text txt;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetText(string str)
    {
        txt.text = str;
    }

    public void PopUp()
    {
        animator.Play("PopUp");
    }
}
