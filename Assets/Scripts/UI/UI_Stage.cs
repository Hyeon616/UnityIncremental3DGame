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
            MonsterManager.Instance.OnMonsterDefeated += OnMonsterDefeated;
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
            MonsterManager.Instance.OnMonsterDefeated -= OnMonsterDefeated;
        }
        UIManager.Instance.UnregisterUpdatableUI(this);
    }

    private void OnMonsterDefeated(MonsterModel monster) // 추가된 메서드
    {
        UpdateUI();
    }

    private void OnMonsterHealthChanged(float healthPercentage, MonsterModel monster)
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
        var activeMonsters = MonsterManager.Instance.ActiveMonsters;
        if (activeMonsters.Count > 0)
        {
            var bossMonster = activeMonsters[0]; // 보스는 하나만 있다고 가정
            StageText.text = $"Boss : {bossMonster.Name}";
            int currentHP = bossMonster.CurrentHealth;
            int maxHP = bossMonster.Health;
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
        StageSliderValueText.text = $"{defeatedMonsters}/{totalMonsters} ({Mathf.RoundToInt(progressPercentage * 100)}%)";
    }

    private void SetDefaultValues()
    {
        StageSlider.value = 0;
        StageSliderValueText.text = "N/A";
    }
}
