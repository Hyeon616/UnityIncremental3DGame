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

    private bool _hasBeenAcquired;
    public int WeaponId { get; private set; }
    public void Initialize(string rarity, string grade)
    {
        WeaponRarityText.text = rarity;
        WeaponGradeText.text = grade;
        WeaponCountText.text = "0/5";
        WeaponLevelText.text = "Lv. 0";
        ItemImage.color = new Color(1f, 1f, 1f, 0.2f);
        SlotBackgroundImage.color = GetRarityColor(rarity);
        _hasBeenAcquired = false;
    }

    public void SetSlot(Weapon weapon, bool isActive)
    {
        WeaponId = weapon.id;

        if (isActive && weapon != null)
        {
            WeaponRarityText.text = weapon.GetRarityName();
            WeaponGradeText.text = weapon.GetGradeName();
            WeaponCountText.text = $"{weapon.count}/5";
            WeaponLevelText.text = $"Lv. {weapon.level}";
            ItemImage.color = new Color(1f, 1f, 1f, 1f); 
            SlotBackgroundImage.color = GetRarityColor(weapon.rarity); 
            _hasBeenAcquired = true;
            Debug.Log($"Slot set with Weapon Rarity: {weapon.rarity}, Grade: {weapon.grade}, Count: {weapon.count}");
        }
        else if (_hasBeenAcquired)
        {
            WeaponCountText.text = $"{weapon.count}/5";
            WeaponLevelText.text = $"Lv. {weapon.level}";
            ItemImage.color = new Color(1f, 1f, 1f, 1f);
            SlotBackgroundImage.color = GetRarityColor(weapon.rarity);
            Debug.Log("Slot previously acquired set to default state.");
        }
        else
        {
            ItemImage.color = new Color(1f, 1f, 1f, 0.2f); 
            Debug.Log("Slot not acquired and set to initial state.");
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
            "에픽" => new Color(1f, 1f, 0f, 1f), // 상아
            "고대" => new Color(0.55f, 0f, 0f, 1f), // 빨강
            "신화" => new Color(0.53f, 0.81f, 0.92f, 1f), // 밝은 파랑
            _ => new Color(0f, 0f, 0f, 1f), // 기본 검정색
        };
    }

    public void IncreaseCount()
    {
        int currentCount = int.Parse(WeaponCountText.text.Split('/')[0]);
        currentCount++;
        WeaponCountText.text = $"{currentCount}/5";
    }

}
