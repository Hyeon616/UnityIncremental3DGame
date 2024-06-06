using UnityEngine;

public abstract class CharacterModel : MonoBehaviour, IDamageable
{
    protected int _maxHealth;
    protected int _currentHealth;
    protected int _attackPower;
    protected float _attackRange;
    protected float _attackCooldown;
    protected float _attackTimer;

    public int AttackPower => _attackPower;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void FullHeal()
    {
        _currentHealth = _maxHealth;
    }
    protected abstract void Die();

    public virtual void Attack(Transform target)
    {
        // 공격 애니메이션 시작
        // ApplyDamage는 애니메이션 이벤트에 의해 호출됨
        Debug.Log($"{name} is attacking {target.name} with {_attackPower} power.");
    }

    public virtual int GetRandomAttackPower()
    {
        float randomMultiplier = Random.Range(0.8f, 1.2f); // 80%에서 120% 범위의 무작위 값
        return Mathf.RoundToInt(_attackPower * randomMultiplier);
    }
}
