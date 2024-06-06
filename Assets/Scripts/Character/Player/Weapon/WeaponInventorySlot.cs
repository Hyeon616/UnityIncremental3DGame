using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    public Text WeaponRarityText;
    public Text WeaponLevelText;
    public Text WeaponGradeText;
    public Text WeaponCountText;
    public Image ItemImage;
    public Image SlotBackgroundImage;  // 슬롯의 기본 이미지를 어둡게 처리하기 위해 추가

    public void SetSlot(Weapon weapon, bool isActive)
    {
        if (isActive)
        {
            WeaponRarityText.text = weapon.GetRarityName();
            WeaponGradeText.text = weapon.GetGradeName();
            WeaponCountText.text = $"{weapon.Count}/5";
            WeaponLevelText.text = $"Lv. {weapon.Level}";
            ItemImage.color = GetRarityColor(weapon.Rarity);
            SlotBackgroundImage.color = Color.white;  // 활성화된 슬롯은 밝게 표시
        }
        else
        {
            WeaponRarityText.text = weapon.GetRarityName();
            WeaponGradeText.text = weapon.GetGradeName();
            WeaponCountText.text = $"{weapon.Count}/5";
            WeaponLevelText.text = $"Lv. {weapon.Level}";
            ItemImage.color = Color.gray;  // 비활성화된 슬롯은 어둡게 표시
            SlotBackgroundImage.color = new Color(0.3f, 0.3f, 0.3f);  // 어둡게 처리
        }
    }

    private Color GetRarityColor(WeaponRarity rarity)
    {
        switch (rarity)
        {
            case WeaponRarity.Normal: return Color.gray;
            case WeaponRarity.Magic: return Color.blue;
            case WeaponRarity.Rare: return Color.green;
            case WeaponRarity.Unique: return Color.yellow;
            case WeaponRarity.Epic: return Color.magenta;
            case WeaponRarity.Legend: return Color.red;
            case WeaponRarity.Star: return Color.cyan;
            case WeaponRarity.Galaxy: return Color.white;
            default: return Color.black;
        }
    }
}
