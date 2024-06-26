using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    private Dictionary<string, GameObject> activeUIs = new Dictionary<string, GameObject>();
    private Dictionary<string, CancellationTokenSource> removeTimers = new Dictionary<string, CancellationTokenSource>();
    private Stack<GameObject> uiPool = new Stack<GameObject>();
    private const float UI_REMOVE_DELAY = 15f; // 3 minutes

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
            var uiInstance = GetOrCreateUIInstance(prefab, instanceName);
            activeUIs[instanceName] = uiInstance;
        }
        else
        {
            var uiInstance = activeUIs[instanceName];

            if (uiInstance == null)
            {
                Debug.LogWarning($"UI '{uiName}' was destroyed unexpectedly. Re-instantiating...");
                uiInstance = GetOrCreateUIInstance(prefab, instanceName);
                activeUIs[instanceName] = uiInstance;
            }
            else
            {
                uiInstance.SetActive(true);
                CancelRemoveUITimer(instanceName); // 타이머 취소
            }
        }
    }

    private GameObject GetOrCreateUIInstance(GameObject prefab, string instanceName)
    {
        var canvas = GameObject.Find("@UI_Canvas");
        if (canvas == null)
        {
            Debug.LogError("@UI_Canvas not found!");
            return null;
        }

        GameObject uiInstance;
        if (uiPool.Count > 0)
        {
            uiInstance = uiPool.Pop();
            uiInstance.transform.SetParent(canvas.transform);
            uiInstance.SetActive(true);
        }
        else
        {
            uiInstance = Instantiate(prefab, canvas.transform);
        }

        uiInstance.name = instanceName;
        return uiInstance;
    }

    public void ShowLoadingUI()
    {
        ShowUI("LoadingUI");
    }

    public void HideLoadingUI()
    {
        HideUI("LoadingUI");
    }

    public void HideUI(string uiName)
    {
        string instanceName = $"@UI_{uiName}";

        if (activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            uiInstance.SetActive(false);
            StartRemoveUITimer(instanceName);
        }
    }

    private void StartRemoveUITimer(string instanceName)
    {
        CancelRemoveUITimer(instanceName); // 이전 타이머가 있을 경우 취소

        var cts = new CancellationTokenSource();
        removeTimers[instanceName] = cts;

        RemoveUIAfterDelay(instanceName, UI_REMOVE_DELAY, cts.Token).Forget();
    }

    private async UniTask RemoveUIAfterDelay(string instanceName, float delay, CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);

            if (activeUIs.TryGetValue(instanceName, out var uiInstance) && !uiInstance.activeSelf)
            {
                Destroy(uiInstance);
                activeUIs.Remove(instanceName);
                removeTimers.Remove(instanceName);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"UI '{instanceName}' removal canceled.");
        }
    }

    private void CancelRemoveUITimer(string instanceName)
    {
        if (removeTimers.TryGetValue(instanceName, out var cts))
        {
            cts.Cancel();
            removeTimers.Remove(instanceName);
        }
    }

    public async void HandleLoginSuccess()
    {
        HideUI("LoginUI");
        ShowLoadingUI();
        await GameManager.Instance.InitializeGame();
        HideLoadingUI();
    }

    public void OnGameDataLoaded()
    {
        HideLoadingUI();
    }

    public void ShowError(string message)
    {
        // 에러 메시지를 표시하는 UI
        Debug.LogError(message);
    }
}
