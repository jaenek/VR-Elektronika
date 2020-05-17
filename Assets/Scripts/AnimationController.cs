using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    public GameObject animatingBody;

    public void Start()
    {
        animator = animatingBody.GetComponent<Animator>();
    }

    public void SwitchState(bool state)
    {
        animator.SetBool("On", state);
    }
}
