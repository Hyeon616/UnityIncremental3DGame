using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : CharacterModel
{
    [SerializeField] private int initialHealth = 100;
    [SerializeField] private int initialAttackPower = 6;
    [SerializeField] private float playerAttackRange = 5f;
    [SerializeField] private float playerAttackCooldown = 2f;
    protected override void Awake()
    {
        maxHealth = initialHealth;
        attackPower = initialAttackPower;
        attackRange = playerAttackRange;
        attackCooldown = playerAttackCooldown;
        base.Awake();
    }

    protected override void Die()
    {
        Debug.Log("Player died.");
        // 사망 처리 로직 추가
    }
}
