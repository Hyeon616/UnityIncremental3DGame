using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stage : MonoBehaviour, IUpdatableUI
{
    [SerializeField] private TextMeshProUGUI StageText;
    [SerializeField] private TextMeshProUGUI StageSliderValueText;
    [SerializeField] private Slider StageSlider;

    private void OnEnable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageProgressChanged += UpdateUI;
            StageManager.Instance.OnStageChanged += UpdateUI;
        }
        if (MonsterManager.Instance != null)
        {
            MonsterManager.Instance.OnMonsterHealthChanged += OnMonsterHealthChanged;
        }
        UIManager.Instance.RegisterUpdatableUI(this);
        if (UIManager.Instance.IsDataLoaded)
        {
            UpdateUI();
        }
    }

    private void OnDisable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageProgressChanged -= UpdateUI;
            StageManager.Instance.OnStageChanged -= UpdateUI;
        }
        if (MonsterManager.Instance != null)
        {
            MonsterManager.Instance.OnMonsterHealthChanged -= OnMonsterHealthChanged;
        }
        UIManager.Instance.UnregisterUpdatableUI(this);
    }

    private void OnMonsterHealthChanged(float healthPercentage)
    {
        UpdateUI();
    }


    public void UpdateUI()
    {
       
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (StageManager.Instance == null || MonsterManager.Instance == null)
        {
            SetDefaultValues();
            return;
        }

        if (StageManager.Instance.IsBossStage())
        {
            UpdateBossProgress();
        }
        else
        {
            UpdateNormalStageProgress();
        }
    }

    private void UpdateBossProgress()
    {
        var currentMonster = MonsterManager.Instance.CurrentMonster;
        Debug.Log(currentMonster);
        Debug.Log(currentMonster.Name);
        if (currentMonster != null)
        {
            StageText.text = $"Boss : {currentMonster.Name}";
            int currentHP = currentMonster.CurrentHealth;
            int maxHP = currentMonster.Health;
            float healthPercentage = (float)currentHP / maxHP;
            StageSlider.value = healthPercentage;
            StageSliderValueText.text = $"{currentHP}/{maxHP} ({(int)(healthPercentage * 100)}%)";
        }
        else
        {
            SetDefaultValues();
        }
    }

    private void UpdateNormalStageProgress()
    {
        var stage = StageManager.Instance?.CurrentStage;
        StageText.text = $"Stage : {stage}";
        int defeatedMonsters = StageManager.Instance.MonstersDefeatedInCurrentStage;
        int totalMonsters = StageManager.TotalMonstersPerStage;
        float progressPercentage = (float)defeatedMonsters / totalMonsters;
        StageSlider.value = progressPercentage;
        StageSliderValueText.text = $"{defeatedMonsters}/{totalMonsters} ({(int)(progressPercentage * 100)}%)";
    }

    private void SetDefaultValues()
    {
        StageSlider.value = 0;
        StageSliderValueText.text = "N/A";
    }
}
