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
    private PlayerViewModel playerViewModel;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerViewModel = GetComponent<PlayerViewModel>();
        agent.stoppingDistance = attackRange; // Stopping distance¸¦ ¼³Á¤
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
                StopAndAttack();
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
            playerViewModel.characterView.Animator.SetBool("isWalking", true);
            playerViewModel.characterView.Animator.SetBool("isAttacking", false);
            agent.isStopped = false;
        }
    }

    void StopAndAttack()
    {
        agent.isStopped = true;
        playerViewModel.characterView.Animator.SetBool("isWalking", false);
        if (attackTimer <= 0f)
        {
            transform.LookAt(target.transform);
            MonsterViewModel targetViewModel = target.GetComponent<MonsterViewModel>();
            if (targetViewModel != null)
            {
                playerViewModel.Attack(targetViewModel);
            }
            attackTimer = attackCooldown;
        }
    }

    void Idle()
    {
        playerViewModel.characterView.Animator.SetBool("isWalking", false);
        playerViewModel.characterView.Animator.SetBool("isAttacking", false);
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    
}
