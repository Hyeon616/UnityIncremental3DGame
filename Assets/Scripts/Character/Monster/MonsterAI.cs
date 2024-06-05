using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private float detectionRange = 100f;
    private float attackRange = 5f;
    private float attackCooldown = 3f;
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private MonsterViewModel monsterViewModel;
    private PlayerViewModel playerViewModel;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterViewModel = GetComponent<MonsterViewModel>();
        playerViewModel = FindObjectOfType<PlayerViewModel>();

        monsterViewModel.SetState(new IdleState(monsterViewModel, detectionRange, attackRange));
    }

    private void Update()
    {
        monsterViewModel.CurrentState.Execute();
    }
}
