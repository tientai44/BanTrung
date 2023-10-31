using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G4_Star : MonoBehaviour
{
    [SerializeField]Animator animator;

  
    public void PlayAnim(string animName)
    {
        animator.Play(animName);
    }
}
