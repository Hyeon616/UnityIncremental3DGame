using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class WeaponManager : UnitySingleton<WeaponManager>
{
    public APISettings apiSettings;
    private List<Weapon> activeWeapons = new List<Weapon>();
    private List<Weapon> allWeapons = new List<Weapon>();

    private void Start()
    {
        FetchAndInitializeWeapons().Forget();
    }

    public async UniTask FetchAndInitializeWeapons()
    {
        await FetchWeapons();
        await FetchAllWeapons();
        WeaponInventoryUIManager.Instance.UpdateWeaponSlots(activeWeapons);
    }

    public async UniTask FetchWeapons()
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = UnityWebRequest.Get(apiSettings.WeaponsUrl))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                activeWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);
                WeaponInventoryUIManager.Instance.UpdateWeaponSlots(activeWeapons); // UI 업데이트
            }
        }
    }

    public async UniTask FetchAllWeapons()
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = UnityWebRequest.Get(apiSettings.WeaponsUrl))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                allWeapons = JsonConvert.DeserializeObject<List<Weapon>>(json);
            }
        }
    }

    public List<Weapon> GetActiveWeapons()
    {
        return activeWeapons;
    }

    public List<Weapon> GetAllWeapons()
    {
        return allWeapons;
    }

    public async UniTask<Weapon> GetRandomWeapon()
    {
        return await GetRandomWeaponFromList(allWeapons);
    }

    
    private async UniTask<List<Weapon>> GetWeaponFromDB(string rarity)
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = UnityWebRequest.Get($"{apiSettings.GetWeaponsByRarityUrl}?rarity={rarity}"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return null;
            }
            else if (request.responseCode == 200)
            {
                return JsonConvert.DeserializeObject<List<Weapon>>(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError(request.downloadHandler.text);
                return null;
            }
        }
    }

    private async UniTask AddWeaponToInventory(Weapon weapon)
    {
        string token = PlayerPrefs.GetString("authToken");

        using (UnityWebRequest request = new UnityWebRequest(apiSettings.DrawWeaponUrl, "POST"))
        {
            var requestBody = new { weaponId = weapon.id, weapon.count, weapon.attack_power, weapon.critical_chance, weapon.critical_damage, weapon.max_health };
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
            }
            else if (request.responseCode == 200)
            {
                Debug.Log($"Weapon {weapon.id} added to inventory.");
            }
            else
            {
                Debug.LogError(request.downloadHandler.text);
            }
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
                    await FetchWeapons();
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
                    await FetchWeapons(); // Fetch updated weapons
                    WeaponInventoryUIManager.Instance.UpdateWeaponSlots(activeWeapons); // UI 업데이트
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
                    await FetchWeapons(); // Fetch updated weapons
                    WeaponInventoryUIManager.Instance.UpdateWeaponSlots(activeWeapons); // UI 업데이트
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

    public Weapon GetRandomWeaponSync()
    {
        return GetRandomWeaponFromListSync(allWeapons);
    }
    public async UniTask<Weapon> GetRandomWeaponFromList(List<Weapon> weaponList)
    {
        float randomValue = UnityEngine.Random.value * 100f;
        string selectedRarity = GetRarityBasedOnProbability(randomValue);
        var weaponsOfSelectedRarity = weaponList.Where(w => w.rarity == selectedRarity).ToList();

        if (weaponsOfSelectedRarity.Count == 0)
        {
            // 해당 희귀도의 무기가 없으면 DB에서 기본 무기를 가져옴
            var defaultWeapons = await GetWeaponFromDB(selectedRarity);
            if (defaultWeapons != null && defaultWeapons.Count > 0)
            {
                var defaultWeapon = defaultWeapons[UnityEngine.Random.Range(0, defaultWeapons.Count)];
                await AddWeaponToInventory(defaultWeapon);
                await FetchWeapons(); // Fetch updated weapons
                WeaponInventoryUIManager.Instance.ActivateWeaponSlot(defaultWeapon); // 슬롯 활성화
                return defaultWeapon;
            }
            else
            {
                Debug.LogError($"No weapons found for rarity: {selectedRarity} in WeaponDB.");
                return null;
            }
        }

        var randomWeapon = weaponsOfSelectedRarity[UnityEngine.Random.Range(0, weaponsOfSelectedRarity.Count)];
        bool success = await DrawWeapon(randomWeapon.id);
        if (success)
        {
            await FetchWeapons(); // Fetch updated weapons
            WeaponInventoryUIManager.Instance.ActivateWeaponSlot(randomWeapon); // 슬롯 활성화
            return randomWeapon;
        }
        else
        {
            return null;
        }
    }

    public Weapon GetRandomWeaponFromListSync(List<Weapon> weaponList)
    {
        float randomValue = UnityEngine.Random.value * 100f;
        string selectedRarity = GetRarityBasedOnProbability(randomValue);
        var weaponsOfSelectedRarity = weaponList.Where(w => w.rarity == selectedRarity).ToList();

        if (weaponsOfSelectedRarity.Count == 0)
        {
            // 해당 희귀도의 무기가 없으면 로컬 데이터를 사용합니다.
            return new Weapon { rarity = selectedRarity, id = -1 };
        }

        return weaponsOfSelectedRarity[UnityEngine.Random.Range(0, weaponsOfSelectedRarity.Count)];
    }

}
