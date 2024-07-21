using System.Collections.Generic;
using System;
using UnityEngine;

public class MonsterManager : Singleton<MonsterManager>
{
    private List<MonsterModel> monsters;
    public MonsterModel CurrentMonster { get; private set; }
    public event Action<MonsterModel> OnMonsterDefeated;
    public event Action<float> OnMonsterHealthChanged;

    public void LoadMonsters(List<MonsterModel> loadedMonsters)
    {
        monsters = loadedMonsters;
    }

    public void SetCurrentMonster(string stage)
    {
        CurrentMonster = monsters.Find(m => m.Stage == stage);
        if (CurrentMonster != null)
        {
            CurrentMonster.Initialize();
        }
        
    }



    public void DamageCurrentMonster(int damage)
    {
        if (CurrentMonster == null) return;

        CurrentMonster.CurrentHealth -= damage;

        if (CurrentMonster.CurrentHealth <= 0)
        {
            DefeatCurrentMonster();
        }

        OnMonsterHealthChanged?.Invoke(CurrentMonster.GetHealthPercentage());
    }

    public void DefeatCurrentMonster()
    {
        if (CurrentMonster != null)
        {
            CurrentMonster.CurrentHealth = 0;
            OnMonsterDefeated?.Invoke(CurrentMonster);
        }
    }
    public void Reset()
    {
        CurrentMonster = null;
    }
}