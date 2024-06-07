using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] private Button Button_LoginTabButton;
    [SerializeField] private Button Button_RegisterTabButton;
    [SerializeField] private GameObject GameObject_LoginTabUI;
    [SerializeField] private GameObject GameObject_RegisterTabUI;

    private Color32 inactiveColor = new Color32(142, 191, 224, 177); // #8EBFE0, alpha 177
    private Color32 activeColor = new Color32(170, 181, 221, 255); // #AAB5DD, alpha 255

    void Start()
    {
        //// 초기 상태 설정
        //ShowPanelA();

        // 버튼 클릭 이벤트 추가
        Button_LoginTabButton.onClick.AddListener(ShowLoginTabUI);
        Button_RegisterTabButton.onClick.AddListener(ShowRegisterTabUI);
    }

    void ShowLoginTabUI()
    {
        SetPanelState(GameObject_LoginTabUI, Button_LoginTabButton, true);
        SetPanelState(GameObject_RegisterTabUI, Button_RegisterTabButton, false);
    }

    void ShowRegisterTabUI()
    {
        SetPanelState(GameObject_LoginTabUI, Button_LoginTabButton, false);
        SetPanelState(GameObject_RegisterTabUI, Button_RegisterTabButton, true);
    }

    void SetPanelState(GameObject panel, Button button, bool isActive)
    {
        panel.SetActive(isActive);
        button.GetComponent<Image>().color = isActive ? activeColor : inactiveColor;
    }
}
