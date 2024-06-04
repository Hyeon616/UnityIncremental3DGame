using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterModel : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int attackPower;
    protected int currentHealth;
    protected Animator animator;

    public int CurrentHealth
    {
        get => currentHealth;
        protected set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

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
        else
        {
            animator.SetTrigger("TakeDamage");
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
