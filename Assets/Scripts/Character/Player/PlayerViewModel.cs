using UnityEngine;

public class PlayerViewModel : CharacterViewModel
{
    public override string TargetTag => "Monster";
    public override bool IsPlayer => true;

    public void FullHeal()
    {
        CharacterModel.FullHeal();
    }

    protected override void Die()
    {
        Debug.Log("Player died.");
        // 추가적인 플레이어 사망 처리 로직
    }

    public override void ApplyDamage()
    {
        if (GameObject_DamageTextPrefab == null)
        {
            Debug.LogError("GameObject_DamageTextPrefab is not set.");
            return;
        }

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
                    ShowDamage(hitCollider.transform, damage);
                    break;
                }
            }
        }

        Debug.Log("ApplyDamage called");
    }
}
