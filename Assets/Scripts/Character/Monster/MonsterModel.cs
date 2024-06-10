using Cysharp.Threading.Tasks;
using UnityEngine;

public class MonsterModel : CharacterModel
{
    [SerializeField] private int _initialHealth = 10;
    [SerializeField] private int _initialAttackPower = 1;
    [SerializeField] private float _monsterAttackRange = 5f;
    [SerializeField] private float _monsterAttackCooldown = 3f;
    [SerializeField] private float _monsterDetectionRange = 100f;
  
    private static HealthIncrementer _healthIncrementer = new HealthIncrementer(3);

    protected override void Start()
    {
        _maxHealth = _initialHealth;
        _attackPower = _initialAttackPower;
        _attackRange = _monsterAttackRange;
        _attackCooldown = _monsterAttackCooldown;
        _detectionRange = _monsterDetectionRange;
        base.Start();
    }

    public override int GetRandomAttackPower()
    {
        return base.GetRandomAttackPower();
    }

    public static void ResetHealthIncrementer()
    {
        _healthIncrementer.Reset();
    }

    protected override async void Die()
    {
        gameObject.SetActive(false);
        await TryDropWeapon();
    }

    public void ResetHealth()
    {
        FullHeal();
    }

    private async UniTask TryDropWeapon()
    {
        // 10% 확률로 무기 드랍
        if (Random.value <= 0.4f)
        {
            Weapon droppedWeapon = await WeaponManager.Instance.GetRandomWeapon();
            if (droppedWeapon != null)
            {
                WeaponInventoryUIManager.Instance.IncreaseWeaponCount(droppedWeapon);
                WeaponInventoryUIManager.Instance.ActivateWeaponSlot(droppedWeapon); // 슬롯 활성화
                DropNotificationManager.Instance.ShowDropNotification(droppedWeapon.rarity, droppedWeapon.grade);
                Debug.Log($"Dropped {droppedWeapon.rarity} {droppedWeapon.grade} weapon: {droppedWeapon.id}");
            }
        }
    }
    private string GetRandomRarity()
    {
        float randomValue = Random.value * 100f;

        if (randomValue <= 0.1f) return "신화";
        else if (randomValue <= 0.3f) return "고대";
        else if (randomValue <= 0.8f) return "에픽";
        else if (randomValue <= 1.6f) return "영웅";
        else if (randomValue <= 6.7f) return "유물";
        else if (randomValue <= 16.9f) return "매직";
        else if (randomValue <= 50f) return "고급";
        else return "일반";
    }
}
