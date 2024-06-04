using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour, IDamageable
{
    [Header("Combat")]
    private float detectionRange = 20f;
    private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackPower = 1;
    private float attackTimer = 0f;

    [Header("Health")]
    private int maxHealth = 10;
    private int currentHealth;

    [Header("Move")]
    [SerializeField] private float wanderRadius = 20f;
    [SerializeField] private float wanderTimer = 5f;
    private float wanderTimerCounter;

    private NavMeshAgent agent;
    private GameObject player;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ResetHealth();
        wanderTimerCounter = wanderTimer;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform.gameObject;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        attackTimer -= Time.deltaTime;
        wanderTimerCounter -= Time.deltaTime;

        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            ChaseOrAttackPlayer();
        }
        else if (wanderTimerCounter <= 0f)
        {
            Wander();
            wanderTimerCounter = wanderTimer;
        }
    }
    void ChaseOrAttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= attackRange)
        {
            Attack();
        }
        else
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.transform.position);
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void Attack()
    {
        if (attackTimer <= 0f)
        {
            transform.LookAt(player.transform);
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            attackTimer = attackCooldown;
        }
    }

    void Wander()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void Idle()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
    }

    public void ApplyDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            if (player.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(GetAttackPower());
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("TakeDamage");
        }
    }

    public int GetAttackPower()
    {
        return attackPower;
    }

    void Die()
    {
        Debug.Log("Monster died.");
        gameObject.SetActive(false);
    }
}
