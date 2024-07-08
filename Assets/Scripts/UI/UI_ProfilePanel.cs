using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProfilePanel : MonoBehaviour, IUpdatableUI
{
    [SerializeField] private TextMeshProUGUI PlayerLevel;
    [SerializeField] private TextMeshProUGUI PlayerRank;
    [SerializeField] private TextMeshProUGUI PlayerNickName;
    [SerializeField] private TextMeshProUGUI PlayerCombatPower;
    [SerializeField] private Slider HPSlider;
    [SerializeField] private TextMeshProUGUI HPSliderText;

    private void OnEnable()
    {
        if (GameLogic.Instance != null)
        {
            GameLogic.Instance.OnPlayerHealthChanged += UpdateHP;
        }
        UIManager.Instance.RegisterUpdatableUI(this);
        if (UIManager.Instance.IsDataLoaded)
        {
            UpdateUI();
        }
    }

    private void OnDisable()
    {
        if (GameLogic.Instance != null)
        {
            GameLogic.Instance.OnPlayerHealthChanged -= UpdateHP;
        }
        UIManager.Instance.UnregisterUpdatableUI(this);
    }

    public void UpdateUI()
    {
        UpdateProfile();
        UpdateHP(GameLogic.Instance.CurrentPlayer?.attributes?.max_health ?? 0);
    }

    private void UpdateProfile()
    {
        var player = GameLogic.Instance?.CurrentPlayer;
        Debug.Log("플레이어"+player);
        if (player == null)
        {
            PlayerLevel.text = "N/A";
            PlayerRank.text = "N/A";
            PlayerNickName.text = "N/A";
            UpdateHP(0);
            return;
        }
        
        PlayerLevel.text = $"Lv.{player.attributes.level}";
        PlayerRank.text = $"{player.attributes.rank}위";
        PlayerNickName.text = player.player_nickname;
        PlayerCombatPower.text = $"{player.attributes.combat_power}";
        UpdateHP(player.attributes.max_health);
    }

    private void UpdateHP(int currentHP)
    {
        var player = GameLogic.Instance?.CurrentPlayer;
        if (player == null)
        {
            HPSlider.value = 0;
            HPSliderText.text = "0/0 (0%)";
            return;
        }

        var maxHP = player.attributes.max_health;
        float hpPercentage = maxHP > 0 ? (float)currentHP / maxHP : 0;
        HPSlider.value = hpPercentage;
        HPSliderText.text = $"{currentHP}/{maxHP} ({(int)(hpPercentage * 100)}%)";
    }


}
