using Cysharp.Threading.Tasks;
using System;
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
    
    private Animator animator;
    private static readonly int IsMoving = Animator.StringToHash("Move");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private bool isAttacking = false;

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
        animator = GetComponent<Animator>();
        behaviorTree = new MonsterBehaviorTree(this);
    }

    private void OnDisable()
    {
        behaviorTree = null;
    }

    private void Update()
    {
        behaviorTree.Update();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool(IsMoving, isMoving && !isAttacking);
        }
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


    public GameObject FindPlayer()
    {
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, 100f, playerLayerMask);

        if (playersInRange.Length > 0)
        {
            return playersInRange[0].gameObject;
        }

        return null;
    }

    public void MoveTowardsPlayer()
    {
        if (PlayerTransform != null && agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
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
                Vector3 directionToPlayer = (PlayerTransform.position - transform.position).normalized;
                directionToPlayer.y = 0;
                transform.forward = directionToPlayer;

                if (animator != null)
                {
                    isAttacking = true;
                    agent.isStopped = true;
                    animator.SetTrigger(Attack);
                    ResetAttackState().Forget();
                }

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


    private async UniTaskVoid ResetAttackState()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(attackCooldown));
        isAttacking = false;
        if(agent != null)
            agent.isStopped = false;
    }

    public void Idle()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void TakeDamage(int damage)
    {
        MonsterManager.Instance.DamageMonster(Model, damage);
    }

    public void ResetState()
    {
        if (Model != null)
        {
            Model.CurrentHealth = Model.Health;
        }
        lastAttackTime = 0f;
        isAttacking = false;
        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }
        if (animator != null)
        {
            animator.ResetTrigger(Attack);
            animator.SetBool(IsMoving, false);
        }
    }

    public float GetHealthPercentage()
    {
        return Model.GetHealthPercentage();
    }
}