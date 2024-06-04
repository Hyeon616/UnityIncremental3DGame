using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : ICharacterState
{
    private readonly CharacterViewModel character;

    public ChasingState(CharacterViewModel character)
    {
        this.character = character;
    }

    public void Enter()
    {
        character.characterView.Animator.SetBool("isWalking", true);
        character.characterView.Animator.SetBool("isAttacking", false);
        character.agent.isStopped = false;
    }

    public void Execute()
    {
        // 목표를 향해 이동
        if (character.target != null)
        {
            character.agent.SetDestination(character.target.transform.position);
            if (Vector3.Distance(character.transform.position, character.target.transform.position) <= character.CharacterModel.AttackRange)
            {
                character.ChangeState(new AttackingState(character));
            }
        }
        else
        {
            character.ChangeState(new IdleState(character));
        }
    }

    public void Exit()
    {
        // Chasing 상태에서 나갈 때 할 일 (예: 이동을 멈추기)
        character.agent.isStopped = true;
    }
}
