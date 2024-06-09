using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;

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
        FetchActiveWeapons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TestGachaProbability(10000).Forget();
        }
    }

    private void FetchActiveWeapons()
    {
        activeWeapons = WeaponManager.Instance.GetActiveWeapons();
        Debug.Log($"Fetched {activeWeapons.Count} active weapons.");
    }
    public async void PerformGacha()
    {
        await WeaponManager.Instance.FetchWeapons();
        activeWeapons = WeaponManager.Instance.GetActiveWeapons();

        gachaResultPanel.SetActive(true); // 패널을 미리 활성화

        for (int i = 0; i < 10; i++)
        {
            Weapon selectedWeapon = await WeaponManager.Instance.GetRandomWeapon();
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

    private async UniTaskVoid TestGachaProbability(int numTests)
    {
        Dictionary<string, int> rarityCounts = new Dictionary<string, int>
        {
            { "일반", 0 },
            { "고급", 0 },
            { "매직", 0 },
            { "유물", 0 },
            { "영웅", 0 },
            { "에픽", 0 },
            { "고대", 0 },
            { "신화", 0 }
        };

        for (int i = 0; i < numTests; i++)
        {
            Weapon selectedWeapon = await WeaponManager.Instance.GetRandomWeapon();
            if (selectedWeapon != null)
            {
                rarityCounts[selectedWeapon.rarity]++;
            }
        }

        foreach (var rarity in rarityCounts.Keys)
        {
            Debug.Log($"Rarity: {rarity}, Count: {rarityCounts[rarity]}, Probability: {(float)rarityCounts[rarity] / numTests * 100}%");
        }
    }
}
