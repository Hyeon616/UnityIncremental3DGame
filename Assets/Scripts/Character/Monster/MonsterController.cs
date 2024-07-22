using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public MonsterModel Model { get; private set; }
    private NavMeshAgent agent;
    private MonsterBehaviorTree behaviorTree;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    public void Initialize(MonsterModel model)
    {
        this.Model = model;
        agent = GetComponent<NavMeshAgent>();
        behaviorTree = new MonsterBehaviorTree(this);
    }

    private void Update()
    {
        if (PlayerController.Instance != null)
        {
            behaviorTree.Update();
        }
    }

    public bool IsPlayerInRange()
    {
        if (PlayerController.Instance == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        return distanceToPlayer <= attackRange;
    }

    public void AttackPlayer()
    {
        if (PlayerController.Instance == null) return;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            int damage = Model.Attack;
            PlayerController.Instance.TakeDamage(damage);
            Debug.Log($"Monster {Model.Name} attacks player for {damage} damage!");
        }
    }

    public void ChasePlayer()
    {
        if (PlayerController.Instance == null) return;

        agent.SetDestination(PlayerController.Instance.transform.position);
    }

    public void TakeDamage(int damage)
    {
        Model.CurrentHealth -= damage;
        if (Model.CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        MonsterManager.Instance.DefeatCurrentMonster();
        Destroy(gameObject);
        // 추가적인 몬스터 사망 처리 로직
    }

}
