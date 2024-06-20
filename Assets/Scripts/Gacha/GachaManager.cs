using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;
using System;

public class GachaManager : MonoBehaviour
{
    [SerializeField] private WeaponInventoryUIManager weaponInventoryUIManager;
    [SerializeField] private GameObject gachaResultPanel;
    [SerializeField] private Transform gachaResultContainer;
    [SerializeField] private GameObject gachaResultSlotPrefab;
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private float resultDelay = 0.2f;
    private List<Weapon> activeWeapons = new List<Weapon>();

    private void OnEnable()
    {
        gachaButton.onClick.AddListener(PerformGacha);
        closeButton.onClick.AddListener(CloseGachaResults);
    }

    private void OnDisable()
    {
        gachaButton.onClick.RemoveListener(PerformGacha);
        closeButton.onClick.RemoveListener(CloseGachaResults);
    }

    private void Start()
    {
        FetchAllWeapons();
    }

    private void FetchAllWeapons()
    {
        activeWeapons = WeaponManager.Instance.GetAllWeapons();
        Debug.Log($"Fetched {activeWeapons.Count} weapons.");
    }

    private void FetchActiveWeapons()
    {
        activeWeapons = WeaponManager.Instance.GetActiveWeapons();
        Debug.Log($"Fetched {activeWeapons.Count} active weapons.");
    }

    public async void PerformGacha()
    {
        try
        {
            await WeaponManager.Instance.FetchAllWeapons();
            List<Weapon> allWeapons = WeaponManager.Instance.GetAllWeapons();

            gachaResultPanel.SetActive(true); // 패널을 미리 활성화

            for (int i = 0; i < 10; i++)
            {
                Weapon selectedWeapon = await WeaponManager.Instance.GetRandomWeaponFromList(allWeapons);
                if (selectedWeapon != null)
                {
                    weaponInventoryUIManager.IncreaseWeaponCount(selectedWeapon);
                    weaponInventoryUIManager.ActivateWeaponSlot(selectedWeapon); // 슬롯 활성화
                    ShowGachaResult(selectedWeapon);
                    await UniTask.Delay((int)(resultDelay * 1000)); // 지연 시간 추가
                }
                else
                {
                    Debug.LogError("Failed to select a weapon based on rarity.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception during PerformGacha: {ex.Message}");
        }
    }

    private void ShowGachaResult(Weapon weapon)
    {
        GameObject slot = Instantiate(gachaResultSlotPrefab, gachaResultContainer);
        GachaResultSlot slotScript = slot.GetComponent<GachaResultSlot>();
        slotScript.SetSlot(weapon);
    }

    private async UniTask<Weapon> SelectWeaponBasedOnRarity()
    {
        return await WeaponManager.Instance.GetRandomWeapon();
    }

    public void CloseGachaResults()
    {
        foreach (Transform child in gachaResultContainer)
        {
            Destroy(child.gameObject);
        }
        gachaResultPanel.SetActive(false);
    }

}
