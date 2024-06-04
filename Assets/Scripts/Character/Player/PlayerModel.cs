using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : CharacterModel
{
    [SerializeField] private int initialHealth = 100;
    [SerializeField] private int initialAttackPower = 6;

    protected override void Awake()
    {
        maxHealth = initialHealth;
        attackPower = initialAttackPower;
        base.Awake();
    }

    protected override void Die()
    {
        Debug.Log("Player died.");
        // 사망 처리 로직 추가
    }
}
