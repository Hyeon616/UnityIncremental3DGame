using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BackButton : MonoBehaviour
{
    private Button backButton;
    private string uiName;

    private void OnEnable()
    {
        backButton = GetComponent<Button>();
        uiName = transform.root.name;
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

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
        UIManager.Instance.HideUI(uiName);
    }
}
