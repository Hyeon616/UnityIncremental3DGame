using UnityEngine;

public class MonsterViewModel : CharacterViewModel
{
    private Transform _player;

    public void SetPlayer(Transform playerTransform)
    {
        _player = playerTransform;
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
        if (_player != null && Vector3.Distance(transform.position, _player.position) <= 100f)
        {
            MoveTo(_player.position);
        }
    }

    protected override void ShowDamage(Transform targetTransform, int damageAmount)
    {
        // 몬스터는 데미지 폰트를 표시하지 않음
    }

}
