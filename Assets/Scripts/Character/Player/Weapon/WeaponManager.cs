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

    void Start()
    {
        if (apiSettings != null)
        {
            FetchWeapons().Forget();
        }
        else
        {
            Debug.LogError("APISettings is not set.");
        }
    }

    public async UniTask FetchWeapons()
    {
        try
        {
            string json = await GetWeapons();
            if (!string.IsNullOrEmpty(json))
            {
                allWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);
                Debug.Log($"Fetched {allWeapons.Count} weapons from server.");

                activeWeapons.Clear();

                foreach (Weapon weapon in allWeapons)
                {
                    Debug.Log($"Weapon: ID={weapon.id}, Rarity={weapon.rarity}, Grade={weapon.grade}, Count={weapon.count}");
                    activeWeapons.Add(weapon);
                }

                Debug.Log($"Active weapons count: {activeWeapons.Count}");
                UpdateInventoryUI(activeWeapons);
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

    private async UniTask<string> GetWeapons()
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = UnityWebRequest.Get(apiSettings.WeaponsUrl))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"무기 가져오기 중 오류 발생: {request.error}");
                return null;
            }

            return request.downloadHandler.text;
        }
    }

    private void UpdateInventoryUI(List<Weapon> weapons)
    {
        var uiManager = WeaponInventoryUIManager.Instance;
        if (uiManager != null)
        {
            uiManager.UpdateWeaponSlots(weapons);
        }
        else
        {
            Debug.LogError("WeaponInventoryUIManager.Instance is not set.");
        }
    }

    public List<Weapon> GetActiveWeapons()
    {
        return activeWeapons;
    }

    public Weapon GetWeaponById(int weaponId)
    {
        return allWeapons.FirstOrDefault(w => w.id == weaponId);
    }

    public async UniTask<bool> DrawWeapon(int weaponId)
    {
        string token = PlayerPrefs.GetString("authToken");
        using (UnityWebRequest request = new UnityWebRequest(apiSettings.DrawWeaponUrl, "POST"))
        {
            var requestBody = new { weaponId = weaponId };
            string jsonData = JsonConvert.SerializeObject(requestBody);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error drawing weapon: {request.error}");
                return false;
            }

            if (request.responseCode == 200)
            {
                return true;
            }
            else
            {
                Debug.LogError($"Failed to draw weapon: {request.downloadHandler.text}");
                return false;
            }
        }
    }

    public async UniTask<Weapon> GetRandomWeaponByRarity(string rarity)
    {
        await FetchWeapons(); // 무기 리스트를 최신화

        var weaponsOfSelectedRarity = allWeapons.Where(w => w.rarity == rarity).ToList();
        if (weaponsOfSelectedRarity.Count > 0)
        {
            return weaponsOfSelectedRarity[Random.Range(0, weaponsOfSelectedRarity.Count)];
        }
        else
        {
            Debug.LogWarning($"No weapons found for rarity: {rarity}");
            return null;
        }
    }


}
