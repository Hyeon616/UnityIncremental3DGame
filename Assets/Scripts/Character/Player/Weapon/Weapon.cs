using UnityEngine;

public enum WeaponRarity
{
    Normal,
    Magic,
    Rare,
    Unique,
    Epic,
    Legend,
    Star,
    Galaxy
}

public enum WeaponGrade
{
    Lower,
    Intermediate,
    Upper,
    Highest
}

[System.Serializable]
public class Weapon
{
    public WeaponRarity Rarity;
    public WeaponGrade Grade;
    public int Level;
    public int Count;

    public int AttackPower;
    public float CriticalChance;
    public float CriticalDamage;
    public int MaxHealth;

    public Weapon(WeaponRarity rarity, WeaponGrade grade, int level, int count, int attackPower, float criticalChance, float criticalDamage, int maxHealth)
    {
        Rarity = rarity;
        Grade = grade;
        Level = level;
        Count = count;
        AttackPower = attackPower;
        CriticalChance = criticalChance;
        CriticalDamage = criticalDamage;
        MaxHealth = maxHealth;
    }

    public string GetRarityName()
    {
        switch (Rarity)
        {
            case WeaponRarity.Normal: return "노말";
            case WeaponRarity.Magic: return "매직";
            case WeaponRarity.Rare: return "레어";
            case WeaponRarity.Unique: return "유니크";
            case WeaponRarity.Epic: return "에픽";
            case WeaponRarity.Legend: return "레전드";
            case WeaponRarity.Star: return "스타";
            case WeaponRarity.Galaxy: return "갤럭시";
            default: return Rarity.ToString();
        }
    }

    public string GetGradeName()
    {
        switch (Grade)
        {
            case WeaponGrade.Lower: return "하급";
            case WeaponGrade.Intermediate: return "중급";
            case WeaponGrade.Upper: return "상급";
            case WeaponGrade.Highest: return "최상급";
            default: return Grade.ToString();
        }
    }
}
