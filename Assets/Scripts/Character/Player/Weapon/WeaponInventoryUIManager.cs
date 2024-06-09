using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class WeaponInventoryUIManager : Singleton<WeaponInventoryUIManager>
{
    public List<WeaponInventorySlot> weaponSlots;

    private Dictionary<int, WeaponInventorySlot> slotDictionary = new Dictionary<int, WeaponInventorySlot>();

    protected override void Awake()
    {
        base.Awake();
        InitializeSlotDictionary();
    }

    private void InitializeSlotDictionary()
    {
        slotDictionary.Clear(); // 딕셔너리 초기화

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            var slot = weaponSlots[i];
            if (i < 32) // 슬롯의 개수가 무기 개수보다 적은 경우를 방지
            {
                slot.SetSlot(new Weapon { id = i + 1 }, false); // 무기 ID를 설정
                Debug.Log($"Initializing slot. SlotName: {slot.SlotName}, WeaponId: {slot.WeaponId}");
            }

            if (slot.WeaponId != 0) // 무기 ID가 0이 아닌 경우만 추가
            {
                if (slotDictionary.ContainsKey(slot.WeaponId))
                {
                    Debug.LogWarning($"Duplicate WeaponId found: {slot.WeaponId} in WeaponInventorySlot list, SlotName: {slot.SlotName}");
                }
                else
                {
                    slotDictionary[slot.WeaponId] = slot;
                    Debug.Log($"Added WeaponId {slot.WeaponId} to slotDictionary, SlotName: {slot.SlotName}");
                }
            }
            else
            {
                Debug.LogWarning($"WeaponId is 0 for a slot in the WeaponInventorySlot list, SlotName: {slot.SlotName}");
            }
        }
    }

    public void UpdateWeaponSlots(List<Weapon> weapons)
    {
        foreach (var weapon in weapons)
        {
            if (slotDictionary.TryGetValue(weapon.id, out var slot))
            {
                slot.SetSlot(weapon, true);
                Debug.Log($"Updated slot for Weapon ID: {weapon.id}");
            }
            else
            {
                Debug.LogWarning($"No slot found for Weapon ID: {weapon.id}");
            }
        }
    }

    public void IncreaseWeaponCount(Weapon weapon)
    {
        if (slotDictionary.TryGetValue(weapon.id, out var slot))
        {
            slot.IncreaseCount();
            Debug.Log($"Increased count for Weapon ID: {weapon.id}, New Count: {slot.Count}");
        }
        else
        {
            Debug.LogError($"No slot found for Weapon ID: {weapon.id}");
        }
    }

    public async void OnDrawWeaponButtonPressed(int weaponId)
    {
        bool success = await WeaponManager.Instance.DrawWeapon(weaponId);
        if (success)
        {
            Weapon weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            if (weapon != null)
            {
                IncreaseWeaponCount(weapon);
            }

            // 서버에서 다시 데이터를 가져와 인벤토리 업데이트
            await WeaponManager.Instance.FetchWeapons();
        }
        else
        {
            Debug.LogError("Failed to draw weapon.");
        }
    }

    private async UniTask DrawWeaponAsync(int weaponId)
    {
        bool success = await WeaponManager.Instance.DrawWeapon(weaponId);
        if (success)
        {
            Weapon weapon = WeaponManager.Instance.GetWeaponById(weaponId);
            if (weapon != null)
            {
                IncreaseWeaponCount(weapon);
            }

            // 서버에서 다시 데이터를 가져와 인벤토리 업데이트
            await WeaponManager.Instance.FetchWeapons();
        }
        else
        {
            Debug.LogError("Failed to draw weapon.");
        }
    }
}
