using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : ICharacterState
{
    private readonly CharacterViewModel character;

    public AttackingState(CharacterViewModel character)
    {
        this.character = character;
    }

    public void Enter()
    {
        character.characterView.Animator.SetBool("isWalking", false);
        character.characterView.Animator.SetBool("isAttacking", true);
        character.agent.isStopped = true;
    }

    public void Execute()
    {
        // 공격 로직
        if (character.attackTimer <= 0f)
        {
            if (character.target != null)
            {
                character.transform.LookAt(character.target.transform);
                character.ApplyDamage();
                character.attackTimer = character.CharacterModel.AttackCooldown;
            }
        }
        character.attackTimer -= Time.deltaTime;

        // 적이 범위를 벗어났는지 확인
        if (character.target == null || Vector3.Distance(character.transform.position, character.target.transform.position) > character.CharacterModel.AttackRange)
        {
            character.ChangeState(new ChasingState(character));
        }
    }

    public void Exit()
    {
        // Attacking 상태에서 나갈 때 할 일 (예: 공격 애니메이션 멈추기)
        character.characterView.Animator.SetBool("isAttacking", false);
    }
}
