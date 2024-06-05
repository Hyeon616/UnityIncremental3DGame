using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    private float detectionRange = 100f;
    private float attackRange = 5f;
    private float attackCooldown = 2f;
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private PlayerViewModel playerViewModel;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerViewModel = GetComponent<PlayerViewModel>();

        playerViewModel.SetState(new IdleState(playerViewModel, detectionRange, attackRange));
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
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

            if (distanceToTarget <= attackRange)
            {
                playerViewModel.ChangeState(new AttackingState(playerViewModel, closestTarget.transform, detectionRange, attackRange));
            }
            else
            {
                playerViewModel.ChangeState(new ChasingState(playerViewModel, closestTarget.transform, detectionRange, attackRange));
            }

            playerViewModel.Target = closestTarget.transform;
        }
        else
        {
            playerViewModel.ChangeState(new IdleState(playerViewModel, detectionRange, attackRange));
        }

        playerViewModel.CurrentState.Execute();
    }
}
