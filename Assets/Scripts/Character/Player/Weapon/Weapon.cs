[System.Serializable]
public class Weapon
{
    public int id { get; set; }
    public string rarity { get; set; }
    public string grade { get; set; }
    public int level { get; set; }
    public int count { get; set; }
    public int attack_power { get; set; }
    public float critical_chance { get; set; }
    public float critical_damage { get; set; }
    public int max_health { get; set; }

    public string GetRarityName()
    {
        return rarity switch
        {
            "Normal" => "일반",
            "Magic" => "고급",
            "Rare" => "매직",
            "Unique" => "유물",
            "Epic" => "영웅",
            "Legend" => "에픽",
            "Ancient" => "고대",
            "Mythical" => "신화",
            _ => rarity
        };
    }

    public string GetGradeName()
    {
        return grade switch
        {
            "Lower" => "하급",
            "Intermediate" => "중급",
            "Upper" => "상급",
            "Highest" => "최상급",
            _ => grade
        };
    }
}
