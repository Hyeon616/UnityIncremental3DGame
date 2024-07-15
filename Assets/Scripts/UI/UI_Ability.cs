using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ability : MonoBehaviour, IUpdatableUI
{
    [Header("AbilitySlot1")]
    [SerializeField] private TextMeshProUGUI AbilityItemsMaxText1;
    [SerializeField] private TextMeshProUGUI AbilityItemsRatio1;

    [Header("AbilitySlot1")]
    [SerializeField] private TextMeshProUGUI AbilityItemsMaxText2;
    [SerializeField] private TextMeshProUGUI AbilityItemsRatio2;

    [Header("AbilitySlot1")]
    [SerializeField] private TextMeshProUGUI AbilityItemsMaxText3;
    [SerializeField] private TextMeshProUGUI AbilityItemsRatio3;

    [Header("ResetBtn")]
    [SerializeField] private TextMeshProUGUI ResetCostText;
    [SerializeField] private Button ResetBtn;

    private void OnEnable()
    {
        UIManager.Instance.RegisterUpdatableUI(this);
        ResetBtn.onClick.AddListener(OnResetButtonClicked);
        GameLogic.Instance.OnPlayerDataUpdated += UpdateUI;
        if (UIManager.Instance.IsDataLoaded)
        {
            UpdateUI();
        }
    }

    private void OnDisable()
    {
        UIManager.Instance.UnregisterUpdatableUI(this);
        ResetBtn.onClick.RemoveListener(OnResetButtonClicked);
        GameLogic.Instance.OnPlayerDataUpdated -= UpdateUI;
    }

    public void UpdateUI()
    {
        var player = GameLogic.Instance.CurrentPlayer;
        if (player == null || player.attributes == null) return;

        UpdateAbilitySlot(AbilityItemsMaxText1, AbilityItemsRatio1, player.attributes.Ability1);
        UpdateAbilitySlot(AbilityItemsMaxText2, AbilityItemsRatio2, player.attributes.Ability2);
        UpdateAbilitySlot(AbilityItemsMaxText3, AbilityItemsRatio3, player.attributes.Ability3);

        // Reset 비용 업데이트 (예: 100 골드)
        ResetCostText.text = "100";

        
    }

    private void UpdateAbilitySlot(TextMeshProUGUI maxText, TextMeshProUGUI ratioText, string ability)
    {
        if (string.IsNullOrEmpty(ability))
        {
            maxText.text = "None";
            ratioText.text = "0%";
        }
        else
        {
            var parts = ability.Split(':');
            maxText.text = TranslateAbilityName(parts[0]);
            ratioText.text = $"{parts[1]}%";
        }
    }

    private string TranslateAbilityName(string abilityName)
    {
        switch (abilityName)
        {
            case "max_health":
                return "최대체력";
            case "attack_power":
                return "공격력";
            case "critical_damage":
                return "치명타 피해";
            case "critical_chance":
                return "치명타 확률";
            default:
                return abilityName;
        }
    }

    private async void OnResetButtonClicked()
    {
        ResetBtn.interactable = false; // 버튼 비활성화
        try
        {
            var updatedPlayer = await ResourceManager.Instance.ResetAbilities();
            if (updatedPlayer != null)
            {
                GameLogic.Instance.OnPlayerDataLoaded(updatedPlayer);
                UpdateUI();
            }
            else
            {
                Debug.LogError("Failed to reset abilities: Received null player data");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to reset abilities: {ex.Message}");
            // 여기에 사용자에게 오류를 알리는 UI 로직을 추가할 수 있습니다.
        }
        finally
        {
            ResetBtn.interactable = true; // 버튼 다시 활성화
        }
    }
}
