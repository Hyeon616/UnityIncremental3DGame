using UnityEngine;

[System.Serializable]
public class Weapon
{
    public int id;
    public string rarity;
    public string grade;
    public int level;
    public int count;
    public int attack_power;
    public float critical_chance;
    public float critical_damage;
    public int max_health;

    public string GetRarityName()
    {
        return rarity;
    }

    public string GetGradeName()
    {
        return grade;
    }
}
