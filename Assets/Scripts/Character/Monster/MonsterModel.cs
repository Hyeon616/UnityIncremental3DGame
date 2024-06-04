using UnityEngine;

public class MonsterModel : CharacterModel
{
    [SerializeField] private int initialHealth = 10;
    [SerializeField] private int initialAttackPower = 1;
    [SerializeField] private float monsterAttackRange = 5f;
    [SerializeField] private float monsterAttackCooldown = 3f;


    protected override void Awake()
    {
        maxHealth = initialHealth;
        attackPower = initialAttackPower;
        attackRange = monsterAttackRange;
        attackCooldown = monsterAttackCooldown;
        base.Awake();
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
