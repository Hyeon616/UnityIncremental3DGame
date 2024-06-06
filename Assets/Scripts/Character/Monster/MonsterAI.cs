using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private float detectionRange = 100f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 3f;
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private MonsterViewModel monsterViewModel;
    private Transform playerTransform;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterViewModel = GetComponent<MonsterViewModel>();
        playerTransform = FindObjectOfType<PlayerViewModel>().transform;

        monsterViewModel.SetState(new IdleState(monsterViewModel, detectionRange, attackRange));
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                monsterViewModel.ChangeState(new AttackingState(monsterViewModel, attackRange));
            }
            else if (distanceToPlayer <= detectionRange)
            {
                monsterViewModel.ChangeState(new ChasingState(monsterViewModel, playerTransform, detectionRange, attackRange));
            }
            else
            {
                monsterViewModel.ChangeState(new IdleState(monsterViewModel, detectionRange, attackRange));
            }
        }

        monsterViewModel.CurrentState.Execute();
    }
}
