using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Status : MonoBehaviour, IUpdatableUI
{
    [Header("Portrait")]
    [SerializeField] private TextMeshProUGUI CharacterLevel;

    [Header("UpgradeText")]
    [SerializeField] private TextMeshProUGUI CurrentAttackText;
    [SerializeField] private TextMeshProUGUI UpgradeAttackText;
    [SerializeField] private TextMeshProUGUI CurrentHpText;
    [SerializeField] private TextMeshProUGUI UpgradeHpText;
    [SerializeField] private TextMeshProUGUI CurrentCriticalChanceText;
    [SerializeField] private TextMeshProUGUI UpgradeCriticalChanceText;
    [SerializeField] private TextMeshProUGUI CurrentCriticalDamageText;
    [SerializeField] private TextMeshProUGUI UpgradeCriticalDamageText;

    [Header("UpgradeBtn")]

    [SerializeField] private TextMeshProUGUI UpgradeCostText;
    [SerializeField] private Button UpgradeBtn;

    private void OnEnable()
    {
        UIManager.Instance.RegisterUpdatableUI(this);
        UpgradeBtn.onClick.AddListener(OnUpgradeButtonClicked);
        GameLogic.Instance.OnPlayerDataUpdated += UpdateUI;
        if (UIManager.Instance.IsDataLoaded)
        {
            UpdateUI();
        }
    }

    private void OnDisable()
    {
        UIManager.Instance.UnregisterUpdatableUI(this);
        UpgradeBtn.onClick.RemoveListener(OnUpgradeButtonClicked);
        GameLogic.Instance.OnPlayerDataUpdated -= UpdateUI;
    }

    public void UpdateUI()
    {
        var player = GameLogic.Instance.CurrentPlayer;
        if (player == null || player.attributes == null) return;

        CharacterLevel.text = $"Lv.{player.attributes.level}";
        CurrentAttackText.text = GameLogic.FormatKoreanCurrency(player.attributes.base_attack_power);
        UpgradeAttackText.text = GameLogic.FormatKoreanCurrency(player.attributes.base_attack_power * 2);

        CurrentHpText.text = GameLogic.FormatKoreanCurrency(player.attributes.base_max_health);
        UpgradeHpText.text = GameLogic.FormatKoreanCurrency(player.attributes.base_max_health * 2);

        CurrentCriticalChanceText.text = $"{player.attributes.base_critical_chance:F2}%";
        UpgradeCriticalChanceText.text = $"{player.attributes.base_critical_chance * 2:F2}%";

        CurrentCriticalDamageText.text = $"{player.attributes.base_critical_damage:F2}%";
        UpgradeCriticalDamageText.text = $"{player.attributes.base_critical_damage * 2:F2}%";

        int upgradeCost = GameLogic.Instance.GetUpgradeCost();

        UpgradeCostText.text = GameLogic.FormatKoreanCurrency(upgradeCost);
        //UpgradeBtn.interactable = player.attributes.money >= upgradeCost;
    }

    private void OnUpgradeButtonClicked()
    {
        GameLogic.Instance.UpgradePlayer();
        UpdateUI();
    }
}
