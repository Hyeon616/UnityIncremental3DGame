using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public MonsterModel Model { get; private set; }
    private NavMeshAgent agent;
    private MonsterBehaviorTree behaviorTree;
    public float attackRange = 5f;
    public float attackCooldown = 3f;
    private float lastAttackTime;
    private Transform _playerTransform;
    private float updatePlayerPositionInterval = 0.5f; 
    private float lastUpdateTime;


    private Transform PlayerTransform
    {
        get
        {
            if (_playerTransform == null)
            {
                UpdatePlayerPosition();
            }
            return _playerTransform;
        }
    }

    public void Initialize(MonsterModel model)
    {
        this.Model = model;
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.speed = 3.5f;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 2f;

        if (GetComponent<Collider>() == null)
        {
            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0, 1, 0);
        }

        behaviorTree = new MonsterBehaviorTree(this);
    }

    private void OnDisable()
    {
        behaviorTree = null;
    }

    private void Update()
    {
        if (behaviorTree == null)
            return;
        if (!agent.isOnNavMesh)
        {
            PlaceOnNavMesh();
        }

        if (Time.time - lastUpdateTime > updatePlayerPositionInterval)
        {
            UpdatePlayerPosition();
            lastUpdateTime = Time.time;
        }

        behaviorTree.Update(Time.deltaTime);
    }

    private void UpdatePlayerPosition()
    {
        var player = FindPlayer();
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            _playerTransform = null;
        }
    }

    public Transform GetPlayerTransform()
    {
        return PlayerTransform;
    }


    private void PlaceOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    public GameObject FindPlayer()
    {
        if (PlayerController.IsQuitting)
        {
            return null;
        }

        if (PlayerController.Instance == null || PlayerController.Instance.gameObject == null)
        {
            Debug.LogWarning("PlayerController is null or has been destroyed.");
            return null;
        }
        return PlayerController.Instance.gameObject;
    }

    public void MoveTowardsPlayer()
    {
        if (PlayerTransform != null && agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(PlayerTransform.position);
        }
        else
        {
            Idle();
        }
    }

    public bool AttackPlayer()
    {
        if (PlayerTransform == null || !PlayerTransform.gameObject.activeInHierarchy)
        {
            return false;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                int damage = Model.Attack;
                PlayerController playerController = PlayerTransform.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                    return true;
                }
            }
        }
        return false;
    }

    public void Idle()
    {
        if (agent != null)
        {
            agent.ResetPath();
        }
    }

    public void TakeDamage(int damage)
    {
        MonsterManager.Instance.DamageMonster(Model, damage);
    }


    public float GetHealthPercentage()
    {
        return Model.GetHealthPercentage();
    }
}