using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract  class CharacterViewModel : MonoBehaviour
{
    protected CharacterModel characterModel;
    public CharacterView characterView { get; private set; }
    protected NavMeshAgent agent;
    protected CharacterViewModel attackTarget;

    protected virtual void Awake()
    {
        characterModel = GetComponent<CharacterModel>();
        characterView = GetComponent<CharacterView>();
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void TakeDamage(int amount)
    {
        characterModel.TakeDamage(amount);
     //   characterView.PlayDamageAnimation();
        if (characterModel.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        characterModel.Heal(amount);
    }

    public virtual void FullHeal()
    {
        characterModel.FullHeal();
    }

    public virtual void Attack(CharacterViewModel target)
    {
        attackTarget = target;
        characterView.PlayAttackAnimation();
    }

    // 애니메이션 이벤트로 호출될 메서드
    public void ApplyDamage()
    {
        
        if (attackTarget != null)
        {
            attackTarget.TakeDamage(characterModel.GetAttackPower());
        }
        
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    protected abstract void Die();
}
