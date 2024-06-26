using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceManager : Singleton<ResourceManager>
{
    public APISettings APISettings { get; private set; }

    private ResourceManager()
    {
        APISettings = Resources.Load<APISettings>("APISettings");
    }


    #region LoadData

    public async UniTask<bool> LoadAllData(int userId)
    {
        try
        {
            await LoadPlayerData(userId);
            await LoadMails(userId);
            await LoadGuilds();
            await LoadFriends(userId);
            await LoadPlayerWeapons(userId);
            await LoadPlayerSkills(userId);
            await LoadPlayerBlessings(userId);
            await LoadMissionProgress(userId);
            await LoadRewards();
            await LoadStages();
            await LoadCurrentStage(userId);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading data: {ex.Message}");
            return false;
        }
    }

    public async UniTask LoadPlayerData(int playerId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerData, playerId);
        await LoadData<PlayerModel>(url, GameLogic.Instance.OnPlayerDataLoaded);
    }

    public async UniTask LoadMails(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Mails, userId);
        await LoadData<ObservableCollection<MailModel>>(url, GameLogic.Instance.OnMailsLoaded);
    }

    public async UniTask LoadGuilds()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Guilds);
        await LoadData<ObservableCollection<GuildModel>>(url, GameLogic.Instance.OnGuildsLoaded);
    }

    public async UniTask LoadFriends(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Friends, userId);
        await LoadData<ObservableCollection<FriendModel>>(url, GameLogic.Instance.OnFriendsLoaded);
    }

    public async UniTask LoadPlayerWeapons(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerWeapons, userId);
        await LoadData<ObservableCollection<PlayerWeaponModel>>(url, GameLogic.Instance.OnPlayerWeaponsLoaded);
    }

    public async UniTask LoadPlayerSkills(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerSkills, userId);
        await LoadData<ObservableCollection<PlayerSkillModel>>(url, GameLogic.Instance.OnPlayerSkillsLoaded);
    }

    public async UniTask LoadPlayerBlessings(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerBlessings, userId);
        await LoadData<ObservableCollection<PlayerBlessingModel>>(url, GameLogic.Instance.OnPlayerBlessingsLoaded);
    }

    public async UniTask LoadMissionProgress(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.MissionProgress, userId);
        await LoadData<MissionProgressModel>(url, GameLogic.Instance.OnMissionProgressLoaded);
    }

    public async UniTask LoadRewards()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Rewards);
        await LoadData<ObservableCollection<RewardModel>>(url, GameLogic.Instance.OnRewardsLoaded);
    }

    public async UniTask LoadStages()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Stages);
        await LoadData<ObservableCollection<StageModel>>(url, GameLogic.Instance.OnStagesLoaded);
    }

    public async UniTask LoadCurrentStage(int playerId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.CurrentStage, playerId);
        await LoadData<Dictionary<string, string>>(url, GameLogic.Instance.OnCurrentStageLoaded);
    }

    public async UniTask UpdateCurrentStage(string stage)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.UpdateStage);
        var requestData = new { userId = GameLogic.Instance.CurrentPlayer.player_id, stage = stage };
        await PostData(url, requestData, GameLogic.Instance.OnCurrentStageUpdated);
    }

    public async UniTask ClaimReward(int rewardId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.ClaimReward);
        var requestData = new { rewardId = rewardId };
        await PostData(url, requestData, GameLogic.Instance.OnRewardClaimed);
    }

    #endregion LoadData

    #region HelperMethods

    private async UniTask LoadData<T>(string url, Action<T> onSuccess)
    {
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
                T data = JsonConvert.DeserializeObject<T>(jsonResult);
                onSuccess?.Invoke(data);
            }
        }
    }

    private async UniTask PostData<T>(string url, T requestData, Action onSuccess)
    {
        string jsonData = JsonConvert.SerializeObject(requestData);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AuthToken")}");

            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                onSuccess?.Invoke();
            }
        }
    }

    #endregion HelperMethods

}
