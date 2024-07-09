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

}

public static class UIPrefabExtensions
{
    public static string GetPrefabName(this UIPrefab uiPrefab)
    {
        return uiPrefab.ToString();
    }
}