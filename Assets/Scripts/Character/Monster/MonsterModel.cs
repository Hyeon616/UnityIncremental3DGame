using UnityEngine;

public class MonsterModel : CharacterModel
{
    [SerializeField] private int _initialHealth = 10;
    [SerializeField] private int _initialAttackPower = 1;
    [SerializeField] private float _monsterAttackRange = 5f;
    [SerializeField] private float _monsterAttackCooldown = 3f;
    private static HealthIncrementer _healthIncrementer = new HealthIncrementer(3);
    protected override void Start()
    {
        _maxHealth = _initialHealth;
        _attackPower = _initialAttackPower;
        _attackRange = _monsterAttackRange;
        _attackCooldown = _monsterAttackCooldown;
        base.Start();
    }

    public override int GetRandomAttackPower()
    {
        float randomMultiplier = Random.Range(0.8f, 1.2f);
        return Mathf.RoundToInt(_attackPower * randomMultiplier);
    }
    public static void ResetHealthIncrementer()
    {
        _healthIncrementer.Reset();
    }
    protected override void Die()
    {
        Debug.Log("Monster died.");
        gameObject.SetActive(false);
    }

    public void ResetHealth()
    {
        FullHeal();
    }
}

