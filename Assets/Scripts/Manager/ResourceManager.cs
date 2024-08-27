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
            await LoadAllStages();
            await LoadMonsterData();
            await LoadCurrentStage(userId);

            GameLogic.Instance.NotifyDataLoaded();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error loading data: {ex.Message}");
        }
    }

    public async UniTask LoadPlayerData(int playerId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerData, playerId);
        try
        {
            await LoadData<PlayerModel>(url, playerData =>
            {
                if (playerData != null)
                {
                    GameLogic.Instance.OnPlayerDataLoaded(playerData);
                    Debug.Log(playerData);
                }
                else
                {
                    throw new Exception("Received null player data");
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load player data: {ex.Message}");
            throw;
        }
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
        try
        {
            await LoadData<ObservableCollection<PlayerWeaponModel>>(url, GameLogic.Instance.OnPlayerWeaponsLoaded);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load player weapons: {ex.Message}");
        }
    }

    public async UniTask LoadPlayerSkills(int userId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerSkills);
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
            Debug.LogWarning($"Failed to load mission progress: {ex.Message}. Mission progress may not exist for this user.");
        }
    }

    public async UniTask LoadAllStages()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Stages);
        await LoadData<List<Dictionary<string, string>>>(url, OnAllStagesLoaded);
    }

    public void OnAllStagesLoaded(List<Dictionary<string, string>> stages)
    {
        if (stages != null)
        {
            List<string> stageNames = new List<string>();
            foreach (var stage in stages)
            {
                if (stage.ContainsKey("Stage"))
                {
                    stageNames.Add(stage["Stage"]);
                }
                else
                {
                    Debug.LogWarning("Stage dictionary does not contain key 'Stage'");
                }
            }
            StageManager.Instance.LoadAllStages(stageNames);
        }
        else
        {
            Debug.LogWarning("Received null stages data");
            StageManager.Instance.LoadAllStages(new List<string>());
        }
    }

    public async UniTask LoadCurrentStage(int playerId)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.CurrentStage, playerId);
        try
        {
            await LoadData<Dictionary<string, string>>(url, data =>
            {
                if (data != null && data.ContainsKey("current_stage"))
                {
                    GameLogic.Instance.CurrentPlayer.attributes.current_stage = data["current_stage"];
                    StageManager.Instance.SetCurrentStage(data["current_stage"]);
                }
                else
                {
                    Debug.LogWarning("Received invalid current stage data");
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load current stage: {ex.Message}");
        }
    }

    public async UniTask UpdateCurrentStage(string newStage)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.UpdateStage);
        var requestData = new { userId = GameManager.Instance.GetUserId(), stage = newStage };
        await PostData(url, requestData, null);
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

        await LoadMails(GameManager.Instance.GetUserId());
    }

    public async UniTask LoadMonsterData()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.Monsters);
        try
        {
            await LoadData<List<MonsterModel>>(url, monsters =>
            {
                if (monsters != null && MonsterManager.Instance != null)
                {
                    MonsterManager.Instance.LoadMonsters(monsters);
                }
                else
                {
                    Debug.LogWarning("Received null monster data or MonsterManager is null");
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load monster data: {ex.Message}");
            Debug.LogWarning($"Stack trace: {ex.StackTrace}");
        }
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
                Debug.LogWarning($"Error: {www.error}");
                Debug.LogWarning($"URL: {url}");
                Debug.LogWarning($"Response Code: {www.responseCode}");
                Debug.LogWarning($"Response Body: {www.downloadHandler.text}");
                throw new Exception($"Web request failed: {www.error}");
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(jsonResult))
                {
                    try
                    {
                        var settings = new JsonSerializerSettings
                        {
                            Converters = new List<JsonConverter> { new BigIntConverter() }
                        };
                        T data = JsonConvert.DeserializeObject<T>(jsonResult);
                        onSuccess?.Invoke(data);
                    }
                    catch (JsonException ex)
                    {
                        Debug.LogWarning($"JSON Deserialization error: {ex.Message}");
                        Debug.LogWarning($"JSON string: {jsonResult}");
                        throw;
                    }
                }
                else
                {
                    Debug.LogWarning($"Empty response from {url}");
                    onSuccess?.Invoke(default(T));
                }
            }
        }
    }

    private async UniTask<string> PostData<T>(string url, T requestData, Action onSuccess)
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
                return null;
            }
            else
            {
                onSuccess?.Invoke();
                return www.downloadHandler.text;
            }
        }
    }

    private async UniTask<object> PutData<T>(string url, T requestData)
    {
        string jsonData = JsonConvert.SerializeObject(requestData);
        Debug.Log($"PUT request to {url} with data: {jsonData}");
        using (UnityWebRequest www = UnityWebRequest.Put(url, jsonData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {GameManager.Instance.GetAuthToken()}");
            Debug.Log($"Authorization header: Bearer {GameManager.Instance.GetAuthToken()}");

            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Error: {www.error}");
                Debug.LogWarning($"Response: {www.downloadHandler.text}");
                throw new Exception(www.error);
            }
            else
            {
                return JsonConvert.DeserializeObject(www.downloadHandler.text);
            }
        }
    }

    private async UniTask<TResponse> PutData<TRequest, TResponse>(string url, TRequest requestData)
    {
        string jsonData = JsonConvert.SerializeObject(requestData);
        using (UnityWebRequest www = UnityWebRequest.Put(url, jsonData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {GameManager.Instance.GetAuthToken()}");

            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Error: {www.error}");
                Debug.LogWarning($"Response: {www.downloadHandler.text}");
                throw new Exception(www.error);
            }
            else
            {
                return JsonConvert.DeserializeObject<TResponse>(www.downloadHandler.text);
            }
        }
    }
    #endregion Logic

    public async UniTask UpdatePlayerAttributes(PlayerModel playerData)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.PlayerData, playerData.player_id);
        var requestData = new
        {
            base_money = playerData.attributes.base_money,
            base_element_stone = playerData.attributes.base_element_stone,
            base_attack_power = playerData.attributes.base_attack_power,
            base_max_health = playerData.attributes.base_max_health,
            base_critical_chance = playerData.attributes.base_critical_chance,
            base_critical_damage = playerData.attributes.base_critical_damage,
            level = playerData.attributes.level,
            equipped_skill1_id = playerData.attributes.equipped_skill1_id,
            equipped_skill2_id = playerData.attributes.equipped_skill2_id,
            equipped_skill3_id = playerData.attributes.equipped_skill3_id
        };
        try
        {
            var result = await PutData<object>(url, requestData);
            if (result != null)
            {
                var updatedPlayerData = JsonConvert.DeserializeObject<PlayerModel>(result.ToString());
                GameLogic.Instance.OnPlayerDataLoaded(updatedPlayerData);

                // 업데이트된 데이터를 로컬 PlayerModel에 반영
                playerData.attributes.base_money = updatedPlayerData.attributes.base_money;
                playerData.attributes.base_element_stone = updatedPlayerData.attributes.base_element_stone;
                playerData.attributes.base_attack_power = updatedPlayerData.attributes.base_attack_power;
                playerData.attributes.base_max_health = updatedPlayerData.attributes.base_max_health;
                playerData.attributes.base_critical_chance = updatedPlayerData.attributes.base_critical_chance;
                playerData.attributes.base_critical_damage = updatedPlayerData.attributes.base_critical_damage;
                playerData.attributes.combat_power = updatedPlayerData.attributes.combat_power;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to update player attributes: {ex.Message}");
        }
    }

    public async UniTask<PlayerModel> ResetAbilities(int abilityIndex)
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.ResetAbilities, GameLogic.Instance.CurrentPlayer.player_id);
        try
        {
            var requestData = new { playerId = GameLogic.Instance.CurrentPlayer.player_id, abilityIndex = abilityIndex };
            Debug.Log($"Sending reset request with player ID: {requestData.playerId}");
            var result = await PutData<object, PlayerModel>(url, requestData);
            Debug.Log($"Reset result received: {(result != null ? "not null" : "null")}");
            if (result != null)
            {
                Debug.Log($"Reset successful. New player data: {JsonUtility.ToJson(result)}");
                GameLogic.Instance.OnPlayerDataLoaded(result);

                return result;
            }
            else
            {
                Debug.LogWarning("Received null response when resetting abilities.");
                return null;
            }
        }
        catch (UnityWebRequestException ex)
        {
            Debug.LogWarning($"Failed to reset abilities: {ex.Message}");
            Debug.LogWarning($"Response: {ex.Text}");
            Debug.LogWarning($"Response Code: {ex.ResponseCode}");
            Debug.LogWarning($"Result: {ex.Result}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Unexpected error when resetting abilities: {ex.Message}");
            return null;
        }
    }



    public async UniTask UpdateOnlineTime()
    {
        string url = APISettings.GetUrl(APISettings.Endpoint.UpdateOnlineTime);
        var requestData = new { type = "online_time", value = 0 };
        try
        {
            await PostData(url, requestData, () =>
            {
                Debug.Log("Online time updated successfully");
            });
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to update online time: {ex.Message}");
        }
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

    public class BigIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(long) || objectType == typeof(long?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                if (long.TryParse((string)reader.Value, out long result))
                {
                    return result;
                }
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToInt64(reader.Value);
            }

            throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }


}
