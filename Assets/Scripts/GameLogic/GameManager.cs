using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameManager : UnitySingleton<GameManager>
{
    private async void Start()
    {
        await InitializeGameData();
    }

    private async UniTask InitializeGameData()
    {
        int userId = GetUserId();
        await GameLogic.Instance.LoadPlayerData(userId);
        await GameLogic.Instance.LoadMails(userId);
        await GameLogic.Instance.LoadGuilds();
        await GameLogic.Instance.LoadFriends(userId);
        await GameLogic.Instance.LoadPlayerWeapons(userId);
        await GameLogic.Instance.LoadPlayerSkills(userId);
        await GameLogic.Instance.LoadPlayerBlessings(userId);
        await GameLogic.Instance.LoadMissionProgress(userId);
    }

    private int GetUserId()
    {
        return PlayerPrefs.GetInt("UserId"); // 로그인 시 저장된 UserId를 가져옵니다.
    }
}