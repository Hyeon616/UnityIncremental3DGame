using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    public Text WeaponRarityText;
    public Text WeaponLevelText;
    public Text WeaponGradeText;
    public Text WeaponCountText;
    public Image ItemImage;
    public Image SlotBackgroundImage;

    public WeaponRarity Rarity { get; private set; }
    public WeaponGrade Grade { get; private set; }

    public void Initialize(WeaponRarity rarity, WeaponGrade grade)
    {
        Rarity = rarity;
        Grade = grade;
        SetSlot(null, false);
    }

    public void SetSlot(Weapon weapon, bool isActive)
    {
        if (isActive && weapon != null)
        {
            WeaponRarityText.text = weapon.GetRarityName();
            WeaponGradeText.text = weapon.GetGradeName();
            WeaponCountText.text = $"{weapon.Count}/5";
            WeaponLevelText.text = $"Lv. {weapon.Level}";
            ItemImage.color = new Color(1f, 1f, 1f, 1f); // 활성화된 슬롯의 아이템 이미지를 은은하게 표시
            SlotBackgroundImage.color = GetRarityColor(weapon.Rarity, 1f); // 활성화된 슬롯의 배경색
        }
        else
        {
            WeaponRarityText.text = GetRarityName(Rarity);
            WeaponGradeText.text = GetGradeName(Grade);
            WeaponCountText.text = "0/5";
            WeaponLevelText.text = "Lv. 0";
            ItemImage.color = new Color(1f, 1f, 1f, 0.2f); // 비활성화된 슬롯의 아이템 이미지를 어둡게 표시
            SlotBackgroundImage.color = GetRarityColor(Rarity, 0.2f); // 비활성화된 슬롯의 배경색
        }
    }

    private Color GetRarityColor(WeaponRarity rarity, float alpha)
    {
        switch (rarity)
        {
            case WeaponRarity.Normal: return new Color(0.75f, 0.75f, 0.75f, alpha); // 밝은 회색
            case WeaponRarity.Magic: return new Color(0f, 0f, 1f, alpha);  // 파랑 
            case WeaponRarity.Rare: return new Color(0f, 1f, 0f, alpha); // 초록색
            case WeaponRarity.Unique: return new Color(1f, 0.65f, 0f, alpha); // 주황
            case WeaponRarity.Epic: return new Color(1f, 0f, 1f, alpha); // 마젠타
            case WeaponRarity.Legend: return new Color(1f, 1f, 0f, alpha); // 노랑
            case WeaponRarity.Ancient: return new Color(0.5f, 0f, 0.5f, alpha); // 진한 마젠타
            case WeaponRarity.Mythical: return new Color(0.53f, 0.81f, 0.92f, alpha); // 밝은 파랑
            default: return new Color(0f, 0f, 0f, alpha);
        }
    }

    private string GetRarityName(WeaponRarity rarity)
    {
        switch (rarity)
        {
            case WeaponRarity.Normal: return "일반";
            case WeaponRarity.Magic: return "고급";
            case WeaponRarity.Rare: return "매직";
            case WeaponRarity.Unique: return "유물";
            case WeaponRarity.Epic: return "영웅";
            case WeaponRarity.Legend: return "에픽";
            case WeaponRarity.Ancient: return "고대";
            case WeaponRarity.Mythical: return "신화";
            default: return "알 수 없음";
        }
    }

    private string GetGradeName(WeaponGrade grade)
    {
        switch (grade)
        {
            case WeaponGrade.Lower: return "하급";
            case WeaponGrade.Intermediate: return "중급";
            case WeaponGrade.Upper: return "상급";
            case WeaponGrade.Highest: return "최상급";
            default: return "알 수 없음";
        }
    }
}
