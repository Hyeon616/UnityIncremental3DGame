using UnityEngine;

public class IdleState : ICharacterState
{
    private readonly CharacterViewModel _character;
    private readonly float _detectionRange;
    private readonly float _attackRange;

    public IdleState(CharacterViewModel character, float detectionRange, float attackRange)
    {
        _character = character;
        _detectionRange = detectionRange;
        _attackRange = attackRange;
    }

    public void Enter()
    {
        _character.CharacterView.Animator.SetBool("isWalking", false);
        _character.CharacterView.Animator.SetBool("isAttacking", false);
        _character.Agent.isStopped = true;
    }

    public void Execute()
    {
        Transform target = _character.FindTarget(_detectionRange, _character.TargetTag);
        if (target != null)
        {
            _character.Target = target;
            _character.ChangeState(new ChasingState(_character, target, _detectionRange, _attackRange));
        }
    }

    public void Exit()
    {
        // Exit 상태 처리
    }
}
