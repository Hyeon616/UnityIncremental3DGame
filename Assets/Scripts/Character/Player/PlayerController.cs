using System;
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

    public event Action<int> OnHealthChanged;

    public static bool IsQuitting { get; private set; }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        behaviorTree = new PlayerBehaviorTree(this);
        IsInitialized = true;
        GameLogic.Instance.InitializePlayerHealth();
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
        if (monster != null && agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(monster.transform.position);
        }
    }

    public bool AttackMonsterIfInRange(GameObject monster)
    {
        if (monster == null || !monster.activeInHierarchy)
        {
            NearestMonster = null;
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
        if(agent != null)
        {
            agent.ResetPath();

        }
    }

   

    public void TakeDamage(int damage)
    {
        int newHealth = GameLogic.Instance.CurrentPlayerHealth - damage;
        GameLogic.Instance.UpdatePlayerHealth(newHealth);
        if (newHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 플레이어 사망 처리 로직
        Debug.Log("플레이어 사망");
    }

    private void OnApplicationQuit()
    {
        IsQuitting = true;
    }


}
