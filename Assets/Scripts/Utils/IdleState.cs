using UnityEngine;

public class IdleState : ICharacterState
{
    private readonly CharacterViewModel _character;
    private readonly float _detectionRange;
    private readonly float _attackRange;

    public IdleState(CharacterViewModel character, float detectionRange, float attackRange)
    {
        this._character = character;
        this._detectionRange = detectionRange;
        this._attackRange = attackRange;
    }

    public void Enter()
    {
        _character.CharacterView.Animator.SetBool("isWalking", false);
        _character.CharacterView.Animator.SetBool("isAttacking", false);
        _character.Agent.isStopped = true;
    }

    public void Execute()
    {
        Transform target = _character.FindTarget(_detectionRange, "Player");
        if (target != null)
        {
            _character.Target = target;
            _character.ChangeState(new ChasingState(_character, target, _detectionRange, _attackRange));
        }
    }

    public void Exit()
    {
        // 대기 상태 종료 시 로직
    }
}
