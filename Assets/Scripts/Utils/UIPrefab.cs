public enum UIPrefab
{
    LoadingUI,
    AuthenticationUI,
    GrowthUI,
    EquipmentUI,
    SkillUI,
    DungeonUI,
    ShopUI,
    SummonUI,
    DamageTextUI,
}

public static class UIPrefabExtensions
{
    public static string GetPrefabName(this UIPrefab uiPrefab)
    {
        return uiPrefab.ToString();
    }
}