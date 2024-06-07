using UnityEngine;

public class WeaponInventoryUIManager : MonoBehaviour
{
    [SerializeField] private WeaponInventorySlot[] weaponSlots;
    private WeaponInventory weaponInventory;

    private void Start()
    {
        weaponInventory = new WeaponInventory();

        // Test data
        weaponInventory.AddWeapon("Normal_Lower", new Weapon(WeaponRarity.Normal, WeaponGrade.Lower, 1, 2, 10, 0.1f, 1.5f, 100));
        weaponInventory.AddWeapon("Magic_Intermediate", new Weapon(WeaponRarity.Magic, WeaponGrade.Intermediate, 1, 1, 15, 0.2f, 2.0f, 150));

        InitializeSlots();
        UpdateUI();
    }

    private void InitializeSlots()
    {
        WeaponRarity[] rarities = (WeaponRarity[])System.Enum.GetValues(typeof(WeaponRarity));
        WeaponGrade[] grades = (WeaponGrade[])System.Enum.GetValues(typeof(WeaponGrade));

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            WeaponRarity rarity = rarities[i / grades.Length];
            WeaponGrade grade = grades[i % grades.Length];

            weaponSlots[i].Initialize(rarity, grade);
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            WeaponInventorySlot slot = weaponSlots[i];
            string slotKey = $"{slot.Rarity}_{slot.Grade}";
            Weapon weapon = weaponInventory.GetWeapon(slotKey);
            if (weapon != null)
            {
                slot.SetSlot(weapon, true);
            }
            else
            {
                slot.SetSlot(null, false);
            }
        }
    }
}
