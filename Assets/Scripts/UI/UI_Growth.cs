using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Growth : MonoBehaviour
{
    [Header("GrowthPanelTab")]
    [SerializeField] private Button StatusBtn;
    [SerializeField] private Button AbilityBtn;

    [Header("GrowthPanel")]
    [SerializeField] private GameObject StatusPanel;
    [SerializeField] private GameObject AbilityPanel;

    private void OnEnable()
    {
        // 초기 상태 설정
        ShowStatusPanel();

        // 버튼 클릭 이벤트 추가
        StatusBtn.onClick.AddListener(ShowStatusPanel);
        AbilityBtn.onClick.AddListener(ShowAbilityPanel);
    }

    private void OnDisable()
    {
        // 버튼 클릭 이벤트 제거
        StatusBtn.onClick.RemoveListener(ShowStatusPanel);
        AbilityBtn.onClick.RemoveListener(ShowAbilityPanel);
    }

    private void ShowStatusPanel()
    {
        StatusPanel.SetActive(true);
        AbilityPanel.SetActive(false);
    }

    private void ShowAbilityPanel()
    {
        StatusPanel.SetActive(false);
        AbilityPanel.SetActive(true);
    }
}
