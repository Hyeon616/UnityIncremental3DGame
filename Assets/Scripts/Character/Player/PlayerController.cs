using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitySingleton<PlayerController>
{

    [SerializeField] private Projectile Projectile;
    [SerializeField] private Transform ProjectileSpawnPoint;
    private int projectilePoolSize = 15;
    private ObjectPool<Projectile> _projectilePool;

    public GameObject NearestMonster { get; set; }
    public bool IsInitialized { get; private set; }
    private NavMeshAgent _agent;
    private PlayerBehaviorTree _behaviorTree;
    private Animator _animator;
    private float _attackRange = 10f;
    private float _attackCooldown = 1f;
    private float lastAttackTime;

    //public event Action<int> OnHealthChanged;
    private Vector3 _startPos;


    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");

    private bool isAttacking = false;

    // 게임 종료
    public static bool IsQuitting { get; private set; }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            _agent = gameObject.AddComponent<NavMeshAgent>();
        }
        _behaviorTree = new PlayerBehaviorTree(this);
        IsInitialized = true;

        _animator = GetComponent<Animator>();

        GameLogic.Instance.InitializePlayerHealth();

        _projectilePool = new ObjectPool<Projectile>(Projectile, projectilePoolSize, transform);

        _startPos = transform.position;
    }

    void Update()
    {
        _behaviorTree.Update();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (_animator != null)
        {
            bool isMoving = _agent.velocity.magnitude > 0.1f;
            _animator.SetBool(IsMoving, isMoving);
            _animator.SetBool(IsIdle, !isMoving && !isAttacking);
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
        if (!isAttacking && monster != null && _agent != null && _agent.isActiveAndEnabled)
        {
            _agent.isStopped = false;
            _agent.SetDestination(monster.transform.position);
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
        if (distanceToMonster <= _attackRange)
        {
            if (Time.time - lastAttackTime >= _attackCooldown)
            {
                lastAttackTime = Time.time;
                if (_animator != null)
                {
                    
                    Vector3 directionToMonster = (monster.transform.position - transform.position).normalized;
                    directionToMonster.y = 0; 
                    transform.forward = directionToMonster;

                    isAttacking = true;
                    _agent.isStopped = true;
                    _animator.SetTrigger(Attack);
                    ResetAttackState().Forget();
                }
                return true;
            }
        }
        return false;
    }

    private async UniTaskVoid ResetAttackState()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown));
        isAttacking = false;
        _agent.isStopped = false;
    }

    // AnimationEvent
    public void LaunchProjectile()
    {
        if (ProjectileSpawnPoint != null && NearestMonster != null)
        {
            int damage = GameLogic.Instance.CurrentPlayer.attributes.attack_power;
            Projectile projectile = _projectilePool.GetObject();
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
        if(_agent != null)
        {
            _agent.isStopped = true;
            _agent.ResetPath();

        }

        if (_animator != null)
        {
            _animator.SetBool(IsIdle, true);
            _animator.SetBool(IsMoving, false);
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
        RestartCurrentStage();
    }

    private void RestartCurrentStage()
    {
        transform.position = _startPos;
        _agent.ResetPath();

        GameLogic.Instance.RestorePlayerHealth();

        StageManager.Instance.RestartCurrentStage();
        MonsterSpawner.Instance.SpawnMonstersForCurrentStage();

        isAttacking = false;
        _animator.SetBool(IsMoving, false);
        _animator.SetBool(IsIdle, true);

        foreach (var monsterObject in MonsterSpawner.Instance.CurrentMonsterObjects)
        {
            MonsterController controller = monsterObject.GetComponent<MonsterController>();
            if (controller != null)
            {
                controller.ResetState();
            }
        }

    }

    private void OnApplicationQuit()
    {
        IsQuitting = true;
    }


}
