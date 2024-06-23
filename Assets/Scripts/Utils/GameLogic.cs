using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
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

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async UniTask LoadPlayerData(int playerId)
    {
        string url = ResourceManager.Instance.APISettings.PlayerDataUrl(playerId);
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
        string url = $"{ResourceManager.Instance.APISettings.MailsUrl(userId)}";
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
        string url = $"{ResourceManager.Instance.APISettings.GuildsUrl}";
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
        string url = $"{ResourceManager.Instance.APISettings.FriendsUrl(userId)}";
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
        string url = $"{ResourceManager.Instance.APISettings.PlayerWeaponsUrl(userId)}";
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
        string url = $"{ResourceManager.Instance.APISettings.PlayerSkillsUrl(userId)}";
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
        string url = $"{ResourceManager.Instance.APISettings.PlayerBlessingsUrl(userId)}";
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
        string url = $"{ResourceManager.Instance.APISettings.MissionProgressUrl(userId)}";
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
        string url = $"{ResourceManager.Instance.APISettings.RewardsUrl}";
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
        string url = $"{ResourceManager.Instance.APISettings.ClaimRewardUrl}";
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

    private void UpdateCollection<T>(ObservableCollection<T> collection, List<T> newItems)
    {
        collection.Clear();
        foreach (var item in newItems)
        {
            collection.Add(item);
        }
    }
}
