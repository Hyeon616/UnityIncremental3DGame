using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    private float _detectionRange = 100f;
    private float _attackRange = 5f;
    private float _attackCooldown = 2f;
    private float _attackTimer = 0f;

    private NavMeshAgent _agent;
    private PlayerViewModel _playerViewModel;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _playerViewModel = GetComponent<PlayerViewModel>();

        _playerViewModel.SetState(new IdleState(_playerViewModel, _detectionRange, _attackRange));
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRange);
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.gameObject;
                }
            }
        }

        if (closestTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, closestTarget.transform.position);

            if (distanceToTarget <= _attackRange)
            {
                _playerViewModel.ChangeState(new AttackingState(_playerViewModel, closestTarget.transform, _detectionRange, _attackRange));
            }
            else
            {
                _playerViewModel.ChangeState(new ChasingState(_playerViewModel, closestTarget.transform, _detectionRange, _attackRange));
            }

            _playerViewModel.Target = closestTarget.transform;
        }
        else
        {
            _playerViewModel.ChangeState(new IdleState(_playerViewModel, _detectionRange, _attackRange));
        }

        _playerViewModel.CurrentState.Execute();
    }
}
