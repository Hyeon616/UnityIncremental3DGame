using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract  class CharacterViewModel : MonoBehaviour
{
    protected CharacterModel characterModel;
    public CharacterModel CharacterModel => characterModel;

    public CharacterView characterView { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public GameObject target { get; set; }
    public float attackTimer = 0f;

    private ICharacterState currentState;

    protected virtual void Awake()
    {
        characterModel = GetComponent<CharacterModel>();
        characterView = GetComponent<CharacterView>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        ChangeState(new IdleState(this));
    }

    public void ChangeState(ICharacterState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    protected virtual void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    public virtual void TakeDamage(int amount)
    {
        characterModel.TakeDamage(amount);
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
        this.target = target.gameObject;
        characterView.PlayAttackAnimation();
    }

    // 애니메이션 이벤트로 호출될 메서드
    public void ApplyDamage()
    {
        Debug.Log("ApplyDamage called");
        if (target != null)
        {
            CharacterViewModel targetViewModel = target.GetComponent<CharacterViewModel>();
            if (targetViewModel != null)
            {
                Debug.Log("Applying damage to target");
                targetViewModel.TakeDamage(characterModel.GetAttackPower());
            }
        }
        else
        {
            Debug.Log("No attack target");
        }
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    protected abstract void Die();

    public void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster") && hitCollider.gameObject.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.gameObject;
                }
            }
        }

        target = closestTarget;
        if (target != null)
        {
            ChangeState(new ChasingState(this));
        }
    }
}
