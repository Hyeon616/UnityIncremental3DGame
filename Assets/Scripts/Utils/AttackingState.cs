using UnityEngine;

public class AttackingState : ICharacterState
{
    private readonly CharacterViewModel _character;
    private readonly float _attackRange;
    private readonly float _attackCooldown;
    private float _attackTimer;

    public AttackingState(CharacterViewModel character, float attackRange)
    {
        _character = character;
        _attackRange = attackRange;
        _attackCooldown = character.CharacterModel.AttackCooldown;
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
        if (_character.Target == null)
        {
            _character.ChangeState(new IdleState(_character, _character.CharacterModel.DetectionRange, _attackRange));
            return;
        }

        float distanceToTarget = Vector3.Distance(_character.transform.position, _character.Target.position);
        if (distanceToTarget > _attackRange)
        {
            _character.ChangeState(new ChasingState(_character, _character.Target, _character.CharacterModel.DetectionRange, _attackRange));
            return;
        }

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            _character.transform.LookAt(_character.Target.transform);
            _attackTimer = _attackCooldown;
        }

        if (_character.FindTarget(_character.CharacterModel.DetectionRange, _character.TargetTag) == null)
        {
            _character.ChangeState(new IdleState(_character, _character.CharacterModel.DetectionRange, _attackRange));
        }
    }

    public void Exit()
    {
        _character.CharacterView.Animator.SetBool("isAttacking", false);
    }
}
