using UnityEngine;

public enum WeaponRarity
{
    Normal,
    Magic,
    Rare,
    Unique,
    Epic,
    Legend,
    Ancient,
    Mythical
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
        return Rarity switch
        {
            WeaponRarity.Normal => "일반",
            WeaponRarity.Magic => "고급",
            WeaponRarity.Rare => "매직",
            WeaponRarity.Unique => "유물",
            WeaponRarity.Epic => "영웅",
            WeaponRarity.Legend => "에픽",
            WeaponRarity.Ancient => "고대", 
            WeaponRarity.Mythical => "신화",
            _ => Rarity.ToString()
        };
    }

    public string GetGradeName()
    {
        return Grade switch
        {
            WeaponGrade.Lower => "하급",
            WeaponGrade.Intermediate => "중급",
            WeaponGrade.Upper => "상급",
            WeaponGrade.Highest => "최상급",
            _ => Grade.ToString()
        };
    }
}
