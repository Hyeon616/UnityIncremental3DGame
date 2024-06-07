using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : MonoBehaviour
{
    public APISettings apiSettings;

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

    private async UniTaskVoid FetchWeapons()
    {
        try
        {
            string json = await GetWeapons();
            if (!string.IsNullOrEmpty(json))
            {
                List<Weapon> weapons = JsonConvert.DeserializeObject<List<Weapon>>(json);
                foreach (Weapon weapon in weapons)
                {
                    if (weapon != null)
                    {
                        Debug.Log($"Rarity: {weapon.rarity}, Grade: {weapon.grade}, Level: {weapon.level}, AttackPower: {weapon.attack_power}, CriticalChance: {weapon.critical_chance}, CriticalDamage: {weapon.critical_damage}, MaxHealth: {weapon.max_health}, Count: {weapon.count}");
                    }
                    else
                    {
                        Debug.LogError("Weapon object is null.");
                    }
                }
                // 여기서 weapons 리스트를 인벤토리 UI에 적용할 수 있습니다.
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
        using (UnityWebRequest request = UnityWebRequest.Get(apiSettings.apiUrl))
        {
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
}
