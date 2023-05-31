using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnim(string animName)
    {
        animator.Play(animName);
    }
}