using UnityEngine;

public class MonsterViewModel : CharacterViewModel
{
    private Transform player;

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void MoveTo(Vector3 destination)
    {
        if (Agent != null)
        {
            Agent.SetDestination(destination);
        }
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
        // 추가적인 몬스터 사망 처리 로직
    }

    public override void Update()
    {
        base.Update();
        if (player != null && Vector3.Distance(transform.position, player.position) <= 100f)
        {
            MoveTo(player.position);
        }
    }
}
