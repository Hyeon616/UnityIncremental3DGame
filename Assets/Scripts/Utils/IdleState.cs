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
        // Idle 상태에서 할 일 (예: 적을 찾기)
        character.FindTarget();
    }

    public void Exit()
    {
        // Idle 상태에서 나갈 때 할 일 (특별한 작업이 없다면 비워둠)
    }
}
