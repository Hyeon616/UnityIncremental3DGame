using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitySingleton<PlayerController>
{

    [SerializeField] private Projectile Projectile;
    [SerializeField] private Transform ProjectileSpawnPoint;
    [SerializeField] private int initialPoolSize = 10;
    private ObjectPool<Projectile> projectilePool;

    public GameObject NearestMonster { get; set; }
    public bool IsInitialized { get; private set; }
    private NavMeshAgent agent;
    private PlayerBehaviorTree behaviorTree;
    private Animator animator;
    public float attackRange = 10f;
    public float attackCooldown = 1f;
    private float lastAttackTime;


    public event Action<int> OnHealthChanged;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");

    private bool isAttacking = false;

    // 게임 종료
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

        animator = GetComponent<Animator>();

        GameLogic.Instance.InitializePlayerHealth();

        projectilePool = new ObjectPool<Projectile>(Projectile, initialPoolSize, transform);


    }

    void Update()
    {
        behaviorTree.Update();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool(IsMoving, isMoving);
            animator.SetBool(IsIdle, !isMoving && !isAttacking);
        }
    }

    #region BT


   
    public GameObject FindNearestMonster()
    {
        int monsterLayerMask = 1 << LayerMask.NameToLayer("Monster");
        Collider[] monstersInRange = Physics.OverlapSphere(transform.position, 100f, monsterLayerMask);

        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider monsterCollider in monstersInRange)
        {
            float distance = Vector3.Distance(transform.position, monsterCollider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = monsterCollider.gameObject;
            }
        }

        return nearest;
    }

    public void MoveTowardsMonster(GameObject monster)
    {
        if (!isAttacking && monster != null && agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
            agent.SetDestination(monster.transform.position);
        }
    }

    public bool AttackMonster(GameObject monster)
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
                if (animator != null)
                {
                    // 몬스터를 향해 즉시 회전
                    Vector3 directionToMonster = (monster.transform.position - transform.position).normalized;
                    directionToMonster.y = 0; // y축 회전을 무시하려면 이 줄을 유지하세요
                    transform.forward = directionToMonster;

                    isAttacking = true;
                    agent.isStopped = true;
                    animator.SetTrigger(Attack);
                    ResetAttackState().Forget();
                }
                return true;
            }
        }
        return false;
    }

    private async UniTaskVoid ResetAttackState()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(attackCooldown));
        isAttacking = false;
        agent.isStopped = false;
    }

    public void LaunchProjectile()
    {
        if (ProjectileSpawnPoint != null && NearestMonster != null)
        {
            int damage = GameLogic.Instance.CurrentPlayer.attributes.attack_power;
            Projectile projectile = projectilePool.GetObject();
            projectile.transform.position = ProjectileSpawnPoint.position;
            projectile.transform.rotation = Quaternion.LookRotation(NearestMonster.transform.position - ProjectileSpawnPoint.position);
            projectile.gameObject.SetActive(true);
            projectile.Initialize(damage, ReturnProjectileToPool, GameLogic.Instance.CurrentPlayer);
        }
    }

    private void ReturnProjectileToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }
    public void Idle()
    {
        if(agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();

        }

        if (animator != null)
        {
            animator.SetBool(IsIdle, true);
            animator.SetBool(IsMoving, false);
        }
    }

    #endregion BT

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
