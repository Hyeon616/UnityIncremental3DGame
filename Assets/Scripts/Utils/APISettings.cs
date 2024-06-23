using UnityEngine;

[CreateAssetMenu(fileName = "APISettings", menuName = "Settings/APISettings")]
public class APISettings : ScriptableObject
{
    public string baseUrl;
    public string registerEndpoint = "register";
    public string loginEndpoint = "login";
    public string weaponsEndpoint = "weapons";
    public string drawWeaponEndpoint = "drawWeapon";
    public string synthesizeWeaponEndpoint = "synthesizeWeapon";
    public string synthesizeAllWeaponsEndpoint = "synthesizeAllWeapons";
    public string getWeaponsByRarityEndpoint = "weaponsByRarity";
    public string playerDataEndpoint = "player";
    public string mailsEndpoint = "mails";
    public string claimRewardEndpoint = "claimReward";
    public string guildsEndpoint = "guilds";
    public string friendsEndpoint = "friends";
    public string playerWeaponsEndpoint = "playerWeapons";
    public string playerSkillsEndpoint = "playerSkills";
    public string playerBlessingsEndpoint = "playerBlessings";
    public string missionProgressEndpoint = "missionProgress";
    public string checkUsernameEndpoint = "check-username";
    public string checkNicknameEndpoint = "check-nickname";
    public string rewardsEndpoint = "rewards";

    public string RegisterUrl => $"{baseUrl}/{registerEndpoint}";
    public string LoginUrl => $"{baseUrl}/{loginEndpoint}";
    public string WeaponsUrl => $"{baseUrl}/{weaponsEndpoint}";
    public string DrawWeaponUrl => $"{baseUrl}/{drawWeaponEndpoint}";
    public string SynthesizeWeaponUrl => $"{baseUrl}/{synthesizeWeaponEndpoint}";
    public string SynthesizeAllWeaponsUrl => $"{baseUrl}/{synthesizeAllWeaponsEndpoint}";
    public string GetWeaponsByRarityUrl => $"{baseUrl}/{getWeaponsByRarityEndpoint}";
    public string PlayerDataUrl(int playerId) => $"{baseUrl}/{playerDataEndpoint}/{playerId}";
    public string MailsUrl(int userId) => $"{baseUrl}/{mailsEndpoint}/{userId}";
    public string ClaimRewardUrl => $"{baseUrl}/{claimRewardEndpoint}";
    public string GuildsUrl => $"{baseUrl}/{guildsEndpoint}";
    public string FriendsUrl(int userId) => $"{baseUrl}/{friendsEndpoint}/{userId}";
    public string PlayerWeaponsUrl(int userId) => $"{baseUrl}/{playerWeaponsEndpoint}/{userId}";
    public string PlayerSkillsUrl(int userId) => $"{baseUrl}/{playerSkillsEndpoint}/{userId}";
    public string PlayerBlessingsUrl(int userId) => $"{baseUrl}/{playerBlessingsEndpoint}/{userId}";
    public string MissionProgressUrl(int userId) => $"{baseUrl}/{missionProgressEndpoint}/{userId}";
    public string CheckUsernameUrl(string username) => $"{baseUrl}/{checkUsernameEndpoint}?username={username}";
    public string CheckNicknameUrl(string nickname) => $"{baseUrl}/{checkNicknameEndpoint}?nickname={nickname}";
    public string RewardsUrl => $"{baseUrl}/{rewardsEndpoint}";
}
