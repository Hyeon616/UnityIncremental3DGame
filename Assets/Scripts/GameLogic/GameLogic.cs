using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

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
        private set
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
        private set
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

    #region LoadData
    public async UniTask LoadPlayerData(int playerId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.PlayerData, playerId);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                CurrentPlayer = JsonConvert.DeserializeObject<PlayerModel>(jsonResult);
            }
        }
    }

    public async UniTask LoadMails(int userId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Mails, userId);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(Mails, JsonConvert.DeserializeObject<List<MailModel>>(jsonResult));
            }
        }
    }

    public async UniTask LoadGuilds()
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Guilds);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(Guilds, JsonConvert.DeserializeObject<List<GuildModel>>(jsonResult));
            }
        }
    }

    public async UniTask LoadFriends(int userId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Friends, userId);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(Friends, JsonConvert.DeserializeObject<List<FriendModel>>(jsonResult));
            }
        }
    }

    public async UniTask LoadPlayerWeapons(int userId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.PlayerWeapons, userId);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(PlayerWeapons, JsonConvert.DeserializeObject<List<PlayerWeaponModel>>(jsonResult));
            }
        }
    }

    public async UniTask LoadPlayerSkills(int userId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.PlayerSkills, userId);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(PlayerSkills, JsonConvert.DeserializeObject<List<PlayerSkillModel>>(jsonResult));
            }
        }
    }

    public async UniTask LoadPlayerBlessings(int userId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.PlayerBlessings, userId);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(PlayerBlessings, JsonConvert.DeserializeObject<List<PlayerBlessingModel>>(jsonResult));
            }
        }
    }

    public async UniTask LoadMissionProgress(int userId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.MissionProgress, userId);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                MissionProgress = JsonConvert.DeserializeObject<MissionProgressModel>(jsonResult);
            }
        }
    }

    public async UniTask LoadRewards()
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Rewards);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(Rewards, JsonConvert.DeserializeObject<List<RewardModel>>(jsonResult));
            }
        }
    }

    public async UniTask ClaimReward(int rewardId)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.ClaimReward);
        using (UnityWebRequest www = UnityWebRequest.Post(url, new WWWForm()))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { rewardId })));
            www.uploadHandler.contentType = "application/json";
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                var mail = Mails.FirstOrDefault(m => m.id == rewardId);
                if (mail != null)
                {
                    Mails.Remove(mail);
                }
            }
        }
    }

    public async UniTask LoadStages()
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.Stages);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                UpdateCollection(Stages, JsonConvert.DeserializeObject<List<StageModel>>(jsonResult));
            }
        }
    }

    
   

    public async UniTask LoadCurrentStage()
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.CurrentStage, CurrentPlayer.player_id);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult);
                CurrentPlayer.attributes.current_stage = result["current_stage"];
            }
        }
    }



    public async UniTask UpdateCurrentStage(string stage)
    {
        string url = ResourceManager.Instance.APISettings.GetUrl(APISettings.Endpoint.UpdateStage);
        using (UnityWebRequest www = UnityWebRequest.Post(url, JsonConvert.SerializeObject(new { userId = CurrentPlayer.player_id, stage = stage })))
        {
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");
            www.SetRequestHeader("Content-Type", "application/json");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                CurrentPlayer.attributes.current_stage = stage;
            }
        }
    }

    #endregion LoadData

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

    private void UpdateCollection<T>(ObservableCollection<T> collection, List<T> newItems)
    {
        collection.Clear();
        foreach (var item in newItems)
        {
            collection.Add(item);
        }
    }
}
