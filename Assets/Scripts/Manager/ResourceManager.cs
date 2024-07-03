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

    public async UniTask LoadAllData(int userId)
    {
        try
        {
            await LoadPlayerData(userId);
            await LoadMails(userId);
            await LoadGuilds();
            await LoadFriends(userId);
            await LoadPlayerWeapons(userId);
            await LoadPlayerSkills(userId);
            await LoadMissionProgress(userId);
            //await LoadStages();
            await LoadCurrentStage(userId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading data: {ex.Message}");
        }
    }

    public async UniTask LoadPlayerData(int playerId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerData, playerId);
        Debug.Log($"Attempting to load player data from URL: {url}");
        await LoadData<PlayerModel>(url, GameLogic.Instance.OnPlayerDataLoaded);
    }


    public async UniTask LoadMails(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Mails, userId);
        Debug.Log($"Loading mails from URL: {url}");
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
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerSkills);
        Debug.Log($"Loading player skills from URL: {url}");
        await LoadData<ObservableCollection<PlayerSkillModel>>(url, GameLogic.Instance.OnPlayerSkillsLoaded);
    }


    public async UniTask LoadMissionProgress(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.MissionProgress, userId);
        try
        {
            await LoadData<MissionProgressModel>(url, GameLogic.Instance.OnMissionProgressLoaded);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load mission progress: {ex.Message}. Using default progress.");
            // 기본 미션 진행 상황 생성
            var defaultProgress = new MissionProgressModel
            {
                player_id = userId,
                level_progress = 0,
                combat_power_progress = 0,
                awakening_progress = 0,
                online_time_progress = 0,
                weapon_level_sum_progress = 0,
                last_online_time_check = DateTime.UtcNow,
                total_online_time = 0
            };
            GameLogic.Instance.OnMissionProgressLoaded(defaultProgress);
        }
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



    public async UniTask ClaimAttendanceReward()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.AttendanceReward);
        await LoadData<AttendanceRewardResponse>(url, GameLogic.Instance.OnAttendanceRewardClaimed);
    }

    private async void OnAttendanceRewardResponse(string jsonResponse)
    {
        AttendanceRewardResponse response = JsonConvert.DeserializeObject<AttendanceRewardResponse>(jsonResponse);
        GameLogic.Instance.OnAttendanceRewardClaimed(response);

        // 메일 목록을 새로고침합니다.
        await LoadMails(GameManager.Instance.GetUserId());
    }

    #endregion LoadData

    #region Logic
    private async UniTask LoadData<T>(string url, Action<T> onSuccess)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {GameManager.Instance.GetAuthToken()}");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
                Debug.LogError($"URL: {url}");
                Debug.LogError($"Response Code: {www.responseCode}");
                Debug.LogError($"Response Body: {www.downloadHandler.text}");
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                Debug.Log($"Successful response from {url}: {jsonResult}");
                if (!string.IsNullOrEmpty(jsonResult))
                {
                    T data = JsonConvert.DeserializeObject<T>(jsonResult);
                    onSuccess?.Invoke(data);
                }
                else
                {
                    Debug.LogWarning($"Empty response from {url}");
                    onSuccess?.Invoke(default(T));
                }
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
            www.SetRequestHeader("Authorization", $"Bearer {GameManager.Instance.GetAuthToken()}");

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

    #endregion Logic

    public async UniTask UpdateOnlineTime()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.UpdateOnlineTime);
        var requestData = new { type = "online_time", value = 0 };
        await PostData(url, requestData, () => {
            Debug.Log("Online time updated successfully");
        });
    }

    public async UniTask CheckMissionCompletion()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.UpdateMissionProgress);
        await PostData<object>(url, null, OnMissionCompletionChecked);
    }

    private void OnMissionCompletionChecked()
    {
        // 메일 목록을 새로고침합니다.
        LoadMails(GameManager.Instance.GetUserId());
    }

}
