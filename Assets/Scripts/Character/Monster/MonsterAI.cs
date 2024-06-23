using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private MonsterViewModel monsterViewModel;
    //private PlayerViewModel playerViewModel;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterViewModel = GetComponent<MonsterViewModel>();
        //playerViewModel = FindObjectOfType<PlayerViewModel>();

        // MonsterModel의 값을 사용하여 상태 설정
        monsterViewModel.SetState(new IdleState(monsterViewModel, monsterViewModel.CharacterModel.DetectionRange, monsterViewModel.CharacterModel.AttackRange));
    }

    private void Update()
    {
        if (monsterViewModel.CurrentState is IdleState)
        {
            Transform target = monsterViewModel.FindTarget(monsterViewModel.CharacterModel.DetectionRange, "Player");
            if (target != null)
            {
                monsterViewModel.Target = target;
                monsterViewModel.SetState(new ChasingState(monsterViewModel, target, monsterViewModel.CharacterModel.DetectionRange, monsterViewModel.CharacterModel.AttackRange));
            }
        }
        else
        {
            if (monsterViewModel.Target == null)
            {
                monsterViewModel.SetState(new IdleState(monsterViewModel, monsterViewModel.CharacterModel.DetectionRange, monsterViewModel.CharacterModel.AttackRange));
            }
        }
        monsterViewModel.CurrentState.Execute();
    }
}
