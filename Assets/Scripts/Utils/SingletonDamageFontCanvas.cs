using UnityEngine;
using UnityEngine.UI;

public class SingletonDamageFontCanvas : MonoBehaviour
{
    private static SingletonDamageFontCanvas _instance;

    public static SingletonDamageFontCanvas Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SingletonDamageFontCanvas>();
                if (_instance == null)
                {
                    GameObject canvasObject = new GameObject("DamageFontCanvas");
                    _instance = canvasObject.AddComponent<SingletonDamageFontCanvas>();

                    Canvas canvas = canvasObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);

                    canvasObject.AddComponent<GraphicRaycaster>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
