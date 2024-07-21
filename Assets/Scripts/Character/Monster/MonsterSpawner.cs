using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : UnitySingleton<MonsterSpawner>
{
    public GameObject CurrentMonsterObject { get; private set; }

    private void Start()
    {
        MonsterManager.Instance.OnMonsterDefeated += OnMonsterDefeated;
    }

    private void OnDestroy()
    {
        MonsterManager.Instance.OnMonsterDefeated -= OnMonsterDefeated;
    }

    public void SpawnMonster(string stage)
    {
        MonsterManager.Instance.SetCurrentMonster(stage);
        var currentMonster = MonsterManager.Instance.CurrentMonster;

        if (currentMonster != null)
        {
            if (CurrentMonsterObject != null)
            {
                Destroy(CurrentMonsterObject);
            }

            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Monsters/{currentMonster.PrefabName}");
            if (prefab != null)
            {
                CurrentMonsterObject = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity);
                MonsterController controller = CurrentMonsterObject.AddComponent<MonsterController>();
                controller.Initialize(currentMonster);
            }
            else
            {
                Debug.LogError($"Failed to load monster prefab: {currentMonster.PrefabName}");
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f));
    }

    private void OnMonsterDefeated(MonsterModel monster)
    {
        if (CurrentMonsterObject != null)
        {
            Destroy(CurrentMonsterObject);
        }
    }
}
