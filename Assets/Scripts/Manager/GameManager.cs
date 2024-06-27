using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class GameManager : UnitySingleton<GameManager>
{
    public Dictionary<string, GameObject> UIPrefabs { get; private set; } = new Dictionary<string, GameObject>();

    private async void Awake()
    {
        await LoadAllUIPrefabs();
        UIManager.Instance.InitializeUI();
    }

    private async UniTask LoadAllUIPrefabs()
    {
        
        var prefabs = Resources.LoadAll<GameObject>("Prefabs/UI/PopupUI");
        foreach (var prefab in prefabs)
        {
            UIPrefabs[prefab.name] = prefab;
        }
        await UniTask.Yield();
    }

    public async UniTask InitializeGame()
    {
        int userId = GetUserId();
        try
        {
            await ResourceManager.Instance.LoadAllData(userId);
            UIManager.Instance.HideUI("LoadingUI");
        }
        catch (Exception ex)
        {
            UIManager.Instance.ShowError("An error occurred during game initialization.");
            Debug.LogError($"Exception during game initialization: {ex.Message}");
        }
    }

    private int GetUserId()
    {
        return PlayerPrefs.GetInt("UserId");
    }
}