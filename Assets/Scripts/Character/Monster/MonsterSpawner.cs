using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : UnitySingleton<MonsterSpawner>
{
    public List<GameObject> CurrentMonsterObjects { get; private set; } = new List<GameObject>();
    

    private void Start()
    {
        MonsterManager.Instance.OnMonsterDefeated += OnMonsterDefeated;
        StageManager.Instance.OnStageChanged += SpawnMonstersForCurrentStage;
    }

    private void OnDestroy()
    {
        MonsterManager.Instance.OnMonsterDefeated -= OnMonsterDefeated;
        StageManager.Instance.OnStageChanged -= SpawnMonstersForCurrentStage;
    }

    public void SpawnMonstersForCurrentStage()
    {
        ClearCurrentMonsters();
        string currentStage = StageManager.Instance.CurrentStage;
        List<MonsterModel> monstersToSpawn = MonsterManager.Instance.GetMonstersForStage(currentStage);

        foreach (var monsterModel in monstersToSpawn)
        {
            SpawnMonsterObject(monsterModel);
        }

        Debug.Log($"Spawning monsters for stage {currentStage}. Total spawned: {CurrentMonsterObjects.Count}");
    }

    private void SpawnMonsterObject(MonsterModel monsterModel)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Monsters/{monsterModel.PrefabName}");
        if (prefab != null)
        {
            GameObject monsterObject = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity);
            MonsterController controller = monsterObject.AddComponent<MonsterController>();
            controller.Initialize(monsterModel);
            monsterObject.tag = "Monster";
            CurrentMonsterObjects.Add(monsterObject);
        }
        else
        {
            Debug.LogError($"Failed to load monster prefab: {monsterModel.PrefabName}");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(UnityEngine.Random.Range(-25f, 25f), 0, UnityEngine.Random.Range(-25f, 25f));
    }

    private void OnMonsterDefeated(MonsterModel monster)
    {
        GameObject defeatedMonster = CurrentMonsterObjects.Find(m => m != null && m.GetComponent<MonsterController>()?.Model == monster);
        if (defeatedMonster != null)
        {
            CurrentMonsterObjects.Remove(defeatedMonster);
            Destroy(defeatedMonster);
        }
    }

    private void ClearCurrentMonsters()
    {
        foreach (var monster in CurrentMonsterObjects)
        {
            Destroy(monster);
        }
        CurrentMonsterObjects.Clear();
        MonsterManager.Instance.ClearActiveMonsters();
    }
}
