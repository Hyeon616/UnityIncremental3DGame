using UnityEngine;
using System.Collections.Generic;

public class WeaponInventoryUIManager : Singleton<WeaponInventoryUIManager>
{
    public List<WeaponInventorySlot> slots;

    private Dictionary<string, WeaponInventorySlot> slotDictionary;

    protected override void Awake()
    {
        base.Awake();
        InitializeSlots();
    }

    public void InitializeSlots()
    {
        slotDictionary = new Dictionary<string, WeaponInventorySlot>();

        // 하드코딩된 슬롯 정보 설정
        string[] rarities = { "일반", "고급", "매직", "유물", "영웅", "에픽", "고대", "신화" };
        string[] grades = { "하급", "중급", "상급", "최상급" };

        int slotIndex = 0;
        foreach (var rarity in rarities)
        {
            foreach (var grade in grades)
            {
                if (slotIndex < slots.Count)
                {
                    slots[slotIndex].Initialize(rarity, grade);
                    string key = $"{rarity}_{grade}";
                    slotDictionary[key] = slots[slotIndex];
                    slotIndex++;
                }
            }
        }
    }

    public void UpdateWeaponSlots(List<Weapon> weapons)
    {
        Debug.Log($"Updating weapon slots with {weapons.Count} weapons");

        foreach (var weapon in weapons)
        {
            string key = $"{weapon.rarity}_{weapon.grade}";
            if (slotDictionary.TryGetValue(key, out WeaponInventorySlot slot))
            {
                slot.SetSlot(weapon, weapon.count > 0);
                Debug.Log($"Slot set with Weapon Rarity: {weapon.rarity}, Grade: {weapon.grade}, Count: {weapon.count}");
            }
            else
            {
                Debug.LogWarning($"No slot found for Weapon Rarity: {weapon.rarity}, Grade: {weapon.grade}");
            }
        }
    }
}
