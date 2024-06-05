using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterView : MonoBehaviour
{
    protected Animator _animator;
    protected CharacterModel _characterModel;

    public Animator Animator => _animator;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterModel = GetComponent<CharacterModel>();
    }


    public void PlayAttackAnimation()
    {
        _animator.SetBool("isAttacking", true);
    }

    public void StopAttackAnimation()
    {
        _animator.SetBool("isAttacking", false);
    }
}
