using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour, IDamage
{
    private float detectionRange = 100f;
    private float attackRange = 5f;
    private float attackCooldown = 1f;
    private float attackTimer = 0f;


    [SerializeField] private float enemyHealth;

    private NavMeshAgent agent;
    private GameObject target;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        FindTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= attackRange)
            {
                Attack();
            }
            else
            {
                ChaseTarget();
            }
        }
        else
        {
            Idle();
        }
    }

    void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                target = hitCollider.gameObject;
                return;
            }
        }
        target = null;
    }

    void ChaseTarget()
    {
        transform.position = Vector3.zero;
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
        agent.SetDestination(target.transform.position);
    }

    void Attack()
    {
        if (attackTimer <= 0f)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            transform.LookAt(target.transform);
            attackTimer = attackCooldown;
        }
    }

    void Idle()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void Damage(float damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            // 몬스터 죽음 처리
        }
    }
}
