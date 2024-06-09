using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : Singleton<WeaponManager>
{
    public APISettings apiSettings;
    private List<Weapon> activeWeapons = new List<Weapon>();
    private List<Weapon> allWeapons = new List<Weapon>();

    protected override async void Awake()
    {
        base.Awake();
        await InitializeInventory();
    }

    public async UniTask InitializeInventory()
    {
        await FetchWeapons();
        WeaponInventoryUIManager.Instance.UpdateWeaponSlots(activeWeapons);
    }

    public async UniTask FetchWeapons()
    {
        try
        {
            string json = await GetWeapons();
            if (!string.IsNullOrEmpty(json))
            {
                activeWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);
                Debug.Log($"Fetched {activeWeapons.Count} weapons from server.");
            }
            else
            {
                Debug.LogError("Received empty JSON from server.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error fetching weapons: {ex.Message}");
        }
    }

    public async UniTask FetchAllWeapons()
    {
        try
        {
            string json = await GetAllWeapons();
            if (!string.IsNullOrEmpty(json))
            {
                allWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);
                Debug.Log($"Fetched {allWeapons.Count} weapons from WeaponDB.");
            }
            else
            {
                Debug.LogError("Received empty JSON from WeaponDB.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error fetching all weapons: {ex.Message}");
        }
    }

    public List<Weapon> GetActiveWeapons()
    {
        return activeWeapons;
    }

    public async UniTask<Weapon> GetRandomWeapon()
    {
        await FetchAllWeapons(); // 모든 무기 목록을 항상 최신으로 가져옴

        float randomValue = Random.value * 100f;
        string selectedRarity = GetRarityBasedOnProbability(randomValue);
        var weaponsOfSelectedRarity = allWeapons.Where(w => w.rarity == selectedRarity).ToList();

        if (weaponsOfSelectedRarity.Count == 0)
        {
            Debug.LogError($"No weapons found for rarity: {selectedRarity} in WeaponDB.");
            return null;
        }

        var randomWeapon = weaponsOfSelectedRarity[Random.Range(0, weaponsOfSelectedRarity.Count)];
        bool success = await DrawWeapon(randomWeapon.id);
        if (success)
        {
            await FetchWeapons(); // Fetch updated weapons
            return randomWeapon;
        }
        else
        {
            return null;
        }
    }

    private string GetRarityBasedOnProbability(float randomValue)
    {
        if (randomValue <= 0.1f) return "신화";
        else if (randomValue <= 0.3f) return "고대";
        else if (randomValue <= 0.8f) return "에픽";
        else if (randomValue <= 1.6f) return "영웅";
        else if (randomValue <= 6.7f) return "유물";
        else if (randomValue <= 16.9f) return "매직";
        else if (randomValue <= 50f) return "고급";
        else return "일반";
    }

    public async UniTask<bool> DrawWeapon(int weaponId)
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = new UnityWebRequest(apiSettings.DrawWeaponUrl, "POST"))
        {
            var requestBody = new { weaponId };
            string jsonData = JsonConvert.SerializeObject(requestBody);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return false;
            }
            else
            {
                if (request.responseCode == 200)
                {
                    return true;
                }
                else
                {
                    Debug.LogError(request.downloadHandler.text);
                    return false;
                }
            }
        }
    }

    public async UniTask<bool> SynthesizeWeapon(int weaponId)
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = new UnityWebRequest(apiSettings.SynthesizeWeaponUrl, "POST"))
        {
            var requestBody = new { weaponId };
            string jsonData = JsonConvert.SerializeObject(requestBody);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return false;
            }
            else
            {
                if (request.responseCode == 200)
                {
                    return true;
                }
                else
                {
                    Debug.LogError(request.downloadHandler.text);
                    return false;
                }
            }
        }
    }

    public async UniTask<bool> SynthesizeAllWeapons()
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = new UnityWebRequest(apiSettings.SynthesizeAllWeaponsUrl, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return false;
            }
            else
            {
                if (request.responseCode == 200)
                {
                    return true;
                }
                else
                {
                    Debug.LogError(request.downloadHandler.text);
                    return false;
                }
            }
        }
    }

    private async UniTask<string> GetWeapons()
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = new UnityWebRequest(apiSettings.WeaponsUrl, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return null;
            }
            else
            {
                return request.downloadHandler.text;
            }
        }
    }

    private async UniTask<string> GetAllWeapons()
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = new UnityWebRequest($"{apiSettings.baseUrl}/weapondb", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {token}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return null;
            }
            else
            {
                return request.downloadHandler.text;
            }
        }
    }
}
