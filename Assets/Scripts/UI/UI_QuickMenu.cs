using UnityEngine;
using UnityEngine.UI;

public class UI_QuickMenu : MonoBehaviour
{
    [SerializeField] private Button GrowthBtn;
    [SerializeField] private Button EquipmentBtn;
    [SerializeField] private Button SkillBtn;
    [SerializeField] private Button DungeonBtn;
    [SerializeField] private Button ShopBtn;
    [SerializeField] private Button SummonBtn;

    private void OnEnable()
    {
        GrowthBtn.onClick.AddListener(OnGrowthBtnClicked);
        EquipmentBtn.onClick.AddListener(OnEquipmentBtnClicked);
        SkillBtn.onClick.AddListener(OnSkillBtnClicked);
        DungeonBtn.onClick.AddListener(OnDungeonBtnClicked);
        ShopBtn.onClick.AddListener(OnShopBtnClicked);
        SummonBtn.onClick.AddListener(OnSummonBtnClicked);
    }

    private void OnDisable()
    {
        GrowthBtn.onClick.RemoveListener(OnGrowthBtnClicked);
        EquipmentBtn.onClick.RemoveListener(OnEquipmentBtnClicked);
        SkillBtn.onClick.RemoveListener(OnSkillBtnClicked);
        DungeonBtn.onClick.RemoveListener(OnDungeonBtnClicked);
        ShopBtn.onClick.RemoveListener(OnShopBtnClicked);
        SummonBtn.onClick.RemoveListener(OnSummonBtnClicked);
    }

    private void OnGrowthBtnClicked()
    {

        UIManager.Instance.ShowUI(UIPrefab.GrowthUI);
    }

    private void OnEquipmentBtnClicked()
    {

        UIManager.Instance.ShowUI(UIPrefab.EquipmentUI);
    }

    private void OnSkillBtnClicked()
    {

        UIManager.Instance.ShowUI(UIPrefab.SkillUI);
    }

    private void OnDungeonBtnClicked()
    {

        UIManager.Instance.ShowUI(UIPrefab.DungeonUI);
    }

    private void OnShopBtnClicked()
    {

        UIManager.Instance.ShowUI(UIPrefab.ShopUI);
    }

    private void OnSummonBtnClicked()
    {

        UIManager.Instance.ShowUI(UIPrefab.SummonUI);
    }

}
