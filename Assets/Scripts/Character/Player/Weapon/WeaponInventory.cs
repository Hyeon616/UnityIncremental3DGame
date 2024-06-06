using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponInventory
{
    private Dictionary<string, Weapon> weaponDictionary = new Dictionary<string, Weapon>();

    public void AddWeapon(string slotKey, Weapon weapon)
    {
        if (weaponDictionary.ContainsKey(slotKey))
        {
            weaponDictionary[slotKey] = weapon;
        }
        else
        {
            weaponDictionary.Add(slotKey, weapon);
        }
    }

    public Weapon GetWeapon(string slotKey)
    {
        weaponDictionary.TryGetValue(slotKey, out Weapon weapon);
        return weapon;
    }

    public Dictionary<string, Weapon> GetAllWeapons()
    {
        return weaponDictionary;
    }
}
