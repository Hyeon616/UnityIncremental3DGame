using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterViewModel : MonoBehaviour
{
    [SerializeField] private CharacterView characterView;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private CharacterModel characterModel;

    private ICharacterState currentState;
    private float attackTimer;

    public ICharacterState CurrentState => currentState;
    public CharacterView CharacterView => characterView;
    public NavMeshAgent Agent => agent;
    public Transform Target
    {
        get => target;
        set => target = value;
    }

    public CharacterModel CharacterModel => characterModel;
    public float AttackTimer
    {
        get => attackTimer;
        set => attackTimer = value;
    }

    public void SetState(ICharacterState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    public void ChangeState(ICharacterState newState)
    {
        SetState(newState);
    }

    public virtual void Update()
    {
        currentState?.Execute();
    }

    protected abstract void Die();

    public Transform FindTarget(float detectionRange, string targetTag)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.transform;
                }
            }
        }

        return closestTarget;
    }

    public void ApplyDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, characterModel.AttackRange);
        int targetsHit = 0;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster"))
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(characterModel.AttackPower);
                    targetsHit++;
                    if (targetsHit >= 3) break; // 최대 3마리의 타겟에만 데미지를 줌
                }
            }
        }

        Debug.Log("ApplyDamage called");
    }

    // 애니메이션 이벤트에서 호출될 메서드
    public void OnAttackAnimationEnd()
    {
        if (currentState is AttackingState)
        {
            currentState.Exit();
            SetState(new IdleState(this, 100f, characterModel.AttackRange));
        }
    }
}
