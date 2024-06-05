using UnityEngine;

public class IdleState : ICharacterState
{
    private readonly CharacterViewModel character;
    private readonly float detectionRange;
    private readonly float attackRange;

    public IdleState(CharacterViewModel character, float detectionRange, float attackRange)
    {
        this.character = character;
        this.detectionRange = detectionRange;
        this.attackRange = attackRange;
    }

    public void Enter()
    {
        character.CharacterView.Animator.SetBool("isWalking", false);
        character.CharacterView.Animator.SetBool("isAttacking", false);
        character.Agent.isStopped = true;
    }

    public void Execute()
    {
        Transform target = character.FindTarget(detectionRange, "Player");
        if (target != null)
        {
            character.Target = target;
            character.ChangeState(new ChasingState(character, target, detectionRange, attackRange));
        }
    }

    public void Exit()
    {
        // 대기 상태 종료 시 로직
    }
}
