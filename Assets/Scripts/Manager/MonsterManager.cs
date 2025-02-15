using System.Collections.Generic;
using System;

public class MonsterManager : Singleton<MonsterManager>
{
    private List<MonsterModel> monsterTemplates;
    public List<MonsterModel> ActiveMonsters { get; private set; } = new List<MonsterModel>();
    public event Action<MonsterModel> OnMonsterDefeated;
    public event Action<float, MonsterModel> OnMonsterHealthChanged;

    public void LoadMonsters(List<MonsterModel> loadedMonsters)
    {
        monsterTemplates = loadedMonsters;
    }

    public void SpawnMonstersForStage(string stage)
    {
        ClearActiveMonsters();
        MonsterModel template = monsterTemplates.Find(m => m.Stage == stage);
        if (template == null)
        {
            return;
        }
        int monstersToSpawn = StageManager.Instance.IsBossStage() ? 1 : StageManager.TotalMonstersPerStage;
        for (int i = 0; i < monstersToSpawn; i++)
        {
            MonsterModel newMonster = SpawnMonster(template);
            ActiveMonsters.Add(newMonster);
        }
    }

    private MonsterModel SpawnMonster(MonsterModel template)
    {
        MonsterModel newMonster = new MonsterModel();
        CopyMonsterProperties(template, newMonster);
        return newMonster;
    }

    private void CopyMonsterProperties(MonsterModel source, MonsterModel destination)
    {
        // 속성 복사 로직
        destination.id = source.id;
        destination.Stage = source.Stage;
        destination.Type = source.Type;
        destination.Name = source.Name;
        destination.Health = source.Health;
        destination.Attack = source.Attack;
        destination.DropMoney = source.DropMoney;
        destination.DropElementStone = source.DropElementStone;
        destination.DropElementStoneChance = source.DropElementStoneChance;
        destination.CurrentHealth = source.Health;
        destination.PrefabName = source.PrefabName;
    }

    public void DamageMonster(MonsterModel monster, int damage)
    {
        monster.CurrentHealth -= damage;
        if (monster.CurrentHealth <= 0)
        {
            DefeatMonster(monster);
        }
        else
        {
            OnMonsterHealthChanged?.Invoke(monster.GetHealthPercentage(), monster);
        }
    }

    public void DefeatMonster(MonsterModel monster)
    {
        ActiveMonsters.Remove(monster);
        OnMonsterDefeated?.Invoke(monster);
        StageManager.Instance.IncrementMonstersDefeated();
        if (ActiveMonsters.Count == 0)
        {
            StageManager.Instance.OnAllMonstersDefeated();
        }
    }

    public List<MonsterModel> GetMonstersForStage(string stage)
    {
        MonsterModel monsterModel = monsterTemplates.Find(m => m.Stage == stage);
        if (monsterModel == null)
        {
            return new List<MonsterModel>();
        }

        int monstersToSpawn = StageManager.Instance.IsBossStage() ? 1 : StageManager.TotalMonstersPerStage;
        List<MonsterModel> monsters = new List<MonsterModel>();

        for (int i = 0; i < monstersToSpawn; i++)
        {
            MonsterModel newMonster = new MonsterModel();
            CopyMonsterProperties(monsterModel, newMonster);
            monsters.Add(newMonster);
        }

        ActiveMonsters = monsters;
        return monsters;
    }


    public void ClearActiveMonsters()
    {
        var monsterControllers = UnityEngine.Object.FindObjectsOfType<MonsterController>();
        foreach (var controller in monsterControllers)
        {
            UnityEngine.Object.Destroy(controller.gameObject);
        }
        ActiveMonsters.Clear();
    }




    public void Reset()
    {
        ClearActiveMonsters();
    }
}