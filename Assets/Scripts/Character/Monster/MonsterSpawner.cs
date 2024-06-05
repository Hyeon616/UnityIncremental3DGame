using System.Collections;
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
        float minDistance = 10f; // 최소 스폰 거리
        float maxDistance = 30f; // 최대 스폰 거리

        Vector3 randomPosition;
        do
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += player.position; // 플레이어의 위치를 기준으로 스폰
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, maxDistance, 1);
            randomPosition = hit.position;
        } while (Vector3.Distance(player.position, randomPosition) < minDistance); // 최소 거리 조건 체크

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

