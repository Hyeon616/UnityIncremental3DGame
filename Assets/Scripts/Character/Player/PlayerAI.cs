//using UnityEngine;
//using UnityEngine.AI;

//public class PlayerAI : MonoBehaviour
//{
//    private NavMeshAgent agent;
//    private PlayerViewModel playerViewModel;

//    private void Start()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        playerViewModel = GetComponent<PlayerViewModel>();

//        playerViewModel.SetState(new IdleState(playerViewModel, playerViewModel.CharacterModel.DetectionRange, playerViewModel.CharacterModel.AttackRange));
//    }

//    private void Update()
//    {
//        Transform target = playerViewModel.FindTarget(playerViewModel.CharacterModel.DetectionRange, "Monster");
//        if (target != null)
//        {
//            playerViewModel.Target = target;
//            if (playerViewModel.CurrentState is IdleState)
//            {
//                playerViewModel.SetState(new ChasingState(playerViewModel, target, playerViewModel.CharacterModel.DetectionRange, playerViewModel.CharacterModel.AttackRange));
//            }
//        }
//        else
//        {
//            if (playerViewModel.CurrentState is not IdleState)
//            {
//                playerViewModel.SetState(new IdleState(playerViewModel, playerViewModel.CharacterModel.DetectionRange, playerViewModel.CharacterModel.AttackRange));
//            }
//        }

//        playerViewModel.CurrentState.Execute();
//    }
//}
