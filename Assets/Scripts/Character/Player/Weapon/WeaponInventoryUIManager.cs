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

        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            string slotKey = GetSlotKey(i);
            Weapon weapon = weaponInventory.GetWeapon(slotKey);
            if (weapon != null)
            {
                weaponSlots[i].SetSlot(weapon, true);
            }
            else
            {
                // 기본 무기 값으로 어둡게 표시
                weaponSlots[i].SetSlot(new Weapon(WeaponRarity.Normal, WeaponGrade.Lower, 1, 0, 0, 0, 0, 0), false);
            }
        }
    }

    private string GetSlotKey(int index)
    {
        WeaponRarity rarity = (WeaponRarity)(index / 4);
        WeaponGrade grade = (WeaponGrade)(index % 4);

        return $"{rarity}_{grade}";
    }
}
