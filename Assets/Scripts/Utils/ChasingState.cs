using UnityEngine;

public class ChasingState : ICharacterState
{
    private readonly CharacterViewModel character;
    private readonly Transform player;
    private readonly float detectionRange;
    private readonly float attackRange;

    public ChasingState(CharacterViewModel character, Transform player, float detectionRange, float attackRange)
    {
        this.character = character;
        this.player = player;
        this.detectionRange = detectionRange;
        this.attackRange = attackRange;
    }

    public void Enter()
    {
        character.CharacterView.Animator.SetBool("isWalking", true);
        character.CharacterView.Animator.SetBool("isAttacking", false);
        character.Agent.isStopped = false;
    }

    public void Execute()
    {
        if (player == null)
        {
            character.ChangeState(new IdleState(character, detectionRange, attackRange));
            return;
        }

        float distanceToPlayer = Vector3.Distance(character.transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            character.ChangeState(new AttackingState(character, player, detectionRange, attackRange));
        }
        else
        {
            character.Agent.SetDestination(player.position);
        }
    }

    public void Exit()
    {
        character.CharacterView.Animator.SetBool("isWalking", false);
    }
}
