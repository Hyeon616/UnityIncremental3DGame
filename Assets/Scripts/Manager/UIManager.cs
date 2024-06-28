using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : UnitySingleton<UIManager>
{
    private Dictionary<string, GameObject> activeUIs = new Dictionary<string, GameObject>();
    private Dictionary<string, CancellationTokenSource> removeTimers = new Dictionary<string, CancellationTokenSource>();
    private Stack<GameObject> uiPool = new Stack<GameObject>();
    private const float UI_REMOVE_DELAY = 15f; // 3 minutes

    public void InitializeUI()
    {
        ShowUI("AuthenticationUI");
    }

    public void ShowUI(string uiName)
    {
        if (!GameManager.Instance.UIPrefabs.TryGetValue(uiName, out var prefab))
        {
            Debug.LogError($"UI Prefab '{uiName}' not found!");
            return;
        }

        string instanceName = $"@UI_{uiName}";

        if (!activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            uiInstance = GetOrCreateUIInstance(prefab, instanceName);
            activeUIs[instanceName] = uiInstance;
        }
        else
        {
            if (uiInstance == null)
            {
                uiInstance = GetOrCreateUIInstance(prefab, instanceName);
                activeUIs[instanceName] = uiInstance;
            }
            else
            {
                uiInstance.SetActive(true);
                CancelUITimer(instanceName); // 타이머 취소
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
        CancelUITimer(instanceName);

        var cts = new CancellationTokenSource();
        removeTimers[instanceName] = cts;

        ExpiredUI(instanceName, UI_REMOVE_DELAY, cts.Token).Forget();
    }


    private async UniTask ExpiredUI(string instanceName, float delay, CancellationToken token)
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

    // 삭제 취소
    private void CancelUITimer(string instanceName)
    {
        if (removeTimers.TryGetValue(instanceName, out var cts))
        {
            cts.Cancel();
            removeTimers.Remove(instanceName);
        }
    }

    public void ShowError(string message)
    {
        // 데이터를 불러오는데 실패했다는 에러 메시지를 표시하는 UI
        Debug.LogError(message);
    }
}
