using UnityEngine;

public abstract class CharacterModel : MonoBehaviour, IDamageable
{
    protected int _maxHealth;
    protected int _currentHealth;
    protected int _attackPower;
    protected float _attackRange;
    protected float _attackCooldown;
    protected float _criticalChance;
    protected float _detectionRange;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int AttackPower => _attackPower;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    public float CriticalChance => _criticalChance;
    public float DetectionRange => _detectionRange;
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

    public virtual int GetRandomAttackPower()
    {
        float randomMultiplier = Random.Range(0.8f, 1.2f);
        return Mathf.RoundToInt(_attackPower * randomMultiplier);
    }

    public virtual bool IsCriticalHit()
    {
        return Random.value < _criticalChance;
    }

}
