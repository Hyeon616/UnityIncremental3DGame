using Cysharp.Threading.Tasks;
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
    private ObservableCollection<PlayerBlessingModel> _playerBlessings = new ObservableCollection<PlayerBlessingModel>();
    private MissionProgressModel _missionProgress;
    private ObservableCollection<RewardModel> _rewards = new ObservableCollection<RewardModel>();
    private ObservableCollection<StageModel> _stages = new ObservableCollection<StageModel>();


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

    public ObservableCollection<PlayerBlessingModel> PlayerBlessings
    {
        get => _playerBlessings;
        private set
        {
            if (_playerBlessings != value)
            {
                _playerBlessings = value;
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

    public ObservableCollection<StageModel> Stages
    {
        get => _stages;
        private set
        {
            if (_stages != value)
            {
                _stages = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event Action<int> OnPlayerHealthChanged;
    public event Action<int> OnMonsterHealthChanged;
    public event Action<int> OnPlayerManaChanged;
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

    
    private async UniTask HandleCombat(PlayerModel player, MonsterModel monster)
    {
        while (player.attributes.max_health > 0 && monster.health > 0)
        {
            // 플레이어가 몬스터를 공격
            int playerDamage = CalculateDamage(player);
            monster.health -= playerDamage;
            OnMonsterHealthChanged?.Invoke(monster.health);

            if (monster.health <= 0)
            {
                OnMonsterDefeated?.Invoke(monster);
                AwardPlayer(player, monster.drop_table);
                break;
            }

            // 몬스터가 플레이어를 공격
            int monsterDamage = CalculateDamage(monster);
            player.attributes.max_health -= monsterDamage;
            OnPlayerHealthChanged?.Invoke(player.attributes.max_health);

            if (player.attributes.max_health <= 0)
            {
                OnPlayerDefeated?.Invoke();
                break;
            }

            // MP 감소 로직 추가
            //player.attributes.max_mp -= 10; // 예시로 10씩 감소
            //OnPlayerManaChanged?.Invoke(player.attributes.max_mp);

            await UniTask.Delay(1000); // 1초마다 전투 라운드 진행
        }
    }

    private int CalculateDamage(PlayerModel player)
    {
        // 플레이어의 공격력 계산 로직
        return player.attributes.attack_power;
    }

    private int CalculateDamage(MonsterModel monster)
    {
        // 몬스터의 공격력 계산 로직
        return monster.attack_power;
    }

    private void AwardPlayer(PlayerModel player, DropTable dropTable)
    {
        // 플레이어에게 드롭 아이템 지급
        player.attributes.money += dropTable.money;
        player.attributes.star_dust += dropTable.star_dust;
        player.attributes.element_stone += dropTable.element_stone;
        OnPlayerRewardsUpdated?.Invoke(player.attributes.money, player.attributes.star_dust, player.attributes.element_stone);
    }

    #region Callbacks

    public void OnPlayerDataLoaded(PlayerModel playerData)
    {
        CurrentPlayer = playerData;
    }

    public void OnMailsLoaded(ObservableCollection<MailModel> mails)
    {
        UpdateCollection(Mails, mails);
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

    public void OnPlayerBlessingsLoaded(ObservableCollection<PlayerBlessingModel> blessings)
    {
        UpdateCollection(PlayerBlessings, blessings);
    }

    public void OnMissionProgressLoaded(MissionProgressModel missionProgress)
    {
        MissionProgress = missionProgress;
    }

    public void OnRewardsLoaded(ObservableCollection<RewardModel> rewards)
    {
        UpdateCollection(Rewards, rewards);
    }

    public void OnStagesLoaded(ObservableCollection<StageModel> stages)
    {
        UpdateCollection(Stages, stages);
    }

    public void OnCurrentStageLoaded(Dictionary<string, string> data)
    {
        if (data.TryGetValue("current_stage", out string currentStage))
        {
            CurrentPlayer.attributes.current_stage = currentStage;
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

    #endregion Callbacks

}
