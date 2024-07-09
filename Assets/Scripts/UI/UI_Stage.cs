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
        UIManager.Instance.RegisterUpdatableUI(this);
        if (UIManager.Instance.IsDataLoaded)
        {
            UpdateUI();
        }

        GameLogic.Instance.OnStageProgressChanged += OnStageProgressChanged;
        GameLogic.Instance.OnStageChanged += OnStageChanged;
        GameLogic.Instance.OnMonsterHealthChanged += OnMonsterHealthChanged;
    }

    private void OnDisable()
    {
        UIManager.Instance.UnregisterUpdatableUI(this);
        GameLogic.Instance.OnStageProgressChanged -= OnStageProgressChanged;
        GameLogic.Instance.OnStageChanged -= OnStageChanged;
        GameLogic.Instance.OnMonsterHealthChanged -= OnMonsterHealthChanged;
    }

    public void UpdateUI()
    {
        UpdateStageInfo();
        UpdateMonsterProgress();
    }

    private void UpdateStageInfo()
    {
        string currentStage = GameLogic.Instance.CurrentPlayer.attributes.current_stage;
        StageText.text = $"Stage: {currentStage}";
    }

    private void UpdateMonsterProgress()
    {
        var currentMonster = GameLogic.Instance.CurrentMonster;
        if (currentMonster.Type == "Boss")
        {
            // 보스 몬스터의 체력을 슬라이더에 표시
            float progressPercentage = (float)currentMonster.Health / currentMonster.InitialHealth;
            StageSlider.value = progressPercentage;
            StageSliderValueText.text = $"{currentMonster.Health}/{currentMonster.InitialHealth} ({(int)(progressPercentage * 100)}%)";
        }
        else
        {
            // 일반 몬스터의 진행 상황을 슬라이더에 표시
            int remainingMonsters = GameLogic.Instance.MonstersDefeatedInCurrentStage;
            int totalMonsters = GameLogic.Instance.TotalMonstersPerStage;
            float progressPercentage = (float)remainingMonsters / totalMonsters;

            StageSlider.value = progressPercentage;
            StageSliderValueText.text = $"{remainingMonsters}/{totalMonsters} ({(int)(progressPercentage * 100)}%)";
        }
    }

    private bool IsBossStage()
    {
        var stageParts = GameLogic.Instance.CurrentStage.Split('-');
        if (stageParts.Length == 2 && int.TryParse(stageParts[1], out int stageNumber))
        {
            return stageNumber == 15;
        }
        return false;
    }

    private void OnStageProgressChanged()
    {
        UpdateMonsterProgress();
    }

    private void OnStageChanged()
    {
        UpdateStageInfo();
        UpdateMonsterProgress();
    }

    private void OnMonsterHealthChanged(int currentHealth)
    {
        if (IsBossStage())
        {
            UpdateMonsterProgress();
        }
    }
}
