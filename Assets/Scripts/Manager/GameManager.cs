using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameManager : UnitySingleton<GameManager>
{
    public async UniTask InitializeGameData()
    {
        await GameLogic.Instance.LoadPlayerData(GetUserId());
        await GameLogic.Instance.LoadMails(GetUserId());
        await GameLogic.Instance.LoadGuilds();
        await GameLogic.Instance.LoadFriends(GetUserId());
        await GameLogic.Instance.LoadPlayerWeapons(GetUserId());
        await GameLogic.Instance.LoadPlayerSkills(GetUserId());
        await GameLogic.Instance.LoadPlayerBlessings(GetUserId());
        await GameLogic.Instance.LoadMissionProgress(GetUserId());
        await GameLogic.Instance.LoadRewards();
        await GameLogic.Instance.LoadStages();
        await GameLogic.Instance.LoadCurrentStage();

        
    }

    private int GetUserId()
    {
        return PlayerPrefs.GetInt("UserId");
    }

}