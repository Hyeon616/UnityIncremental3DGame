using UnityEngine;

public class MonsterViewModel : CharacterViewModel
{
    private Transform _player;

    public override string TargetTag => "Player";

    public void SetPlayer(Transform playerTransform)
    {
        _player = playerTransform;
    }

    public void MoveTo(Vector3 destination)
    {
        if (Agent != null)
        {
            Agent.SetDestination(destination);
        }
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
        // 추가적인 몬스터 사망 처리 로직
    }

    public override void Update()
    {
        base.Update();
        if (_player != null && Vector3.Distance(transform.position, _player.position) <= 100f)
        {
            MoveTo(_player.position);
        }
    }

    public override void ApplyDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, CharacterModel.AttackRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(TargetTag))
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int damage = CharacterModel.GetRandomAttackPower();
                    damageable.TakeDamage(damage);
                    break;
                }
            }
        }

        Debug.Log("ApplyDamage called");
    }
}
