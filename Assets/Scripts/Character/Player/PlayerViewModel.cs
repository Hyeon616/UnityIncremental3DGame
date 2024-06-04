using UnityEngine;

public class PlayerViewModel : CharacterViewModel
{
  //  private GameObject target;

    protected override void Die()
    {
        Debug.Log("Player died.");
        // 플레이어 사망 처리 로직 추가
    }

    public void FindAndAttackTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
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

        if (closestTarget != null)
        {
            target = closestTarget;
            Attack(target.GetComponent<CharacterViewModel>());
        }
    }
}
