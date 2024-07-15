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
            MoneyText.text = GameLogic.FormatKoreanCurrency(GameLogic.Instance.CurrentPlayer.attributes.money);
            RubyText.text = GameLogic.FormatKoreanCurrency(GameLogic.Instance.CurrentPlayer.attributes.element_stone);
        }
        else
        {
            MoneyText.text = "N/A";
            RubyText.text = "N/A";
        }
    }

    

}
