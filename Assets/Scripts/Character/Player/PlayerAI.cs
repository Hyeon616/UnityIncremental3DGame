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

    private GameObject target;
    private PlayerViewModel playerViewModel;
    private JPSBPathfinding pathfinding;

    private List<Node> path;
    private int pathIndex = 0;

    void Start()
    {
        playerViewModel = GetComponent<PlayerViewModel>();
        pathfinding = GetComponent<JPSBPathfinding>();

        if (pathfinding.grid == null)
        {
            pathfinding.grid = FindObjectOfType<Grid>();
            if (pathfinding.grid == null)
            {
                Debug.LogError("Grid not found in the scene. Please ensure there is a Grid object in the scene.");
            }
        }

        pathfinding.seeker = transform;
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

        MoveAlongPath();
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
        if (target != null && pathfinding.grid != null)
        {
            pathfinding.FindPath(transform.position, target.transform.position);
            path = pathfinding.grid.path;
            pathIndex = 0;
            playerViewModel.characterView.Animator.SetBool("isWalking", true);
            playerViewModel.characterView.Animator.SetBool("isAttacking", false);
        }
    }

    void StopAndAttack()
    {
        if (attackTimer <= 0f)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= attackRange)
            {
                playerViewModel.characterView.Animator.SetBool("isWalking", false);
                playerViewModel.characterView.Animator.SetBool("isAttacking", true);
                transform.LookAt(target.transform);
                attackTimer = attackCooldown;
            }
        }
    }

    void Idle()
    {
        playerViewModel.characterView.Animator.SetBool("isWalking", false);
        playerViewModel.characterView.Animator.SetBool("isAttacking", false);
    }

    void MoveAlongPath()
    {
        if (path != null && pathIndex < path.Count)
        {
            Vector3 targetPosition = path[pathIndex].worldPosition;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);
            transform.LookAt(targetPosition);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                pathIndex++;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
