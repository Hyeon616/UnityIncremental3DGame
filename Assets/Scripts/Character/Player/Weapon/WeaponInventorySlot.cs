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
            WeaponRarityText.text = GetRarityName(Rarity);
            WeaponGradeText.text = GetGradeName(Grade);
            WeaponCountText.text = "0/5";
            WeaponLevelText.text = "Lv. 0";
            ItemImage.color = new Color(1f, 1f, 1f, 0.8f); // 획득한 적 있는 슬롯의 아이템 이미지를 은은하게 표시
            SlotBackgroundImage.color = GetRarityColor(Rarity, 0.8f); // 획득한 적 있는 슬롯의 배경색
        }
        else
        {
            WeaponCountText.text = "0/5";
            WeaponLevelText.text = "Lv. 0";
            ItemImage.color = new Color(1f, 1f, 1f, 0.2f); // 처음 획득되지 않은 슬롯의 아이템 이미지를 더 어둡게 표시
            SlotBackgroundImage.color = GetRarityColor(Rarity, 0.2f); // 처음 획득되지 않은 슬롯의 배경색
        }
    }

    private Color GetRarityColor(string rarity, float alpha)
    {
        return rarity switch
        {
            "일반" => new Color(0.75f, 0.75f, 0.75f, alpha), // 밝은 회색
            "고급" => new Color(0f, 0f, 1f, alpha), // 파랑 
            "매직" => new Color(0f, 1f, 0f, alpha), // 초록색
            "유물" => new Color(1f, 0.65f, 0f, alpha), // 주황
            "영웅" => new Color(1f, 0f, 1f, alpha), // 마젠타
            "에픽" => new Color(1f, 1f, 0f, alpha), // 노랑
            "고대" => new Color(0.5f, 0f, 0.5f, alpha), // 진한 마젠타
            "신화" => new Color(0.53f, 0.81f, 0.92f, alpha), // 밝은 파랑
            _ => new Color(0f, 0f, 0f, alpha),
        };
    }

    private string GetRarityName(string rarity)
    {
        return rarity switch
        {
            "일반" => "일반",
            "고급" => "고급",
            "매직" => "매직",
            "유물" => "유물",
            "영웅" => "영웅",
            "에픽" => "에픽",
            "고대" => "고대",
            "신화" => "신화",
            _ => "알 수 없음",
        };
    }

    private string GetGradeName(string grade)
    {
        return grade switch
        {
            "하급" => "하급",
            "중급" => "중급",
            "상급" => "상급",
            "최상급" => "최상급",
            _ => "알 수 없음",
        };
    }
}
