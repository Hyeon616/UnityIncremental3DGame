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

    private bool _isDataLoaded = false;
    private List<IUpdatableUI> updatableUIs = new List<IUpdatableUI>();

    private const float UI_REMOVE_DELAY = 15f;

    public bool IsDataLoaded => _isDataLoaded;

    public void InitializeUI()
    {
        ShowUI(UIPrefab.AuthenticationUI);
        GameLogic.Instance.OnDataLoaded += OnDataaLoaded;
    }
    
    private void OnDataaLoaded()
    {
        UpdateAllUIs();
        _isDataLoaded = true;
    }

    public void RegisterUpdatableUI(IUpdatableUI ui)
    {
        if (!updatableUIs.Contains(ui))
        {
            updatableUIs.Add(ui);
        }
    }

    public void UnregisterUpdatableUI(IUpdatableUI ui)
    {
        updatableUIs.Remove(ui);
    }

    public void UpdateAllUIs()
    {
        foreach (var ui in updatableUIs)
        {
            ui.UpdateUI();
        }
    }

    

    public void ShowUI(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
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
            uiInstance.name = instanceName;
        }


        return uiInstance;
    }


    public void HideUI(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
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

    public GameObject GetActiveUI(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
        string instanceName = $"@UI_{uiName}";

        if (activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            return uiInstance;
        }

        return null;
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
