using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private float _detectionRange = 100f;
    private float _attackRange = 5f;
    private float _attackCooldown = 3f;
    private float _attackTimer = 0f;

    private NavMeshAgent _agent;
    private MonsterViewModel _monsterViewModel;
    private PlayerViewModel _playerViewModel;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _monsterViewModel = GetComponent<MonsterViewModel>();
        _playerViewModel = FindObjectOfType<PlayerViewModel>();

        _monsterViewModel.SetState(new IdleState(_monsterViewModel, _detectionRange, _attackRange));
    }

    private void Update()
    {
        _monsterViewModel.CurrentState.Execute();
    }
}
