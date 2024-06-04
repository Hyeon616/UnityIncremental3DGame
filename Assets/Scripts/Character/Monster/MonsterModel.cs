using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterModel : CharacterModel
{
    [SerializeField] private int initialHealth = 10;
    [SerializeField] private int initialAttackPower = 1;

    protected override void Awake()
    {
        maxHealth = initialHealth;
        attackPower = initialAttackPower;
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
