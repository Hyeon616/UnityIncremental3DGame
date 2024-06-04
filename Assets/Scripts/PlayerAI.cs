using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    private float detectionRange = 100f;
    private float attackRange = 5f;
    private float attackCooldown = 1f;
    private float attackTimer = 0f;

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
            if (hitCollider.CompareTag("Monster"))
            {
                target = hitCollider.gameObject;
                return;
            }
        }
        target = null;
    }

    void ChaseTarget()
    {
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
            Debug.Log("Player attacks " + target.name);
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}
