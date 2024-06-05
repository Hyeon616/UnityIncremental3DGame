using UnityEngine;

public class AttackingState : ICharacterState
{
    private readonly CharacterViewModel character;
    private readonly Transform player;
    private readonly float detectionRange;
    private readonly float attackRange;
    private readonly float attackCooldown;
    private float attackTimer;

    public AttackingState(CharacterViewModel character, Transform player, float detectionRange, float attackRange)
    {
        this.character = character;
        this.player = player;
        this.detectionRange = detectionRange;
        this.attackRange = attackRange;
        this.attackCooldown = character.CharacterModel.AttackCooldown;
    }

    public void Enter()
    {
        character.CharacterView.Animator.SetBool("isWalking", false);
        character.CharacterView.Animator.SetBool("isAttacking", true);
        character.Agent.isStopped = true;
        attackTimer = 0f;
    }

    public void Execute()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(character.transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                character.ChangeState(new ChasingState(character, player, detectionRange, attackRange));
                return;
            }

            Collider[] hitColliders = Physics.OverlapSphere(character.transform.position, attackRange);
            bool enemyNearby = false;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Monster"))
                {
                    enemyNearby = true;
                    break;
                }
            }

            if (!enemyNearby)
            {
                character.ChangeState(new IdleState(character, detectionRange, attackRange));
                return;
            }

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                character.transform.LookAt(player.transform);
                attackTimer = attackCooldown;
            }
        }
        else
        {
            character.ChangeState(new IdleState(character, detectionRange, attackRange));
        }
    }

    public void Exit()
    {
        character.CharacterView.Animator.SetBool("isAttacking", false);
    }
}
