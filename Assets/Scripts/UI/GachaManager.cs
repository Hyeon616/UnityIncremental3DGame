using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    [SerializeField] private WeaponInventoryUIManager weaponInventoryUIManager;
    [SerializeField] private GameObject gachaResultPanel;
    [SerializeField] private Transform gachaResultContainer;
    [SerializeField] private GameObject gachaResultSlotPrefab;
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button closeButton;

    private List<Weapon> activeWeapons = new List<Weapon>();

    private void OnEnable()
    {
        gachaButton.onClick.AddListener(() => PerformGacha().Forget());
        closeButton.onClick.AddListener(CloseGachaResults);
    }

    private void OnDisable()
    {
        gachaButton.onClick.RemoveListener(() => PerformGacha().Forget());
        closeButton.onClick.RemoveListener(CloseGachaResults);
    }

    private void Start()
    {
        FetchActiveWeapons().Forget();
    }

    private async UniTaskVoid FetchActiveWeapons()
    {
        activeWeapons = await WeaponManager.Instance.GetActiveWeapons();
    }

    public async UniTask PerformGacha()
    {
        if (activeWeapons.Count == 0)
        {
            Debug.LogError("No active weapons available for gacha.");
            return;
        }

        List<Weapon> gachaResults = new List<Weapon>();
        for (int i = 0; i < 10; i++)
        {
            int randomIndex = Random.Range(0, activeWeapons.Count);
            Weapon selectedWeapon = activeWeapons[randomIndex];
            gachaResults.Add(selectedWeapon);
            await weaponInventoryUIManager.IncreaseWeaponCount(selectedWeapon);
        }

        ShowGachaResults(gachaResults);
    }

    private void ShowGachaResults(List<Weapon> gachaResults)
    {
        foreach (Transform child in gachaResultContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var weapon in gachaResults)
        {
            GameObject slot = Instantiate(gachaResultSlotPrefab, gachaResultContainer);
            GachaResultSlot slotScript = slot.GetComponent<GachaResultSlot>();
            slotScript.SetSlot(weapon);
        }

        gachaResultPanel.SetActive(true);
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
