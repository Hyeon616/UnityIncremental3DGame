using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "APISettings", menuName = "Settings/APISettings")]
public class APISettings : ScriptableObject
{
    public string baseUrl;

    public enum Endpoint
    {
        Register,
        Login,
        CheckUsername,
        CheckNickname,
        Weapons,
        DrawWeapon,
        SynthesizeWeapon,
        SynthesizeAllWeapons,
        GetWeaponsByRarity,
        PlayerData,
        Mails,
        ClaimReward,
        Guilds,
        Friends,
        PlayerWeapons,
        PlayerSkills,
        PlayerBlessings,
        MissionProgress,
        Rewards,
        Stages,
        Monsters,
        CurrentStage,
        UpdateStage,
        EquipSkill,
        SendMail,
        MarkMailAsRead,
        AttendanceReward,
        UpdateMissionProgress,
        UpdateOnlineTime,
        PlayerRank,
        ResetAbilities,
    }

    private static readonly Dictionary<Endpoint, string> endpointPaths = new Dictionary<Endpoint, string>
    {
        { Endpoint.Register, "auth/register" },
        { Endpoint.Login, "auth/login" },
        { Endpoint.CheckUsername, "checks/check-username" },
        { Endpoint.CheckNickname, "checks/check-nickname" },
        { Endpoint.Weapons, "weapons" },
        { Endpoint.DrawWeapon, "weapons/drawWeapon" },
        { Endpoint.SynthesizeWeapon, "weapons/synthesizeWeapon" },
        { Endpoint.SynthesizeAllWeapons, "weapons/synthesizeAllWeapons" },
        { Endpoint.GetWeaponsByRarity, "weapons/weaponsByRarity" },
        { Endpoint.PlayerData, "player/{0}" },
        { Endpoint.PlayerRank, "player/{0}/rank" },
        { Endpoint.Mails, "mails/{0}" },
        { Endpoint.ClaimReward, "rewards/claimReward" },
        { Endpoint.Guilds, "guilds" },
        { Endpoint.Friends, "friends" },
        { Endpoint.PlayerWeapons, "weapons/playerWeapons" },
        { Endpoint.PlayerSkills, "skills/playerSkills" },
        { Endpoint.PlayerBlessings, "playerBlessings" },
        { Endpoint.MissionProgress, "mission/progress" },
        { Endpoint.Rewards, "rewards" },
        { Endpoint.Stages, "stages" },
        { Endpoint.Monsters, "monsters" },
        { Endpoint.CurrentStage, "stages/currentStage" },
        { Endpoint.UpdateStage, "stages/updateStage" },
        { Endpoint.EquipSkill, "player/{0}/equip-skill" },
        {Endpoint.SendMail, "mails/send" },
        { Endpoint.MarkMailAsRead, "mails/{0}/read" },
        { Endpoint.AttendanceReward, "mails/attendance-reward" },
        { Endpoint.UpdateMissionProgress, "mission/progress" },
        { Endpoint.UpdateOnlineTime, "mission/progress" },
        { Endpoint.ResetAbilities, "player/{0}/reset-abilities" },
    };

    public string GetUrl(Endpoint endpoint)
    {
        if (endpointPaths.TryGetValue(endpoint, out string path))
        {
            return $"{baseUrl}/{path}";
        }
        Debug.LogError($"Endpoint '{endpoint}' not found.");
        return null;
    }

    public string GetUrl(Endpoint endpoint, int id)
    {
        if (endpointPaths.TryGetValue(endpoint, out string path))
        {
            return $"{baseUrl}/{string.Format(path, id)}";
        }
        Debug.LogError($"Endpoint '{endpoint}' not found.");
        return null;
    }

    public string GetUrl(Endpoint endpoint, string parameter)
    {
        if (endpointPaths.TryGetValue(endpoint, out string path))
        {
            return $"{baseUrl}/{path}?{parameter}";
        }
        Debug.LogError($"Endpoint '{endpoint}' not found.");
        return null;
    }
}


