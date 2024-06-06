using UnityEngine;

public class PlayerModel : CharacterModel
{
    [SerializeField] private int _initialHealth = 100;
    [SerializeField] private int _initialAttackPower = 6;
    [SerializeField] private float _playerAttackRange = 5f;
    [SerializeField] private float _playerAttackCooldown = 2f;
    [SerializeField] private float _criticalChance = 0.2f; // 20% 크리티컬 확률
    [SerializeField] private float _criticalMultiplier = 2f; // 크리티컬 데미지 배수

    private bool _isCriticalHit = false;

    protected override void Start()
    {
        _maxHealth = _initialHealth;
        _attackPower = _initialAttackPower;
        _attackRange = _playerAttackRange;
        _attackCooldown = _playerAttackCooldown;
        base.Start();
    }

    public override int GetRandomAttackPower()
    {
        float randomMultiplier = Random.Range(0.8f, 1.2f); // 80%에서 120% 범위의 무작위 값
        int baseDamage = Mathf.RoundToInt(_attackPower * randomMultiplier);

        _isCriticalHit = Random.value < _criticalChance;
        if (_isCriticalHit)
        {
            baseDamage = Mathf.RoundToInt(baseDamage * _criticalMultiplier);
            Debug.Log("Critical Hit!");
        }

        return baseDamage;
    }

    public bool IsCriticalHit()
    {
        return _isCriticalHit;
    }

    public void IncreaseCriticalChance(float amount)
    {
        _criticalChance += amount;
    }

    protected override void Die()
    {
        Debug.Log("Player died.");
        // 추가적인 플레이어 사망 처리 로직
    }
}
