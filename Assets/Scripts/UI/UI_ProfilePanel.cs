using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProfilePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerLevel;
    [SerializeField] private TextMeshProUGUI PlayerRank;
    [SerializeField] private TextMeshProUGUI PlayerNickName;
    [SerializeField] private TextMeshProUGUI PlayerCombatPower;
    [SerializeField] private Slider HPSlider;
    [SerializeField] private TextMeshProUGUI HPSliderText;

    void OnEnable()
    {
        //UpdateProfile();
        if (GameLogic.Instance != null)
        {
            GameLogic.Instance.OnPlayerHealthChanged += UpdateHP;
        }
    }

    void OnDisable()
    {
        if (GameLogic.Instance != null)
        {
            GameLogic.Instance.OnPlayerHealthChanged -= UpdateHP;
        }
    }

    private void UpdateProfile()
    {
        var player = GameLogic.Instance?.CurrentPlayer;
        if (player == null)
        {
            PlayerLevel.text = "N/A";
            PlayerRank.text = "N/A";
            PlayerNickName.text = "N/A";
            UpdateHP(0);
            return;
        }

        PlayerLevel.text = player.attributes.level.ToString();
        PlayerRank.text = player.attributes.combat_power.ToString();
        PlayerNickName.text = player.player_nickname;
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
