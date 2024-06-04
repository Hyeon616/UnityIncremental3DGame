using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform player;
    private ObjectPool<MonsterViewModel> monsterPool;
    public int totalMonstersPerStage = 25;
    private int spawnedMonsters = 0;
    private int killedMonsters = 0;

    void Start()
    {
        monsterPool = new ObjectPool<MonsterViewModel>(monsterPrefab.GetComponent<MonsterViewModel>(), totalMonstersPerStage);
        StartCoroutine(SpawnMonstersRoutine());
    }

    IEnumerator SpawnMonstersRoutine()
    {
        while (true)
        {
            if (spawnedMonsters < totalMonstersPerStage)
            {
                for (int i = 0; i < 5; i++)
                {
                    SpawnMonster();
                    spawnedMonsters++;
                }
                yield return new WaitForSeconds(3f);
            }
            else
            {
                if (killedMonsters >= totalMonstersPerStage)
                {
                    // 플레이어 체력 회복
                    player.GetComponent<PlayerViewModel>().FullHeal();

                    yield return new WaitForSeconds(10f);
                    spawnedMonsters = 0;
                    killedMonsters = 0;
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

    void SpawnMonster()
    {
        MonsterViewModel monster = monsterPool.GetObject();
        if (monster != null)
        {
            Vector3 spawnPosition = GetRandomPosition();
            monster.transform.position = spawnPosition;
            monster.gameObject.SetActive(true);

            monster.SetPlayer(player);
            monster.GetComponent<MonsterModel>().ResetHealth();
        }
    }

    Vector3 GetRandomPosition()
    {
        float mapSize = 50f; // 맵의 크기, 50x50 유닛 기준

        Vector3 randomPosition = Vector3.zero;
        bool validPositionFound = false;
        int attempts = 0;
        int maxAttempts = 100; // 시도 횟수

        while (!validPositionFound && attempts < maxAttempts)
        {
            float randomX = Random.Range(-mapSize / 2, mapSize / 2);
            float randomZ = Random.Range(-mapSize / 2, mapSize / 2);
            randomPosition = new Vector3(randomX, 0, randomZ);

            // 추가 검사를 통해 위치의 유효성을 확인할 수 있습니다.
            if (IsPositionValid(randomPosition))
            {
                validPositionFound = true;
            }

            attempts++;
        }

        if (!validPositionFound)
        {
            Debug.LogWarning("Failed to find valid spawn position after multiple attempts. Defaulting to origin.");
            randomPosition = Vector3.zero;
        }

        return randomPosition;
    }

    bool IsPositionValid(Vector3 position)
    {
        // 필요에 따라 추가 검사를 수행할 수 있습니다.
        // 예: 특정 범위 안에 있는지, 다른 오브젝트와 겹치지 않는지 등
        return true; // 기본적으로 모든 위치를 유효하다고 가정
    }

    void Update()
    {
        if (AllMonstersDead())
        {
            killedMonsters++;
        }
    }

    bool AllMonstersDead()
    {
        return monsterPool.AllObjectsInactive();
    }
}

