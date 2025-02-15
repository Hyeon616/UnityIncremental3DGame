using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class StageManager : Singleton<StageManager>
{
    private List<string> allStages;
    public string CurrentStage { get; private set; }
    public int MonstersDefeatedInCurrentStage { get; private set; }
    public static readonly int TotalMonstersPerStage = 15;

    public event Action OnStageChanged;
    public event Action OnStageProgressChanged;

    public bool IsInitialized { get; private set; }

    public void Initialize(string initialStage = null)
    {
        if (allStages != null && allStages.Count > 0)
        {
            SetCurrentStage(initialStage ?? allStages[0]);
            IsInitialized = true;
        }
        else
        {
            throw new InvalidOperationException("Stages have not been loaded. Call LoadAllStages first.");
        }
    }

    public void LoadAllStages(List<string> stages)
    {
        allStages = new List<string>(stages);
        allStages.Sort(CompareStages);
    }

    private int CompareStages(string stage1, string stage2)
    {
        var parts1 = stage1.Split('-');
        var parts2 = stage2.Split('-');

        if (parts1.Length != 2 || parts2.Length != 2)
        {
            return string.Compare(stage1, stage2, StringComparison.Ordinal);
        }

        if (int.TryParse(parts1[0], out int chapter1) && int.TryParse(parts1[1], out int stage1Number) &&
            int.TryParse(parts2[0], out int chapter2) && int.TryParse(parts2[1], out int stage2Number))
        {
            if (chapter1 != chapter2)
            {
                return chapter1.CompareTo(chapter2);
            }
            return stage1Number.CompareTo(stage2Number);
        }

        return string.Compare(stage1, stage2, StringComparison.Ordinal);
    }

    public void SetCurrentStage(string stage)
    {
        if (allStages.Contains(stage))
        {
            CurrentStage = stage;
            MonstersDefeatedInCurrentStage = 0;
            OnStageChanged?.Invoke();
            GameLogic.Instance.OnCurrentStageUpdated(stage); 
        }
        else
        {
            throw new ArgumentException($"Invalid stage: {stage}");
        }
    }
    public void ProgressToNextStage()
    {
        string nextStage = GetNextStage();
        SetCurrentStage(nextStage);
    }

    public bool IsBossStage()
    {
        if (CurrentStage == null)
            return false;

        var stageParts = CurrentStage.Split('-');
        return stageParts.Length == 2 && int.TryParse(stageParts[1], out int stageNumber) && stageNumber == 15;
    }

    public void IncrementMonstersDefeated()
    {
        MonstersDefeatedInCurrentStage++;
        OnStageProgressChanged?.Invoke();
    }

    public void OnAllMonstersDefeated()
    {
        if (IsBossStage() || MonstersDefeatedInCurrentStage >= TotalMonstersPerStage)
        {
            string nextStage = GetNextStage();
            SetCurrentStage(nextStage);
        }
        OnStageProgressChanged?.Invoke();
    }

    public string GetNextStage()
    {
        int currentIndex = allStages.IndexOf(CurrentStage);
        if (currentIndex < allStages.Count - 1)
        {
            return allStages[currentIndex + 1];
        }
        return CurrentStage; 
    }

    public bool IsLastStage()
    {
        return CurrentStage == allStages.Last();
    }

    public void RestartCurrentStage()
    {
        MonstersDefeatedInCurrentStage = 0;
        OnStageProgressChanged?.Invoke();
    }

    public void Reset()
    {
        CurrentStage = null;
        MonstersDefeatedInCurrentStage = 0;
        allStages = null;
        IsInitialized = false;
    }

}