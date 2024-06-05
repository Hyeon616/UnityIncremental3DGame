using UnityEngine;

public class ChasingState : ICharacterState
{
    private readonly CharacterViewModel _character;
    private readonly Transform _player;
    private readonly float _detectionRange;
    private readonly float _attackRange;

    public ChasingState(CharacterViewModel character, Transform player, float detectionRange, float attackRange)
    {
        this._character = character;
        this._player = player;
        this._detectionRange = detectionRange;
        this._attackRange = attackRange;
    }

    public void Enter()
    {
        _character.CharacterView.Animator.SetBool("isWalking", true);
        _character.CharacterView.Animator.SetBool("isAttacking", false);
        _character.Agent.isStopped = false;
    }

    public void Execute()
    {
        if (_player == null)
        {
            _character.ChangeState(new IdleState(_character, _detectionRange, _attackRange));
            return;
        }

        float distanceToPlayer = Vector3.Distance(_character.transform.position, _player.position);

        if (distanceToPlayer <= _attackRange)
        {
            _character.ChangeState(new AttackingState(_character, _player, _detectionRange, _attackRange));
        }
        else
        {
            _character.Agent.SetDestination(_player.position);
        }
    }

    public void Exit()
    {
        _character.CharacterView.Animator.SetBool("isWalking", false);
    }
}
