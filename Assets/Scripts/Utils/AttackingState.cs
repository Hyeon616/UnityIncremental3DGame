using UnityEngine;

public class AttackingState : ICharacterState
{
    private readonly CharacterViewModel _character;
    private readonly Transform _player;
    private readonly float _detectionRange;
    private readonly float _attackRange;
    private readonly float _attackCooldown;
    private float _attackTimer;

    public AttackingState(CharacterViewModel character, Transform player, float detectionRange, float attackRange)
    {
        this._character = character;
        this._player = player;
        this._detectionRange = detectionRange;
        this._attackRange = attackRange;
        this._attackCooldown = character.CharacterModel.AttackCooldown;
    }

    public void Enter()
    {
        _character.CharacterView.Animator.SetBool("isWalking", false);
        _character.CharacterView.Animator.SetBool("isAttacking", true);
        _character.Agent.isStopped = true;
        _attackTimer = 0f;
    }

    public void Execute()
    {
        if (_player != null)
        {
            float distanceToPlayer = Vector3.Distance(_character.transform.position, _player.position);
            if (distanceToPlayer > _attackRange)
            {
                _character.ChangeState(new ChasingState(_character, _player, _detectionRange, _attackRange));
                return;
            }

            Collider[] hitColliders = Physics.OverlapSphere(_character.transform.position, _attackRange);
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
                _character.ChangeState(new IdleState(_character, _detectionRange, _attackRange));
                return;
            }

            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f)
            {
                _character.transform.LookAt(_player.transform);
                _attackTimer = _attackCooldown;
            }
        }
        else
        {
            _character.ChangeState(new IdleState(_character, _detectionRange, _attackRange));
        }
    }

    public void Exit()
    {
        _character.CharacterView.Animator.SetBool("isAttacking", false);
    }
}
