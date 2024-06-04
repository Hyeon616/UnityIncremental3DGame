using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private float detectionRange = 20f;
    private float attackRange = 5f;
    private float attackCooldown = 3f;
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private GameObject player;
    private MonsterViewModel monsterViewModel;
    private PlayerViewModel playerViewModel;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterViewModel = GetComponent<MonsterViewModel>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerViewModel = player.GetComponent<PlayerViewModel>();
        agent.stoppingDistance = attackRange; 
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            ChaseOrAttackPlayer();
        }
        else
        {
            Wander();
        }
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
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
            monsterViewModel.characterView.Animator.SetBool("isWalking", true);
            monsterViewModel.characterView.Animator.SetBool("isAttacking", false);
            agent.isStopped = false;
        }
    }

    void StopAndAttack()
    {
        agent.isStopped = true;
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
        Vector3 newPos = MonsterViewModel.RandomNavSphere(transform.position, 20f, -1);
        agent.SetDestination(newPos);
        monsterViewModel.characterView.Animator.SetBool("isWalking", true);
        monsterViewModel.characterView.Animator.SetBool("isAttacking", false);
    }

    void Idle()
    {
        monsterViewModel.characterView.Animator.SetBool("isWalking", false);
        monsterViewModel.characterView.Animator.SetBool("isAttacking", false);
    }
}
