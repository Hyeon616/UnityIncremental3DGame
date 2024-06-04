using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterViewModel : MonoBehaviour
{
    protected CharacterModel characterModel;
    public CharacterModel CharacterModel => characterModel;

    public CharacterView characterView { get; private set; }
    public GameObject target { get; set; }
    public float attackTimer = 0f;
    protected ICharacterState currentState;
    private List<Node> path;
    private int targetIndex;

    protected virtual void Awake()
    {
        characterModel = GetComponent<CharacterModel>();
        characterView = GetComponent<CharacterView>();
    }

    protected virtual void Start()
    {
        ChangeState(new IdleState(this));
    }

    public void SetPath(List<Node> path)
    {
        this.path = path;
        targetIndex = 0;
    }

    public void MoveAlongPath()
    {
        if (path == null || targetIndex >= path.Count)
        {
            return;
        }

        Vector3 targetPosition = path[targetIndex].worldPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetIndex++;
        }
    }

    public void MoveTo(Vector3 destination)
    {
        path = new List<Node> { new Node(true, destination, 0, 0) }; // 임시 노드 경로 설정
        targetIndex = 0;
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

        MoveAlongPath();
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

        if (target != null)
        {
            CharacterViewModel targetViewModel = target.GetComponent<CharacterViewModel>();
            if (targetViewModel != null)
            {
                Debug.Log($"{target} hit");
                targetViewModel.TakeDamage(characterModel.GetAttackPower());
            }
        }
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
