using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : CharacterModel
{
    [SerializeField] private int _initialHealth = 100;
    [SerializeField] private int _initialAttackPower = 6;
    [SerializeField] private float _playerAttackRange = 5f;
    [SerializeField] private float _playerAttackCooldown = 2f;
    protected override void Start()
    {
        _maxHealth = _initialHealth;
        _attackPower = _initialAttackPower;
        _attackRange = _playerAttackRange;
        _attackCooldown = _playerAttackCooldown;
        base.Start();
    }

    protected override void Die()
    {
        Debug.Log("Player died.");
        // 사망 처리 로직 추가
    }
}
