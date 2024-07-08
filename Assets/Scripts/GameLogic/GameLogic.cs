using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameLogic : Singleton<GameLogic>, INotifyPropertyChanged
{
    private PlayerModel _currentPlayer;
    private ObservableCollection<MailModel> _mails = new ObservableCollection<MailModel>();
    private ObservableCollection<GuildModel> _guilds = new ObservableCollection<GuildModel>();
    private ObservableCollection<FriendModel> _friends = new ObservableCollection<FriendModel>();
    private ObservableCollection<PlayerWeaponModel> _playerWeapons = new ObservableCollection<PlayerWeaponModel>();
    private ObservableCollection<PlayerSkillModel> _playerSkills = new ObservableCollection<PlayerSkillModel>();
    private MissionProgressModel _missionProgress;
    private ObservableCollection<RewardModel> _rewards = new ObservableCollection<RewardModel>();
   // private ObservableCollection<StageModel> _stages = new ObservableCollection<StageModel>();


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

    //public ObservableCollection<StageModel> Stages
    //{
    //    get => _stages;
    //    private set
    //    {
    //        if (_stages != value)
    //        {
    //            _stages = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}

    public event PropertyChangedEventHandler PropertyChanged;

    public event Action OnDataLoaded;
    public event Action<int> OnPlayerHealthChanged;
    public event Action<int> OnMonsterHealthChanged;
    public event Action<MonsterModel> OnMonsterDefeated;
    public event Action OnPlayerDefeated;
    public event Action<int, int, int> OnPlayerRewardsUpdated;

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

    #region Callbacks

    public void OnPlayerDataLoaded(PlayerModel playerData)
    {
        if (playerData != null)
        {
            CurrentPlayer = playerData;
            Debug.Log($"Player data loaded in GameLogic. Player ID: {playerData.player_id}, Username: {playerData.player_username}");
            Debug.Log($"Player attributes in GameLogic: {JsonConvert.SerializeObject(playerData.attributes)}");
        }
        else
        {
            Debug.LogError("Received null player data in OnPlayerDataLoaded");
        }
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

    
    public void OnCurrentStageLoaded(Dictionary<string, string> data)
    {
        Debug.Log($"OnCurrentStageLoaded called. CurrentPlayer is null: {CurrentPlayer == null}");
        if (CurrentPlayer == null)
        {
            Debug.LogError("CurrentPlayer is null in OnCurrentStageLoaded");
            return;
        }

        if (CurrentPlayer.attributes == null)
        {
            Debug.LogError("CurrentPlayer.attributes is null in OnCurrentStageLoaded");
            return;
        }

        if (data != null && data.TryGetValue("current_stage", out string currentStage))
        {
            CurrentPlayer.attributes.current_stage = currentStage;
            Debug.Log($"Current stage updated: {currentStage}");
            OnPropertyChanged(nameof(CurrentPlayer));
        }
        else
        {
            Debug.LogError("Invalid current stage data received.");
        }
    }
    public void OnCurrentStageUpdated()
    {
        Debug.Log("Current stage updated successfully");
    }

    public void OnRewardClaimed()
    {
        Debug.Log("Reward claimed successfully");
    }

    public void OnAttendanceRewardClaimed(AttendanceRewardResponse response)
    {
        Debug.Log($"Attendance reward claimed: {response.message}, Day count: {response.dayCount}");
        // 여기서 필요한 추가 로직을 구현합니다.
        // 예: 플레이어 데이터 업데이트, UI 업데이트 등
    }

    #endregion Callbacks

}
