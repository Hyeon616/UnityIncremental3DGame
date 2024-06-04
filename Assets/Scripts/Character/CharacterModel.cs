using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterModel : MonoBehaviour, IDamageable
{
    protected int maxHealth;
    protected int attackPower;
    protected float attackRange;
    protected float attackCooldown;
    protected int currentHealth;
    protected Animator animator;

    public int CurrentHealth
    {
        get => currentHealth;
        protected set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        CurrentHealth += amount;
    }

    public virtual void FullHeal()
    {
        CurrentHealth = maxHealth;
    }

    public int GetAttackPower()
    {
        return attackPower;
    }

    protected abstract void Die();
}
