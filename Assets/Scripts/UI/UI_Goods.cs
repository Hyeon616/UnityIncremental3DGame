using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Goods : MonoBehaviour, IUpdatableUI
{
    [SerializeField] private TextMeshProUGUI MoneyText;
    [SerializeField] private TextMeshProUGUI RubyText;

    private void OnEnable()
    {
        UIManager.Instance.RegisterUpdatableUI(this);
        if (UIManager.Instance.IsDataLoaded)
        {
            UpdateUI();

        }
    }

    private void OnDisable()
    {
        
        UIManager.Instance.UnregisterUpdatableUI(this);
    }


    public void UpdateUI()
    {
        if (GameLogic.Instance.CurrentPlayer != null && GameLogic.Instance.CurrentPlayer.attributes != null)
        {
            MoneyText.text = FormatKoreanCurrency(GameLogic.Instance.CurrentPlayer.attributes.money);
            RubyText.text = FormatKoreanCurrency(GameLogic.Instance.CurrentPlayer.attributes.element_stone);
        }
        else
        {
            MoneyText.text = "N/A";
            RubyText.text = "N/A";
        }
    }

    private string FormatKoreanCurrency(long amount)
    {
        string[] units = new string[] { "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정" };
        int unitIndex = 0;
        long currentAmount = amount;
        string result = "";

        while (currentAmount > 0 && unitIndex < units.Length)
        {
            long currentUnitValue = currentAmount % 10000;
            if (currentUnitValue > 0)
            {
                result = currentUnitValue + units[unitIndex] + (result.Length > 0 ? " " + result : "");
            }
            currentAmount /= 10000;
            unitIndex++;
        }

        return string.IsNullOrEmpty(result) ? "0" : result;
    }

}
