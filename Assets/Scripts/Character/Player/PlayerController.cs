using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitySingleton<PlayerController>
{
    private PlayerBehaviorTree behaviorTree;
    private NavMeshAgent agent;
    public Transform target;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        behaviorTree = new PlayerBehaviorTree();

        GameLogic.Instance.OnPlayerDataUpdated += OnPlayerDataUpdated;
        GameLogic.Instance.OnPlayerSkillsUpdated += OnPlayerSkillsUpdated;
    }

    private void OnDestroy()
    {
        GameLogic.Instance.OnPlayerDataUpdated -= OnPlayerDataUpdated;
        GameLogic.Instance.OnPlayerSkillsUpdated -= OnPlayerSkillsUpdated;
    }


    void Update()
    {
        behaviorTree.Update(Time.deltaTime);

        // 공격
        // AttackCurrentMonster();
    }

    // Update에서 부를 공격함수
    private void AttackCurrentMonster()
    {
        if (MonsterSpawner.Instance.CurrentMonsterObject != null)
        {
            float distanceToMonster = Vector3.Distance(transform.position, MonsterSpawner.Instance.CurrentMonsterObject.transform.position);
            if (distanceToMonster <= attackRange)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    lastAttackTime = Time.time;
                    int damage = GameLogic.Instance.CurrentPlayer.attributes.attack_power;
                    MonsterManager.Instance.DamageCurrentMonster(damage);
                    Debug.Log($"Player attacks monster for {damage} damage!");
                }
            }
            else
            {
                Debug.Log("Monster is too far to attack!");
            }
        }
        else
        {
            Debug.Log("No monster to attack!");
        }
    }

    public bool HasTarget()
    {
        return target != null;
    }

    public void ChaseTarget()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    public void ApplyBasicAttack()
    {
        var player = GameLogic.Instance.CurrentPlayer;
        // 공격
    }

    public void ApplySkill(int skillIndex)
    {
        var player = GameLogic.Instance.CurrentPlayer;
        var playerSkills = GameLogic.Instance.PlayerSkills;
        if (skillIndex < playerSkills.Count)
        {
            var skill = playerSkills[skillIndex];
            // 스킬 데미지 


        }
    }

    public void UseBasicAttack()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                // 기본 공격 로직
                Debug.Log("Using basic attack");
                lastAttackTime = Time.time;
            }
        }
    }

    public void Idle()
    {
        // 대기 상태 로직
        agent.ResetPath();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnPlayerDataUpdated()
    {
        // 업데이트 되어야할 속성

    }

    private void OnPlayerSkillsUpdated()
    {
        behaviorTree = new PlayerBehaviorTree(); // 스킬이 업데이트되면 BehaviorTree를 새로 생성합니다.
    }


}
