using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : UnitySingleton<MonsterSpawner>
{
    public List<GameObject> CurrentMonsterObjects { get; private set; } = new List<GameObject>();
    private int monstersToSpawn;
    private int monstersDefeated;

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
        monstersDefeated = 0;

        string currentStage = StageManager.Instance.CurrentStage;
        bool isBossStage = StageManager.Instance.IsBossStage();
        monstersToSpawn = isBossStage ? 1 : StageManager.TotalMonstersPerStage;

        for (int i = 0; i < monstersToSpawn; i++)
        {
            SpawnMonster(currentStage);
        }

        Debug.Log($"Spawning monsters for stage {currentStage}. Total to spawn: {monstersToSpawn}");

    }


    private void SpawnMonster(string stage)
    {
        MonsterManager.Instance.SetCurrentMonster(stage);
        var currentMonster = MonsterManager.Instance.CurrentMonster;

        if (currentMonster != null)
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Monsters/{currentMonster.PrefabName}");
            if (prefab != null)
            {
                GameObject monsterObject = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity);
                MonsterController controller = monsterObject.AddComponent<MonsterController>();
                controller.Initialize(currentMonster);
                monsterObject.tag = "Monster";
                CurrentMonsterObjects.Add(monsterObject);
            }
            else
            {
                Debug.LogError($"Failed to load monster prefab: {currentMonster.PrefabName}");
            }
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

        monstersDefeated++;
        Debug.Log($"Monster defeated. {monstersDefeated}/{monstersToSpawn} defeated.");

        if (monstersDefeated >= monstersToSpawn)
        {
            Debug.Log("All monsters defeated. Progressing to next stage.");
            StageManager.Instance.ProgressToNextStage();
        }
    }

    private void ClearCurrentMonsters()
    {
        foreach (var monster in CurrentMonsterObjects)
        {
            Destroy(monster);
        }
        CurrentMonsterObjects.Clear();
    }
}
