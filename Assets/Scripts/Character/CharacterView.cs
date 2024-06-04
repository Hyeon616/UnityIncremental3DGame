using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterView : MonoBehaviour
{
    protected Animator animator;
    protected CharacterModel characterModel;

    public Animator Animator => animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        characterModel = GetComponent<CharacterModel>();
    }


    public void PlayAttackAnimation()
    {
        animator.SetBool("isAttacking", true);
    }

    public void StopAttackAnimation()
    {
        animator.SetBool("isAttacking", false);
    }
}
