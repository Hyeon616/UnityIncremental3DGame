using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_BackButton : MonoBehaviour
{
    private Button backButton;
    private UIPrefab uiPrefab;

    private void OnEnable()
    {
        backButton = GetComponent<Button>();

        Transform uiRoot = FindUIRoot(transform);

        if (uiRoot != null)
        {
            string uiName = uiRoot.name.Replace("@UI_", "").Replace("(Clone)", "");
            if (Enum.TryParse(uiName, out UIPrefab parsedUIPrefab))
            {
                uiPrefab = parsedUIPrefab;
            }
            else
            {
                Debug.LogError($"Failed to parse UI name: {uiName}");
            }
        }
        else
        {
            Debug.LogError($"Failed to find UI root for: {gameObject.name}");
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private Transform FindUIRoot(Transform current)
    {
        while (current != null && current.parent != null)
        {
            if (current.parent.name == "@UI_Canvas")
            {
                return current;
            }
            current = current.parent;
        }
        return null;
    }

    private void OnDisable()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }

    private void OnBackButtonClicked()
    {
        if (uiPrefab != 0) // UIPrefab의 기본값이 0이라고 가정
        {
            UIManager.Instance.HideUI(uiPrefab);
        }
        else
        {
            Debug.LogError($"UI Prefab is not set for: {gameObject.name}. Cannot hide UI.");
        }
    }
}
