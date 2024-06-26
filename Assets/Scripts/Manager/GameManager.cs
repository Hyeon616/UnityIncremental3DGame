using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

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

        try
        {
            bool loadDataSuccess = await ResourceManager.Instance.LoadAllData(userId);
            if (loadDataSuccess)
            {
                Debug.Log("Game data loaded successfully.");
                UIManager.Instance.OnGameDataLoaded();
            }
            else
            {
                Debug.LogError("Failed to load game data.");
                UIManager.Instance.ShowError("Failed to load game data.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception during game initialization: {ex.Message}");
            UIManager.Instance.ShowError("An error occurred during game initialization.");
        }
    }

    private int GetUserId()
    {
        return PlayerPrefs.GetInt("UserId");
    }
}