using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitySingleton<PlayerController>
{
    public GameObject NearestMonster { get; set; }
    public bool IsInitialized { get; private set; }
    private NavMeshAgent agent;
    private PlayerBehaviorTree behaviorTree;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        behaviorTree = new PlayerBehaviorTree(this);
        IsInitialized = true;
    }

    void Update()
    {
        behaviorTree.Update(Time.deltaTime);

    }

    public GameObject FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject monster in monsters)
        {
            if (monster != null)  // null 체크 추가
            {
                float distance = Vector3.Distance(transform.position, monster.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = monster;
                }
            }
        }

        return nearest;
    }
    public void MoveTowardsMonster(GameObject monster)
    {
        if (monster != null)
        {
            agent.SetDestination(monster.transform.position);
        }
    }

    public bool AttackMonsterIfInRange(GameObject monster)
    {
        if (monster == null || !monster.activeInHierarchy)  // 추가된 체크
        {
            NearestMonster = null;  // NearestMonster 초기화
            return false;
        }

        float distanceToMonster = Vector3.Distance(transform.position, monster.transform.position);
        if (distanceToMonster <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                int damage = GameLogic.Instance.CurrentPlayer.attributes.attack_power;
                MonsterController monsterController = monster.GetComponent<MonsterController>();
                if (monsterController != null)
                {
                    monsterController.TakeDamage(damage);
                    Debug.Log($"Player attacks monster for {damage} damage!");
                    return true;
                }
            }
        }
        return false;
    }

    public void Idle()
    {
        // 대기 상태 로직
        agent.ResetPath();
    }

   

    public void TakeDamage(int damage)
    {
        // 플레이어가 데미지를 받는 로직
        GameLogic.Instance.CurrentPlayer.attributes.max_health -= damage;
        if (GameLogic.Instance.CurrentPlayer.attributes.max_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 플레이어 사망 처리 로직
        Debug.Log("플레이어 사망");
    }

}
