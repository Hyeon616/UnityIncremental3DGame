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

    public string Rarity { get; private set; }
    public string Grade { get; private set; }
    private bool _hasBeenAcquired;

    public void Initialize(string rarity, string grade)
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
            WeaponCountText.text = $"{weapon.count}/5";
            WeaponLevelText.text = $"Lv. {weapon.level}";
            ItemImage.color = new Color(1f, 1f, 1f, 0.8f); // 활성화된 슬롯의 아이템 이미지를 은은하게 표시
            SlotBackgroundImage.color = GetRarityColor(weapon.rarity, 0.8f); // 활성화된 슬롯의 배경색
            _hasBeenAcquired = true;
        }
        else if (_hasBeenAcquired)
        {
            WeaponRarityText.text = GetRarityName(Rarity.ToString());
            WeaponGradeText.text = GetGradeName(Grade.ToString());
            WeaponCountText.text = "0/5";
            WeaponLevelText.text = "Lv. 0";
            ItemImage.color = new Color(1f, 1f, 1f, 0.8f); // 획득한 적 있는 슬롯의 아이템 이미지를 은은하게 표시
            SlotBackgroundImage.color = GetRarityColor(Rarity.ToString(), 0.8f); // 획득한 적 있는 슬롯의 배경색
        }
        else
        {
            WeaponRarityText.text = GetRarityName(Rarity.ToString());
            WeaponGradeText.text = GetGradeName(Grade.ToString());
            WeaponCountText.text = "0/5";
            WeaponLevelText.text = "Lv. 0";
            ItemImage.color = new Color(1f, 1f, 1f, 0.2f); // 처음 획득되지 않은 슬롯의 아이템 이미지를 더 어둡게 표시
            SlotBackgroundImage.color = GetRarityColor(Rarity.ToString(), 0.2f); // 처음 획득되지 않은 슬롯의 배경색
        }
    }

    private Color GetRarityColor(string rarity, float alpha)
    {
        return rarity switch
        {
            "Normal" => new Color(0.75f, 0.75f, 0.75f, alpha), // 밝은 회색
            "Magic" => new Color(0f, 0f, 1f, alpha), // 파랑 
            "Rare" => new Color(0f, 1f, 0f, alpha), // 초록색
            "Unique" => new Color(1f, 0.65f, 0f, alpha), // 주황
            "Epic" => new Color(1f, 0f, 1f, alpha), // 마젠타
            "Legend" => new Color(1f, 1f, 0f, alpha), // 노랑
            "Ancient" => new Color(0.5f, 0f, 0.5f, alpha), // 진한 마젠타
            "Mythical" => new Color(0.53f, 0.81f, 0.92f, alpha), // 밝은 파랑
            _ => new Color(0f, 0f, 0f, alpha),
        };
    }

    private string GetRarityName(string rarity)
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
            _ => "알 수 없음",
        };
    }

    private string GetGradeName(string grade)
    {
        return grade switch
        {
            "Lower" => "하급",
            "Intermediate" => "중급",
            "Upper" => "상급",
            "Highest" => "최상급",
            _ => "알 수 없음",
        };
    }
}
