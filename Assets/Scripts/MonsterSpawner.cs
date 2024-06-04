using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform player;
    private ObjectPool<MonsterAI> monsterPool;
    public int totalMonstersPerStage = 25;
    private int spawnedMonsters = 0;
    private int killedMonsters = 0;

    void Start()
    {
        monsterPool = new ObjectPool<MonsterAI>(monsterPrefab.GetComponent<MonsterAI>(), totalMonstersPerStage);
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
        MonsterAI monster = monsterPool.GetObject();
        if (monster != null)
        {
            Vector3 spawnPosition = GetRandomPosition();
            monster.transform.position = spawnPosition;
            monster.gameObject.SetActive(true);

            monster.SetPlayer(player);
            monster.ResetHealth();
        }
    }

    Vector3 GetRandomPosition()
    {
        float minDistance = 10f; // �ּ� ���� �Ÿ�
        float maxDistance = 30f; // �ִ� ���� �Ÿ�

        Vector3 randomPosition;
        do
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += player.position; // �÷��̾��� ��ġ�� �������� ����
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, maxDistance, 1);
            randomPosition = hit.position;
        } while (Vector3.Distance(player.position, randomPosition) < minDistance); // �ּ� �Ÿ� ���� üũ

        return randomPosition;
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

