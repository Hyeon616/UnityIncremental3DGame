using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryUIManager : UnitySingleton<WeaponInventoryUIManager>
{
    [SerializeField] private List<WeaponInventorySlot> weaponSlots;
    [SerializeField] private Button synthesizeAllButton;
    private WeaponInventorySlot selectedSlot;

    private Dictionary<int, WeaponInventorySlot> slotDictionary = new Dictionary<int, WeaponInventorySlot>();

    protected void Awake()
    {
        InitializeSlotDictionary();
    }

    private void OnEnable()
    {
        synthesizeAllButton.onClick.AddListener(OnSynthesizeAllButtonPressed);
    }

    private void OnDisable()
    {
        synthesizeAllButton.onClick.RemoveListener(OnSynthesizeAllButtonPressed);
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
            slot.SetSlot(weapon, true); // 추가: UI 업데이트
            Debug.Log($"Increased count for Weapon ID: {weapon.id}, New Count: {slot.Count}");
        }
        else
        {
            Debug.LogError($"No slot found for Weapon ID: {weapon.id}");
        }
    }

    public void ActivateWeaponSlot(Weapon weapon)
    {
        if (slotDictionary.TryGetValue(weapon.id, out var slot))
        {
            slot.SetSlot(weapon, true);
        }
        else
        {
            Debug.LogError($"No slot found for Weapon ID: {weapon.id}");
        }
    }

    public void ShowCombineButton(WeaponInventorySlot slot)
    {
        if (selectedSlot != null && selectedSlot != slot)
        {
            selectedSlot.HideCombineButton();
        }

        selectedSlot = slot;
        selectedSlot.ShowCombineButton();
        selectedSlot.UpdateCombineButtonState();
    }

    public void OnSynthesizeButtonPressed(int weaponId)
    {
        SynthesizeWeaponAsync(weaponId).Forget();
    }

    private async UniTask SynthesizeWeaponAsync(int weaponId)
    {
        bool success = await WeaponManager.Instance.SynthesizeWeapon(weaponId);
        if (success)
        {
            await WeaponManager.Instance.FetchWeapons();
            UpdateWeaponSlots(WeaponManager.Instance.GetActiveWeapons());
        }
        else
        {
            Debug.LogError("Failed to synthesize weapon.");
        }
    }

    public void OnSynthesizeAllButtonPressed()
    {
        SynthesizeAllWeaponsAsync().Forget();
    }

    private async UniTask SynthesizeAllWeaponsAsync()
    {
        bool success = await WeaponManager.Instance.SynthesizeAllWeapons();
        if (success)
        {
            await WeaponManager.Instance.FetchWeapons();
            UpdateWeaponSlots(WeaponManager.Instance.GetActiveWeapons()); // UI 업데이트
        }
        else
        {
            Debug.LogError("Failed to synthesize all weapons.");
        }
    }
}
