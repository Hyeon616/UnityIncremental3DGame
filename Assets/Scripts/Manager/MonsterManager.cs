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
            // 로그 출력 (Unity의 Debug.LogError 대신 다른 로깅 메커니즘 사용)
            Console.WriteLine($"No monster template found for stage: {stage}");
            return;
        }

        int monstersToSpawn = StageManager.Instance.IsBossStage() ? 1 : StageManager.TotalMonstersPerStage;

        for (int i = 0; i < monstersToSpawn; i++)
        {
            SpawnMonster(template);
        }
    }

    private void SpawnMonster(MonsterModel template)
    {
        MonsterModel newMonster = new MonsterModel();
        CopyMonsterProperties(template, newMonster);
        ActiveMonsters.Add(newMonster);
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
        MonsterModel template = monsterTemplates.Find(m => m.Stage == stage);
        if (template == null)
        {
            Console.WriteLine($"No monster template found for stage: {stage}");
            return new List<MonsterModel>();
        }

        int monstersToSpawn = StageManager.Instance.IsBossStage() ? 1 : StageManager.TotalMonstersPerStage;
        List<MonsterModel> monsters = new List<MonsterModel>();

        for (int i = 0; i < monstersToSpawn; i++)
        {
            MonsterModel newMonster = new MonsterModel();
            CopyMonsterProperties(template, newMonster);
            monsters.Add(newMonster);
        }

        ActiveMonsters = monsters;
        return monsters;
    }
    public void ClearActiveMonsters()
    {
        ActiveMonsters.Clear();
    }

    public void Reset()
    {
        ClearActiveMonsters();
    }
}