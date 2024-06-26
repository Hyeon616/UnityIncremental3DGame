using Cysharp.Threading.Tasks;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : UnitySingleton<UIManager>
{
    [SerializeField] private GameObject loadingUIPrefab;

    private GameObject loadingUIInstance;
    private const string LOADING_UI_NAME = "@UI_Loading";
    private const string CANVAS_NAME = "@UI_Canvas";
    private bool isLoadingUIActive = false;
    private float loadingUIDeactivationTime;

    public async UniTask ShowLoadingUI()
    {
        if (loadingUIInstance == null)
        {
            Canvas canvas = FindObjectsOfType<Canvas>().FirstOrDefault(c => c.name == CANVAS_NAME);
            if (canvas != null)
            {
                loadingUIInstance = Instantiate(loadingUIPrefab, canvas.transform);
                loadingUIInstance.name = LOADING_UI_NAME;
            }
            else
            {
                Debug.LogError($"Canvas with name {CANVAS_NAME} not found.");
            }
        }
        loadingUIInstance.SetActive(true);
        isLoadingUIActive = true;
        await UniTask.CompletedTask;
    }

    public void HideLoadingUI()
    {
        if (loadingUIInstance != null)
        {
            loadingUIInstance.SetActive(false);
            isLoadingUIActive = false;
            loadingUIDeactivationTime = Time.time;
            DeleteLoadingUIAfterDelay().Forget();
        }
    }

    private async UniTaskVoid DeleteLoadingUIAfterDelay()
    {
        await UniTask.Delay(180000); // 3 minutes
        if (loadingUIInstance != null && !isLoadingUIActive && Time.time - loadingUIDeactivationTime >= 180)
        {
            Destroy(loadingUIInstance);
            loadingUIInstance = null;
        }
    }




}
