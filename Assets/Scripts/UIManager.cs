using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // HUD 화면 버튼
    [Header("HUDButton")]
    [SerializeField] private Button MailBtn;
    [SerializeField] private Button ContentsMenuBtn;
    [SerializeField] private Button AmuletBtn;
    [SerializeField] private Button ChattingBtn;

    // 퀵 메뉴 버튼
    [Header("QuickMenuButton")]
    [SerializeField] private Button AbilityBtn;
    [SerializeField] private Button EquipmentBtn;
    [SerializeField] private Button SkillBtn;
    [SerializeField] private Button DungeonBtn;
    [SerializeField] private Button ShopBtn;
    [SerializeField] private Button HUDSummonBtn;

    // 버튼을 누르면 나오게 될 패널
    [Header("ContentsPanel")]
    [SerializeField] private GameObject ContentsPanel;
    [SerializeField] private GameObject ChattingPanel;

    // 퀵 메뉴에서 누르면 나오게 될 패널
    [Header("QuickMenuPanel")]
    [SerializeField] private GameObject AbilityPanel;
    [SerializeField] private GameObject EquipmentPanel;
    [SerializeField] private GameObject SkillPanel;
    [SerializeField] private GameObject DungeonPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject SummonPanel;

    // ContentsBtn을 누르면 나오게 되는 버튼
    [Header("ContentsMenuButton")]
    [SerializeField] private Button AttendanceBtn;
    [SerializeField] private Button MissionBtn;
    [SerializeField] private Button RankBtn;
    [SerializeField] private Button BlessingBtn;
    [SerializeField] private Button AttributeBtn;
    [SerializeField] private Button AwakeBtn;
    [SerializeField] private Button PassBtn;
    [SerializeField] private Button InventoryBtn;
    [SerializeField] private Button PowerSavingBtn;
    [SerializeField] private Button FriendBtn;
    [SerializeField] private Button GuildBtn;
    [SerializeField] private Button SettingsBtn;

    private void OnEnable()
    {
        //HUDBtn
        MailBtn.onClick.AddListener(OnMailBtnClick);
        ContentsMenuBtn.onClick.AddListener(OnContentsMenuBtnClick);
        AmuletBtn.onClick.AddListener(OnAmuletBtnClick);
        ChattingBtn.onClick.AddListener(OnChattingBtnBtnClick);

        //QuickMenuBtn
        AbilityBtn.onClick.AddListener(OnAbilityBtnClick);
        EquipmentBtn.onClick.AddListener(OnEquipmentBtnClick);
        SkillBtn.onClick.AddListener(OnSkillBtnClick);
        DungeonBtn.onClick.AddListener(OnDungeonBtnClick);
        ShopBtn.onClick.AddListener(OnShopBtnClick);
        HUDSummonBtn.onClick.AddListener(OnHUDSummonBtnClick);

        //ContentsMenuBtn
        AttendanceBtn.onClick.AddListener(OnAttendanceBtnClick);
        MissionBtn.onClick.AddListener(OnMissionBtnClick);
        RankBtn.onClick.AddListener(OnRankBtnClick);
        BlessingBtn.onClick.AddListener(OnBlessingBtnClick);
        AttributeBtn.onClick.AddListener(OnAttributeBtnClick);
        AwakeBtn.onClick.AddListener(OnAwakeBtnClick);
        PassBtn.onClick.AddListener(OnPassBtnClick);
        InventoryBtn.onClick.AddListener(OnInventoryBtnClick);
        PowerSavingBtn.onClick.AddListener(OnPowerSavingBtnClick);
        FriendBtn.onClick.AddListener(OnFriendBtnClick);
        GuildBtn.onClick.AddListener(OnGuildBtnClick);
        SettingsBtn.onClick.AddListener(OnSettingsBtnClick);
    }

    private void OnDisable()
    {
        //HUDBtn
        MailBtn.onClick.RemoveListener(OnMailBtnClick);
        ContentsMenuBtn.onClick.RemoveListener(OnContentsMenuBtnClick);
        AmuletBtn.onClick.RemoveListener(OnAmuletBtnClick);
        ChattingBtn.onClick.RemoveListener(OnChattingBtnBtnClick);

        //QuickMenuBtn
        AbilityBtn.onClick.RemoveListener(OnAbilityBtnClick);
        EquipmentBtn.onClick.RemoveListener(OnEquipmentBtnClick);
        SkillBtn.onClick.RemoveListener(OnSkillBtnClick);
        DungeonBtn.onClick.RemoveListener(OnDungeonBtnClick);
        ShopBtn.onClick.RemoveListener(OnShopBtnClick);
        HUDSummonBtn.onClick.RemoveListener(OnHUDSummonBtnClick);

        //ContentsMenuBtn
        AttendanceBtn.onClick.RemoveListener(OnAttendanceBtnClick);
        MissionBtn.onClick.RemoveListener(OnMissionBtnClick);
        RankBtn.onClick.RemoveListener(OnRankBtnClick);
        BlessingBtn.onClick.RemoveListener(OnBlessingBtnClick);
        AttributeBtn.onClick.RemoveListener(OnAttributeBtnClick);
        AwakeBtn.onClick.RemoveListener(OnAwakeBtnClick);
        PassBtn.onClick.RemoveListener(OnPassBtnClick);
        InventoryBtn.onClick.RemoveListener(OnInventoryBtnClick);
        PowerSavingBtn.onClick.RemoveListener(OnPowerSavingBtnClick);
        FriendBtn.onClick.RemoveListener(OnFriendBtnClick);
        GuildBtn.onClick.RemoveListener(OnGuildBtnClick);
        SettingsBtn.onClick.RemoveListener(OnSettingsBtnClick);
    }

    #region HUD
    private void OnMailBtnClick()
    {
        // mail 
    }
    private void OnContentsMenuBtnClick() { }
    private void OnAmuletBtnClick() { }
    private void OnChattingBtnBtnClick() { }
    #endregion

    #region QuickMenu
    private void OnAbilityBtnClick() { }
    private void OnEquipmentBtnClick() { }
    private void OnSkillBtnClick() { }
    private void OnDungeonBtnClick() { }
    private void OnShopBtnClick() { }
    private void OnHUDSummonBtnClick() { }

    #endregion

    #region Contents
    private void OnAttendanceBtnClick() { }
    private void OnMissionBtnClick() { }
    private void OnRankBtnClick() { }
    private void OnBlessingBtnClick() { }
    private void OnAttributeBtnClick() { }
    private void OnAwakeBtnClick() { }
    private void OnPassBtnClick() { }
    private void OnInventoryBtnClick() { }
    private void OnPowerSavingBtnClick() { }
    private void OnFriendBtnClick() { }
    private void OnGuildBtnClick() { }
    private void OnSettingsBtnClick() { }
    #endregion
}
