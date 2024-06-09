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
        if (activeWeapons.Count == 0)
        {
            Debug.LogError("No active weapons available for gacha.");
            return;
        }

        List<Weapon> gachaResults = new List<Weapon>();
        for (int i = 0; i < 10; i++)
        {
            Weapon selectedWeapon = SelectWeaponBasedOnRarity();
            if (selectedWeapon != null)
            {
                bool success = await WeaponManager.Instance.DrawWeapon(selectedWeapon.id);
                if (success)
                {
                    gachaResults.Add(selectedWeapon);
                    weaponInventoryUIManager.IncreaseWeaponCount(selectedWeapon);
                }
                else
                {
                    Debug.LogError($"Failed to draw weapon ID: {selectedWeapon.id}");
                }
            }
            else
            {
                Debug.LogError("Failed to select a weapon based on rarity.");
            }
        }

        // 서버에서 다시 데이터를 가져와 인벤토리 업데이트
        await WeaponManager.Instance.FetchWeapons();
        ShowGachaResults(gachaResults);
    }

    private Weapon SelectWeaponBasedOnRarity()
    {
        float randomValue = Random.value * 100f; // 0 to 100
        string selectedRarity = null;

        if (randomValue <= 0.1f) selectedRarity = "신화";
        else if (randomValue <= 0.3f) selectedRarity = "고대";
        else if (randomValue <= 0.8f) selectedRarity = "에픽";
        else if (randomValue <= 1.6f) selectedRarity = "영웅";
        else if (randomValue <= 6.7f) selectedRarity = "유물";
        else if (randomValue <= 16.9f) selectedRarity = "매직";
        else if (randomValue <= 50f) selectedRarity = "고급";
        else selectedRarity = "일반";

        var weaponsOfSelectedRarity = activeWeapons.Where(w => w.rarity == selectedRarity).ToList();

        if (weaponsOfSelectedRarity.Count == 0)
        {
            Debug.LogWarning($"No weapons found for rarity: {selectedRarity}, falling back to random selection.");
            // 전체 무기 중 무작위로 선택
            if (activeWeapons.Count > 0)
            {
                return activeWeapons[Random.Range(0, activeWeapons.Count)];
            }
            else
            {
                Debug.LogError("No active weapons available for random selection.");
                return null;
            }
        }

        return weaponsOfSelectedRarity[Random.Range(0, weaponsOfSelectedRarity.Count)];
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
            Weapon selectedWeapon = SelectWeaponBasedOnRarity();
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
