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
            maxText.text = parts[0].Replace("_", " ");
            ratioText.text = $"{parts[1]}%";
        }
    }

    private async void OnResetButtonClicked()
    {
        await ResourceManager.Instance.ResetAbilities();
        UpdateUI();
    }
}
