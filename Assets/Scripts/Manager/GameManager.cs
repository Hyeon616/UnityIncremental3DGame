using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class GameManager : UnitySingleton<GameManager>
{
    public Dictionary<string, GameObject> UIPrefabs { get; private set; } = new Dictionary<string, GameObject>();

    private void Awake()
    {
        LoadAllUIPrefabs();
    }

    private void Start()
    {
        UIManager.Instance.StartGame();
    }

    private void LoadAllUIPrefabs()
    {
        var prefabs = Resources.LoadAll<GameObject>("Prefabs/UI/PopupUI");
        foreach (var prefab in prefabs)
        {
            UIPrefabs[prefab.name] = prefab;
        }
    }

    public async UniTask InitializeGame()
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
        return PlayerPrefs.GetInt("UserId");
    }
}