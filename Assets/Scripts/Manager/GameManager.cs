using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class GameManager : UnitySingleton<GameManager>
{
    public Dictionary<string, GameObject> UIPrefabs { get; private set; } = new Dictionary<string, GameObject>();

    private string authToken;
    private int userId;

    private float timeSinceLastOnlineUpdate = 0f;
    private const float onlineUpdateInterval = 300f;

    protected override async void Awake()
    {
        base.Awake();
        await LoadAllUIPrefabs();
        Initialize();
        UIManager.Instance.InitializeUI();
        CombatManager.Instance.Initialize();
    }

    private void Update()
    {
        if (IsAuthenticated())
        {
            UpdateOnlineTime();
        }

    }

    private void Initialize()
    {
        _ = AudioManager.Instance;
        _ = UIManager.Instance;
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
            await ResourceManager.Instance.LoadAllData(userId);
            await SpawnPlayer();
            StageManager.Instance.Initialize(GameLogic.Instance.CurrentPlayer.attributes.current_stage);
            UIManager.Instance.HideUI(UIPrefab.LoadingUI);

        }
        catch (Exception ex)
        {
            UIManager.Instance.ShowError("An error occurred during game initialization.");
            Debug.LogError($"Exception during game initialization: {ex.Message}");
            Debug.LogError($"Stack trace: {ex.StackTrace}");
        }
    }



    private async UniTask SpawnPlayer()
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        if (playerPrefab != null)
        {
            GameObject playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            await UniTask.WaitUntil(() => playerController.IsInitialized);

            CameraManager.Instance.SetPlayerTarget(playerObject.transform);
        }
        else
        {
            Debug.LogError("Failed to load player prefab");
        }
    }

    private void UpdateOnlineTime()
    {
        timeSinceLastOnlineUpdate += Time.deltaTime;
        if (timeSinceLastOnlineUpdate >= onlineUpdateInterval)
        {
            SendOnlineTimeUpdate();
            timeSinceLastOnlineUpdate = 0f;
        }
    }

    private async void SendOnlineTimeUpdate()
    {
        await ResourceManager.Instance.UpdateOnlineTime();
    }



    public void SetAuthToken(string token)
    {
        authToken = token;
        Debug.Log($"Auth token set: {token}");
    }

    public string GetAuthToken()
    {
        if (string.IsNullOrEmpty(authToken))
        {
            Debug.LogWarning("Auth token is empty or null");
        }
        return authToken;
    }

    public void SetUserId(int id)
    {
        userId = id;
        Debug.Log($"User ID set: {id}");
    }

    public int GetUserId()
    {
        return userId;
    }

    public bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(authToken) && userId != 0;
    }

    public void ClearAuthData()
    {
        authToken = null;
        userId = 0;
        GameLogic.Instance.CurrentPlayer = null;
        StageManager.Instance.Reset();
        MonsterManager.Instance.Reset();
        Debug.Log("Auth data cleared");
    }

    // 게임 세션이 끝날 때 호출되는 메서드
    public void OnApplicationQuit()
    {
        MonsterController[] monsters = FindObjectsOfType<MonsterController>();
        foreach (var monster in monsters)
        {
            monster.enabled = false;
        }

        ClearAuthData();
    }
}