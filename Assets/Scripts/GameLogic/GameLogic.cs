using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameLogic : Singleton<GameLogic>, INotifyPropertyChanged
{
    #region Properties
    private PlayerModel _currentPlayer;
    public PlayerModel CurrentPlayer
    {
        get => _currentPlayer;
        set
        {
            if (_currentPlayer != value)
            {
                _currentPlayer = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<MailModel> _mails = new ObservableCollection<MailModel>();
    public ObservableCollection<MailModel> Mails
    {
        get => _mails;
        private set
        {
            if (_mails != value)
            {
                _mails = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<GuildModel> _guilds = new ObservableCollection<GuildModel>();
    public ObservableCollection<GuildModel> Guilds
    {
        get => _guilds;
        private set
        {
            if (_guilds != value)
            {
                _guilds = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<FriendModel> _friends = new ObservableCollection<FriendModel>();
    public ObservableCollection<FriendModel> Friends
    {
        get => _friends;
        private set
        {
            if (_friends != value)
            {
                _friends = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<PlayerWeaponModel> _playerWeapons = new ObservableCollection<PlayerWeaponModel>();
    public ObservableCollection<PlayerWeaponModel> PlayerWeapons
    {
        get => _playerWeapons;
        private set
        {
            if (_playerWeapons != value)
            {
                _playerWeapons = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<PlayerSkillModel> _playerSkills = new ObservableCollection<PlayerSkillModel>();
    public ObservableCollection<PlayerSkillModel> PlayerSkills
    {
        get => _playerSkills;
        private set
        {
            if (_playerSkills != value)
            {
                _playerSkills = value;
                OnPropertyChanged();
            }
        }
    }

    private MissionProgressModel _missionProgress;
    public MissionProgressModel MissionProgress
    {
        get => _missionProgress;
        set
        {
            if (_missionProgress != value)
            {
                _missionProgress = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<RewardModel> _rewards = new ObservableCollection<RewardModel>();
    public ObservableCollection<RewardModel> Rewards
    {
        get => _rewards;
        private set
        {
            if (_rewards != value)
            {
                _rewards = value;
                OnPropertyChanged();
            }
        }
    }

    private string _currentStage;
    public string CurrentStage
    {
        get => _currentStage;
        set
        {
            if (_currentStage != value)
            {
                _currentStage = value;
                OnPropertyChanged();
            }
        }
    }

    public int MonstersDefeatedInCurrentStage => StageManager.Instance.MonstersDefeatedInCurrentStage;
    public int TotalMonstersPerStage => StageManager.TotalMonstersPerStage;

    public event Action OnStageProgressChanged;
    public event Action OnStageChanged;
    #endregion

    #region Events
    public event PropertyChangedEventHandler PropertyChanged;
    public event Action OnDataLoaded;
    public event Action<int> OnPlayerHealthChanged;
    public event Action<int> OnMonsterHealthChanged;
    public event Action<MonsterModel> OnMonsterDefeated;
    public event Action OnPlayerDefeated;
    public event Action<int, int, int> OnPlayerRewardsUpdated;
    public event Action OnPlayerDataUpdated;
    public event Action OnPlayerSkillsUpdated;


    #endregion

    #region Methods
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }

    public void UpdateCollection<T>(ObservableCollection<T> collection, ObservableCollection<T> newItems)
    {
        collection.Clear();
        foreach (var item in newItems)
        {
            collection.Add(item);
        }
    }

    public void NotifyDataLoaded()
    {

        OnDataLoaded?.Invoke();
    }


    public void OnCurrentStageLoaded(string currentStage)
    {
        CurrentStage = currentStage;
        CurrentPlayer.attributes.current_stage = currentStage;
        OnStageChanged?.Invoke();
    }

    public async void OnCurrentStageUpdated(string newStage)
    {
        CurrentStage = newStage;
        CurrentPlayer.attributes.current_stage = newStage;
        await ResourceManager.Instance.UpdateCurrentStage(newStage);
        OnStageChanged?.Invoke();
    }

    private int upgradeCost = 1;

    public int GetUpgradeCost()
    {
        return upgradeCost;
    }



    public void UpdatePlayerData(PlayerModel updatedPlayerData)
    {
        CurrentPlayer = updatedPlayerData;
        OnPlayerDataUpdated?.Invoke();
        UIManager.Instance.UpdateAllUIs();  // 모든 UI 업데이트
    }


    public async void UpgradePlayer()
    {
        CurrentPlayer.attributes.level++;
        CurrentPlayer.attributes.attack_power *= 2;
        CurrentPlayer.attributes.max_health *= 2;
        CurrentPlayer.attributes.critical_chance *= 2;
        CurrentPlayer.attributes.critical_damage *= 2;

        OnPropertyChanged(nameof(CurrentPlayer));
        OnPlayerDataUpdated?.Invoke();
        await ResourceManager.Instance.UpdatePlayerAttributes(CurrentPlayer);

        //if (CurrentPlayer.attributes.money >= upgradeCost)
        //{
        //    CurrentPlayer.attributes.money -= upgradeCost;
        //    CurrentPlayer.attributes.level++;
        //    CurrentPlayer.attributes.attack_power *= 2;
        //    CurrentPlayer.attributes.max_health *= 2;
        //    CurrentPlayer.attributes.critical_chance *= 2;
        //    CurrentPlayer.attributes.critical_damage *= 2;

        //    upgradeCost *= 2;

        //    OnPropertyChanged(nameof(CurrentPlayer));
        //    await ResourceManager.Instance.UpdatePlayerAttributes(CurrentPlayer);
        //}
        //else
        //{
        //    Debug.Log("Not enough money to upgrade");
        //}
    }

    public static string FormatKoreanCurrency(long amount)
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

    #endregion

    #region Callbacks
    public void OnPlayerDataLoaded(PlayerModel playerData)
    {
        if (playerData != null)
        {
            CurrentPlayer = playerData;
            OnPlayerDataUpdated?.Invoke();
            UIManager.Instance.UpdateAllUIs();
        }
        else
        {
            Debug.LogError("Received null player data in OnPlayerDataLoaded");
        }
    }

    public void UpdatePlayerSkills(ObservableCollection<PlayerSkillModel> skills)
    {
        UpdateCollection(PlayerSkills, skills);
        OnPlayerSkillsUpdated?.Invoke();
    }

    public void OnMailsLoaded(ObservableCollection<MailModel> mails)
    {
        if (mails != null)
        {
            UpdateCollection(Mails, mails);
        }
        else
        {
            Debug.LogWarning("Received null mails collection");
            Mails.Clear();
        }
    }

    public void OnGuildsLoaded(ObservableCollection<GuildModel> guilds)
    {
        UpdateCollection(Guilds, guilds);
    }

    public void OnFriendsLoaded(ObservableCollection<FriendModel> friends)
    {
        UpdateCollection(Friends, friends);
    }

    public void OnPlayerWeaponsLoaded(ObservableCollection<PlayerWeaponModel> weapons)
    {
        UpdateCollection(PlayerWeapons, weapons);
    }

    public void OnPlayerSkillsLoaded(ObservableCollection<PlayerSkillModel> skills)
    {
        UpdateCollection(PlayerSkills, skills);
    }

    public void OnMissionProgressLoaded(MissionProgressModel missionProgress)
    {
        MissionProgress = missionProgress;
    }

    public void OnRewardsLoaded(ObservableCollection<RewardModel> rewards)
    {
        UpdateCollection(Rewards, rewards);
    }

    public void OnRewardClaimed()
    {
        Debug.Log("Reward claimed successfully");
    }

    public void OnAttendanceRewardClaimed(AttendanceRewardResponse response)
    {
        Debug.Log($"Attendance reward claimed: {response.message}, Day count: {response.dayCount}");
    }


    #endregion
}