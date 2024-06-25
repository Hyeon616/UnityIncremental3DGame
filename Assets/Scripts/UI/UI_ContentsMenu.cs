using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ContentsMenu : MonoBehaviour
{
    [SerializeField] private GameObject ContentsPanel;

    private Button _contentsMenuBtn;

    private void Awake()
    {
        _contentsMenuBtn = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _contentsMenuBtn.onClick.AddListener(OnContentsMenuBtnClick);
    }

    private void OnDisable()
    {
        _contentsMenuBtn.onClick.RemoveListener(OnContentsMenuBtnClick);
    }


    private void OnContentsMenuBtnClick()
    {
        if (ContentsPanel != null)
        {
            // 패널의 활성화 상태를 반전
            ContentsPanel.SetActive(!ContentsPanel.activeSelf);
        }

    }


}
