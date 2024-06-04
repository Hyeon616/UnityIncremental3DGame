using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private float detectionRange = 20f;
    private float attackRange = 5f;
    private float attackCooldown = 3f;
    private float attackTimer = 0f;

    private float wanderRadius = 20f;
    private float wanderTimer = 5f;
    private float wanderTime;

    private GameObject player;
    private MonsterViewModel monsterViewModel;
    private PlayerViewModel playerViewModel;
    private JPSBPathfinding pathfinding;

    private List<Node> path;
    private int pathIndex = 0;

    private float mapSize = 50f;

    void Start()
    {
        monsterViewModel = GetComponent<MonsterViewModel>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerViewModel = player.GetComponent<PlayerViewModel>();
        pathfinding = GetComponent<JPSBPathfinding>();

        pathfinding.grid = FindObjectOfType<Grid>();
        if (pathfinding.grid == null)
        {
            Debug.LogError("Grid not found in the scene. Please ensure there is a Grid object in the scene.");
        }

        pathfinding.seeker = transform;
        wanderTime = wanderTimer;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        wanderTime -= Time.deltaTime;

        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            ChaseOrAttackPlayer();
        }
        else
        {
            if (wanderTime <= 0f)
            {
                Wander();
                wanderTime = wanderTimer;
            }
        }

        MoveAlongPath();
    }

    void ChaseOrAttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= attackRange)
        {
            StopAndAttack();
        }
        else
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        if (player != null && pathfinding.grid != null)
        {
            pathfinding.FindPath(transform.position, player.transform.position);
            path = pathfinding.grid.path;
            pathIndex = 0;
            monsterViewModel.characterView.Animator.SetBool("isWalking", true);
            monsterViewModel.characterView.Animator.SetBool("isAttacking", false);
        }
    }

    void StopAndAttack()
    {
        monsterViewModel.characterView.Animator.SetBool("isWalking", false);
        if (attackTimer <= 0f)
        {
            transform.LookAt(player.transform);
            monsterViewModel.Attack(playerViewModel);
            attackTimer = attackCooldown;
        }
    }

    void Wander()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-wanderRadius, wanderRadius),
            0,
            Random.Range(-wanderRadius, wanderRadius)
        );

        Vector3 candidatePosition = transform.position + randomDirection;

        // 맵의 경계 안에 있는지 확인
        if (candidatePosition.x > -mapSize / 2 && candidatePosition.x < mapSize / 2 &&
            candidatePosition.z > -mapSize / 2 && candidatePosition.z < mapSize / 2)
        {
            pathfinding.FindPath(transform.position, candidatePosition);
            path = pathfinding.grid.path;
            pathIndex = 0;
            monsterViewModel.characterView.Animator.SetBool("isWalking", true);
            monsterViewModel.characterView.Animator.SetBool("isAttacking", false);
        }
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
}
