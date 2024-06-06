using UnityEngine;

public class MonsterViewModel : CharacterViewModel
{
    private Transform _player;
    public override string TargetTag => "Player";
    public bool HasTarget => _player != null;

    public void SetPlayer(Transform playerTransform)
    {
        _player = playerTransform;
    }

    public void Initialize(Transform playerTransform)
    {
        SetPlayer(playerTransform);
        SetState(new IdleState(this, CharacterModel.DetectionRange, CharacterModel.AttackRange));
    }

    public void MoveTo(Vector3 destination)
    {
        if (Agent != null)
        {
            Agent.SetDestination(destination);
        }
    }

    public override void ApplyDamage()
    {
        base.ApplyDamage(); // 기본 처리만 합니다.
    }

    public new void FullHeal()
    {
        CharacterModel.FullHeal();
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
        // 추가적인 몬스터 사망 처리 로직
    }

    public override void Update()
    {
        base.Update();
        if (_player != null && Vector3.Distance(transform.position, _player.position) <= CharacterModel.DetectionRange)
        {
            MoveTo(_player.position);
        }
    }
}
