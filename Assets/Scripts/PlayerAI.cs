using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    private float detectionRange = 100f;
    private float attackRange = 5f;
    private float attackCooldown = 2f;
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private GameObject target;
    private Animator animator;
    private Player player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
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
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster") && hitCollider.gameObject.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.gameObject;
                }
            }
        }

        target = closestTarget;
    }

    void ChaseTarget()
    {
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
    }

    void Attack()
    {
        if (attackTimer <= 0f)
        {
            transform.LookAt(target.transform);
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            attackTimer = attackCooldown;
        }
    }

    void Idle()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
    }

    public void ApplyDamage()
    {
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            if (target.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(player.GetAttackPower());
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    
}
