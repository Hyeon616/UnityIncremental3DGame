using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : ICharacterState
{
    private readonly CharacterViewModel character;

    public IdleState(CharacterViewModel character)
    {
        this.character = character;
    }

    public void Enter()
    {
        character.characterView.Animator.SetBool("isWalking", false);
        character.characterView.Animator.SetBool("isAttacking", false);
        character.agent.isStopped = true;
    }

    public void Execute()
    {
        // Idle ���¿��� �� �� (��: ���� ã��)
        character.FindTarget();
    }

    public void Exit()
    {
        // Idle ���¿��� ���� �� �� �� (Ư���� �۾��� ���ٸ� �����)
    }
}
