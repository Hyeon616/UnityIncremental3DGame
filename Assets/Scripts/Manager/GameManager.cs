using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class GameManager : UnitySingleton<GameManager>
{
    public Dictionary<string, GameObject> UIPrefabs { get; private set; } = new Dictionary<string, GameObject>();

    private string authToken;
    private int userId;

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
        
        try
        {
            Debug.Log($"Initializing game for user ID: {userId}");
            await ResourceManager.Instance.LoadAllData(userId);
            UIManager.Instance.HideUI("LoadingUI");
        }
        catch (Exception ex)
        {
            UIManager.Instance.ShowError("An error occurred during game initialization.");
            Debug.LogError($"Exception during game initialization: {ex.Message}");
            Debug.LogError($"Stack trace: {ex.StackTrace}");
        }
    }

    public void SetAuthToken(string token)
    {
        authToken = token;
    }

    public void SetUserId(int id)
    {
        userId = id;
    }

    public string GetAuthToken()
    {
        return authToken;
    }

    public int GetUserId()
    {
        return userId;
    }
}