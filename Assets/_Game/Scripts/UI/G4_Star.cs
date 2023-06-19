using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G4_Star : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnim(string animName)
    {
        animator.Play(animName);
    }
}
