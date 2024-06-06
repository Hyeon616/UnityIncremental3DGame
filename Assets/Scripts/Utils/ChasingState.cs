using UnityEngine;

public class ChasingState : ICharacterState
{
    private readonly CharacterViewModel _character;
    private readonly Transform _target;
    private readonly float _detectionRange;
    private readonly float _attackRange;

    public ChasingState(CharacterViewModel character, Transform target, float detectionRange, float attackRange)
    {
        _character = character;
        _target = target;
        _detectionRange = detectionRange;
        _attackRange = attackRange;
    }

    public void Enter()
    {
        _character.CharacterView.Animator.SetBool("isWalking", true);
        _character.CharacterView.Animator.SetBool("isAttacking", false);
        _character.Agent.isStopped = false;
    }

    public void Execute()
    {
        if (_target == null)
        {
            _character.ChangeState(new IdleState(_character, _detectionRange, _attackRange));
            return;
        }

        float distanceToTarget = Vector3.Distance(_character.transform.position, _target.position);

        if (distanceToTarget <= _attackRange)
        {
            _character.ChangeState(new AttackingState(_character, _attackRange));
        }
        else
        {
            _character.Agent.SetDestination(_target.position);
        }
    }

    public void Exit()
    {
        _character.CharacterView.Animator.SetBool("isWalking", false);
    }
}
