[System.Serializable]
public class Weapon
{
    public string rarity;
    public string grade;
    public int level;
    public int attack_power;
    public float critical_chance;
    public float critical_damage;
    public int max_health;
    public int count;

    public string GetRarityName()
    {
        return rarity;
    }

    public string GetGradeName()
    {
        return grade;
    }
}
