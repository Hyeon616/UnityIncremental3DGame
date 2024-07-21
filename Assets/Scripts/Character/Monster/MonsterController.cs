using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    private MonsterModel model;
    private NavMeshAgent agent;
    private Transform player;
    private float attackCooldown = 2f;
    private float lastAttackTime;

    public void Initialize(MonsterModel model)
    {
        this.model = model;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= agent.stoppingDistance)
        {
            // 공격 범위 내
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // 추적
            agent.SetDestination(player.position);
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        // 공격 로직 구현
        Debug.Log($"Monster {model.Name} attacks!");
    }
}
