using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ability : MonoBehaviour, IUpdatableUI
{
    [Header("AbilitySlots")]
    [SerializeField] private List<AbilitySlotUI> abilitySlots;

    [Header("TabButtons")]
    [SerializeField] private List<Button> tabButtons;

    [Header("ResetBtn")]
    [SerializeField] private TextMeshProUGUI ResetCostText;
    [SerializeField] private Button ResetBtn;

    private int currentTabIndex = 0;

    [Serializable]
    private class AbilitySlotUI
    {
        public TextMeshProUGUI AbilityItemsMaxText;
        public TextMeshProUGUI AbilityItemsRatio;
    }


    private void OnEnable()
    {
        UIManager.Instance.RegisterUpdatableUI(this);
        ResetBtn.onClick.AddListener(OnResetButtonClicked);
        GameLogic.Instance.OnPlayerDataUpdated += UpdateUI;

        for (int i = 0; i < tabButtons.Count; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => OnTabButtonClicked(index));
        }

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

        for (int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].onClick.RemoveAllListeners();
        }
    }

    public void UpdateUI()
    {
        var player = GameLogic.Instance.CurrentPlayer;
        if (player == null || player.attributes == null) return;

        UpdateAbilitySlots();
        UpdateTabButtonsVisual();

        ResetCostText.text = "100";
    }


    private void UpdateAbilitySlots()
    {
        var abilities = GameLogic.Instance.CurrentPlayer.attributes.GetAbilitySet(currentTabIndex);
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            UpdateAbilitySlot(abilitySlots[i], i < abilities.Count ? abilities[i] : null);
        }
    }

    private void UpdateAbilitySlot(AbilitySlotUI slot, string ability)
    {
        if (string.IsNullOrEmpty(ability))
        {
            slot.AbilityItemsMaxText.text = "None";
            slot.AbilityItemsRatio.text = "0%";
        }
        else
        {
            var parts = ability.Split(':');
            slot.AbilityItemsMaxText.text = TranslateAbilityName(parts[0]);
            slot.AbilityItemsRatio.text = $"{parts[1]}%";
        }
    }

    // 선택된 탭
    private void UpdateTabButtonsVisual()
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].GetComponent<Image>().color = (i == currentTabIndex) ? Color.yellow : Color.white;
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

    private void OnTabButtonClicked(int index)
    {
        currentTabIndex = index;
        UpdateUI();
    }

    private async void OnResetButtonClicked()
    {
        ResetBtn.interactable = false;
        Debug.Log("Reset");
        try
        {
            var updatedPlayer = await ResourceManager.Instance.ResetAbilities(currentTabIndex);
            if (updatedPlayer != null)
            {
                GameLogic.Instance.OnPlayerDataLoaded(updatedPlayer);
                UpdateUI();
            }
            else
            {
                Debug.LogWarning("Failed to reset abilities: Received null player data");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to reset abilities: {ex.Message}");
        }
        finally
        {
            ResetBtn.interactable = true;
        }
    }
}
