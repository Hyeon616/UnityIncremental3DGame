using UnityEngine;

public abstract class CharacterModel : MonoBehaviour, IDamageable
{
    protected int maxHealth;
    protected int currentHealth;
    protected int attackPower;
    protected float attackRange;
    protected float attackCooldown;
    protected float attackTimer;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
    }
    protected abstract void Die();

    public virtual void Attack(Transform target)
    {
        // 공격 애니메이션 시작
        // ApplyDamage는 애니메이션 이벤트에 의해 호출됨
        Debug.Log($"{name} is attacking {target.name} with {attackPower} power.");
    }

    public int AttackPower => attackPower;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
}
