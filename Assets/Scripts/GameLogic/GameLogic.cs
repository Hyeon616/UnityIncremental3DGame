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


    #region Monster
    private int _monstersDefeatedInCurrentStage = 0;
    private const int MonstersPerStage = 15;
    private List<string> _allStages;
    private string _currentStage;
    private Dictionary<string, MonsterModel> _monsterDataByStage = new Dictionary<string, MonsterModel>();
    private MonsterModel _currentMonster;


    public int MonstersDefeatedInCurrentStage
    {
        get => _monstersDefeatedInCurrentStage;
        private set
        {
            if (_monstersDefeatedInCurrentStage != value)
            {
                _monstersDefeatedInCurrentStage = value;
                OnPropertyChanged();
                OnStageProgressChanged?.Invoke();
            }
        }
    }

    public string CurrentStage
    {
        get => _currentStage;
        private set
        {
            if (_currentStage != value)
            {
                _currentStage = value;
                OnPropertyChanged();
                OnStageChanged?.Invoke();
            }
        }
    }

    public MonsterModel CurrentMonster
    {
        get => _currentMonster;
        private set
        {
            if (_currentMonster != value)
            {
                _currentMonster = value;
                OnPropertyChanged();
                OnMonsterChanged?.Invoke();
            }
        }
    }

    public int TotalMonstersPerStage => MonstersPerStage;

    public event Action<string> OnStageCompleted;
    public event Action<int> OnMonstersDefeatedCountChanged;

    public event Action OnStageProgressChanged; // 몬스터 진행 상황 변경 이벤트
    public event Action OnStageChanged; // 스테이지 변경 이벤트
    public event Action OnMonsterChanged; // 현재 몬스터 변경 이벤트

    public void OnAllStagesLoaded(List<string> stages)
    {
        stages.Sort(CompareStages);
        _allStages = stages;
        Debug.Log($"All stages loaded: {string.Join(", ", _allStages)}");
    }

    public void OnMonsterDataLoaded(List<MonsterModel> monsters)
    {
        _monsterDataByStage.Clear();
        foreach (var monster in monsters)
        {
            monster.Initialize();
            _monsterDataByStage[monster.Stage] = monster;
        }
        Debug.Log("Monster data loaded successfully.");
    }

    private int CompareStages(string stage1, string stage2)
    {
        var stage1Parts = stage1.Split('-');
        var stage2Parts = stage2.Split('-');

        if (stage1Parts.Length != 2 || stage2Parts.Length != 2)
            return string.Compare(stage1, stage2, StringComparison.Ordinal);

        if (int.TryParse(stage1Parts[0], out int chapter1) && int.TryParse(stage2Parts[0], out int chapter2))
        {
            if (chapter1 != chapter2)
                return chapter1.CompareTo(chapter2);

            if (int.TryParse(stage1Parts[1], out int stageNumber1) && int.TryParse(stage2Parts[1], out int stageNumber2))
            {
                return stageNumber1.CompareTo(stageNumber2);
            }
        }

        return string.Compare(stage1, stage2, StringComparison.Ordinal);
    }

    public void OnCurrentStageLoaded(string currentStage)
    {
        _currentStage = currentStage;
        CurrentPlayer.attributes.current_stage = currentStage;
        Debug.Log($"Current stage loaded: {currentStage}");
        OnPropertyChanged(nameof(CurrentPlayer));

        // 현재 스테이지에 해당하는 몬스터 데이터 로드
        if (_monsterDataByStage.TryGetValue(currentStage, out var monster))
        {
            CurrentMonster = monster;
        }

        // 스테이지가 로드되었음을 알리는 이벤트 호출
        OnStageChanged?.Invoke();
    }

    public void DefeatMonster(MonsterModel monster)
    {
        if (monster.Type == "Boss")
        {
            // 보스 몬스터의 체력을 깎는다.
            CurrentMonster.Health -= 10; // 데미지 값은 예시입니다.
            if (CurrentMonster.Health <= 0)
            {
                CompleteCurrentStage();
            }
        }
        else
        {
            _monstersDefeatedInCurrentStage++;
            OnMonstersDefeatedCountChanged?.Invoke(_monstersDefeatedInCurrentStage);
            OnMonsterDefeated?.Invoke(monster);

            // 몬스터 진행 상황 변경 이벤트 호출
            OnStageProgressChanged?.Invoke();

            if (_monstersDefeatedInCurrentStage >= MonstersPerStage)
            {
                CompleteCurrentStage();
            }
        }
    }

    private async void CompleteCurrentStage()
    {
        _monstersDefeatedInCurrentStage = 0;
        OnMonstersDefeatedCountChanged?.Invoke(_monstersDefeatedInCurrentStage);

        int currentIndex = _allStages.IndexOf(_currentStage);
        if (currentIndex < _allStages.Count - 1)
        {
            string newStage = _allStages[currentIndex + 1];
            await ResourceManager.Instance.UpdateCurrentStage(newStage);

            // 스테이지가 변경되었음을 알리는 이벤트 호출
            _currentStage = newStage;
            OnStageChanged?.Invoke();

            // 새 스테이지에 해당하는 몬스터 데이터 로드
            if (_monsterDataByStage.TryGetValue(newStage, out var monster))
            {
                CurrentMonster = monster;
            }
        }
        else
        {
            Debug.Log("All stages completed!");
        }
    }

    #endregion Monster

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

            // 현재 스테이지에 해당하는 몬스터 데이터 로드
            if (_monsterDataByStage.TryGetValue(currentStage, out var monster))
            {
                CurrentMonster = monster;
            }
        }
        else
        {
            Debug.LogError("Invalid current stage data received.");
        }
    }

    public void OnCurrentStageUpdated(string newStage)
    {
        _currentStage = newStage;
        CurrentPlayer.attributes.current_stage = newStage;
        Debug.Log($"Current stage updated to: {newStage}");
        _monstersDefeatedInCurrentStage = 0;
        OnMonstersDefeatedCountChanged?.Invoke(_monstersDefeatedInCurrentStage);
        OnStageCompleted?.Invoke(newStage);
        OnPropertyChanged(nameof(CurrentPlayer));

        // 새 스테이지에 해당하는 몬스터 데이터 로드
        if (_monsterDataByStage.TryGetValue(newStage, out var monster))
        {
            CurrentMonster = monster;
        }
    }


    public void OnRewardClaimed()
    {
        Debug.Log("Reward claimed successfully");
    }

    public void OnAttendanceRewardClaimed(AttendanceRewardResponse response)
    {
        Debug.Log($"Attendance reward claimed: {response.message}, Day count: {response.dayCount}");
        
    }

    #endregion Callbacks

}
