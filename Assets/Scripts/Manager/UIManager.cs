using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    private Dictionary<string, GameObject> activeUIs = new Dictionary<string, GameObject>();
    private Dictionary<string, CancellationTokenSource> uiCancellationTokens = new Dictionary<string, CancellationTokenSource>();
    private const float UI_REMOVE_DELAY = 180f; // 3 minutes

    public void StartGame()
    {
        ShowUI("LoginUI");
    }

    public void ShowUI(string uiName)
    {
        if (!GameManager.Instance.UIPrefabs.TryGetValue(uiName, out var prefab))
        {
            Debug.LogError($"UI Prefab '{uiName}' not found!");
            return;
        }

        string instanceName = $"@UI_{uiName}";

        if (!activeUIs.ContainsKey(instanceName))
        {
            InstantiateAndAddUI(instanceName, prefab);
        }
        else
        {
            var uiInstance = activeUIs[instanceName];

            if (uiInstance == null)
            {
                Debug.LogWarning($"UI '{uiName}' was destroyed unexpectedly. Re-instantiating...");
                InstantiateAndAddUI(instanceName, prefab);
            }
            else
            {
                uiInstance.SetActive(true);
                CancelRemoveUITimer(instanceName);
            }
        }
    }

    private void InstantiateAndAddUI(string instanceName, GameObject prefab)
    {
        var canvas = GameObject.Find("@UI_Canvas");
        if (canvas == null)
        {
            Debug.LogError("@UI_Canvas not found!");
            return;
        }

        var uiInstance = Instantiate(prefab, canvas.transform);
        if (uiInstance == null)
        {
            Debug.LogError($"Failed to instantiate UI '{instanceName}'!");
            return;
        }

        uiInstance.name = instanceName;
        activeUIs[instanceName] = uiInstance;
    }

    public void ShowLoadingUI()
    {
        ShowUI("LoadingUI");
    }

    public async UniTask HideLoadingUI()
    {
        HideUI("LoadingUI");
    }

    public void HideUI(string uiName)
    {
        string instanceName = $"@UI_{uiName}";

        if (activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            if (uiInstance == null)
            {
                Debug.LogWarning($"UI '{uiName}' is null but still in the activeUIs dictionary. Removing from dictionary.");
                activeUIs.Remove(instanceName);
                return;
            }

            uiInstance.SetActive(false);
            StartRemoveUITimer(instanceName, UI_REMOVE_DELAY);
        }
    }

    private async void StartRemoveUITimer(string instanceName, float delay)
    {
        var cts = new CancellationTokenSource();
        uiCancellationTokens[instanceName] = cts;

        try
        {
            await UniTask.Delay((int)(delay * 1000), cancellationToken: cts.Token);
            if (activeUIs.TryGetValue(instanceName, out var uiInstance) && uiInstance != null && !uiInstance.activeSelf)
            {
                Destroy(uiInstance);
                activeUIs.Remove(instanceName);
                uiCancellationTokens.Remove(instanceName);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"UI '{instanceName}' removal canceled.");
        }
    }

    public void CancelRemoveUITimer(string instanceName)
    {
        if (uiCancellationTokens.TryGetValue(instanceName, out var cts))
        {
            cts.Cancel();
            uiCancellationTokens.Remove(instanceName);
        }
    }

    public async void HandleLoginSuccess()
    {
        HideUI("LoginUI");
        ShowLoadingUI();
        await GameManager.Instance.InitializeGame();
        await HideLoadingUI();
    }
}
