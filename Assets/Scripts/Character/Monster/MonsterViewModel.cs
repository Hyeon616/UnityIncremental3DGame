using UnityEngine;
using UnityEngine.AI;

public class MonsterViewModel : CharacterViewModel
{
    private Transform player;

    protected override void Die()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= 10f)
        {
            MoveTo(player.position);

            if (Vector3.Distance(transform.position, player.position) <= 5f)
            {
                Attack(player.GetComponent<CharacterViewModel>());
            }
        }
        else
        {
            Wander();
        }
    }

    void Wander()
    {
        Vector3 newPos = RandomNavSphere(transform.position, 20f, -1);
        MoveTo(newPos);
        characterView.Animator.SetBool("isWalking", true);
        characterView.Animator.SetBool("isAttacking", false);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
