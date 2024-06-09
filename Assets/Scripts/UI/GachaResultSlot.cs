using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultSlot : MonoBehaviour
{
    public Text WeaponRarityText;
    public Text WeaponGradeText;
    public Image ItemImage;
    public Image SlotBackgroundImage;

    public void SetSlot(Weapon weapon)
    {
        if (weapon != null)
        {
            WeaponRarityText.text = weapon.GetRarityName();
            WeaponRarityText.color = GetRarityColor(weapon.rarity);
            WeaponGradeText.text = weapon.GetGradeName();
            ItemImage.color = new Color(1f, 1f, 1f, 1f); // 활성화된 슬롯의 아이템 이미지를 은은하게 표시
            ItemImage.sprite = GetWeaponSprite(weapon);
            SlotBackgroundImage.color = GetRarityColor(weapon.rarity); // 활성화된 슬롯의 배경색
        }

    }

    private Color GetRarityColor(string rarity)
    {
        return rarity switch
        {
            "일반" => new Color(0.75f, 0.75f, 0.75f, 1f), // 밝은 회색
            "고급" => new Color(0f, 1f, 0f, 1f), // 초록
            "매직" => new Color(0f, 0f, 1f, 1f), // 파랑
            "유물" => new Color(1f, 0.65f, 0f, 1f), // 주황
            "영웅" => new Color(1f, 0f, 1f, 1f), // 마젠타
            "에픽" => new Color(1f, 1f, 0f, 1f), // 노랑
            "고대" => new Color(0.55f, 0f, 0f, 1f), // 빨강
            "신화" => new Color(0.53f, 0.81f, 0.92f, 1f), // 밝은 파랑
            _ => new Color(0f, 0f, 0f, 1f), // 기본 검정색
        };
    }

    private Sprite GetWeaponSprite(Weapon weapon)
    {
        string rarity = weapon.rarity;
        string grade = weapon.grade;

        // 영어로 된 경로 및 파일명 사용
        string spritePath = $"Sprites/Weapons/{ConvertToEnglish(rarity)}_{ConvertToEnglish(grade)}";
        Sprite weaponSprite = Resources.Load<Sprite>(spritePath);

        if (weaponSprite == null)
        {
            Debug.LogError($"Sprite not found at path: {spritePath}");
        }

        return weaponSprite;
    }

    private string ConvertToEnglish(string korean)
    {
        return korean switch
        {
            "일반" => "Normal",
            "고급" => "Advanced",
            "매직" => "Magic",
            "유물" => "Rare",
            "영웅" => "Heroic",
            "에픽" => "Epic",
            "고대" => "Ancient",
            "신화" => "Mythical",
            "하급" => "Lower",
            "중급" => "Intermediate",
            "상급" => "Upper",
            "최상급" => "Highest",
            _ => "Unknown",
        };
    }
}
