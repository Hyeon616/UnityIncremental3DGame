using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : Singleton<WeaponManager>
{
    public APISettings apiSettings;
    private List<Weapon> activeWeapons = new List<Weapon>();
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

    private async UniTask FetchWeapons()
    {
        try
        {
            string json = await GetWeapons();
            if (!string.IsNullOrEmpty(json))
            {
                List<Weapon> weapons = JsonConvert.DeserializeObject<List<Weapon>>(json);
                foreach (Weapon weapon in weapons)
                {
                    Debug.Log($"Rarity: {weapon.rarity}, Grade: {weapon.grade}, Level: {weapon.level}, AttackPower: {weapon.attack_power}, CriticalChance: {weapon.critical_chance}, CriticalDamage: {weapon.critical_damage}, MaxHealth: {weapon.max_health}, Count: {weapon.count}");
                    if (weapon.count > 0)
                    {
                        activeWeapons.Add(weapon);
                    }
                }
                UpdateInventoryUI(weapons);
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
                Debug.LogError($"Error fetching weapons: {request.error}");
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
    public async UniTask<List<Weapon>> GetActiveWeapons()
    {
        await FetchWeapons(); // Fetch weapons if not already fetched
        return activeWeapons;
    }
}
