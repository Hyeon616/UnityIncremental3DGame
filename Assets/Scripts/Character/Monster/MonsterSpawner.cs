using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject GameObject_MonsterPrefab;
    public Transform Transform_Player;
    //private ObjectPool<MonsterViewModel> _monsterPool;
    private int _totalMonstersPerStage = 15;
    private int _spawnedMonsters = 0;
    private int _killedMonsters = 0;

    void Start()
    {
        //_monsterPool = new ObjectPool<MonsterViewModel>(GameObject_MonsterPrefab.GetComponent<MonsterViewModel>(), _totalMonstersPerStage);
        SpawnMonstersRoutine().Forget();
    }

    private async UniTaskVoid SpawnMonstersRoutine()
    {
        while (true)
        {
            if (_spawnedMonsters < _totalMonstersPerStage)
            {
                for (int i = 0; i < 5; i++)
                {
                    SpawnMonster();
                    _spawnedMonsters++;
                }
                await UniTask.Delay(3000); // 3초 대기
            }
            else
            {
                if (_killedMonsters >= _totalMonstersPerStage)
                {
                    //Transform_Player.GetComponent<PlayerViewModel>().FullHeal();
                   // MonsterModel.ResetHealthIncrementer();

                    await UniTask.Delay(10000); // 10초 대기
                    _spawnedMonsters = 0;
                    _killedMonsters = 0;
                }
                else
                {
                    await UniTask.Yield();
                }
            }
        }
    }

    void SpawnMonster()
    {
        //MonsterViewModel monster = _monsterPool.GetObject();
        //if (monster != null)
        //{
        //    Vector3 spawnPosition = GetRandomPosition();
        //    monster.transform.position = spawnPosition;
        //    monster.gameObject.SetActive(true);

        //    monster.SetPlayer(Transform_Player);
        //    monster.GetComponent<MonsterModel>().ResetHealth();
        //}
    }

    Vector3 GetRandomPosition()
    {
        float minDistance = 10f; // 최소 스폰 거리
        float maxDistance = 30f; // 최대 스폰 거리

        Vector3 randomPosition;
        do
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += Transform_Player.position; // 플레이어의 위치를 기준으로 스폰
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, maxDistance, 1);
            randomPosition = hit.position;
        } while (Vector3.Distance(Transform_Player.position, randomPosition) < minDistance); // 최소 거리 조건 체크

        return randomPosition;
    }

    void Update()
    {
        //foreach (MonsterViewModel monster in _monsterPool.ActiveObjects())
        //{
        //    if (monster.gameObject.activeSelf && !monster.HasTarget)
        //    {
        //        monster.SetPlayer(Transform_Player);
        //    }
        //}

        //if (AllMonstersDead())
        //{
        //    _killedMonsters++;
        //}
    }

    //bool AllMonstersDead()
    //{
    //    return _monsterPool.AllObjectsInactive();
    //}
}
