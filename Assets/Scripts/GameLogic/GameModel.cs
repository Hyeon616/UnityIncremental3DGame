using System;
using UnityEngine;

[Serializable]
public class PlayerModel
{
    public int player_id;
    public string player_username;
    public string player_nickname;
    public Attributes attributes;

    public PlayerModel()
    {
        attributes = new Attributes();
    }

    [Serializable]
    public class Attributes
    {
        public int element_stone;
        public int skill_summon_tickets;
        public int money;
        public int attack_power;
        public int max_health;
        public float critical_chance;
        public float critical_damage;
        public string current_stage;
        public int level;
        public int awakening;
        public int? guild_id;
        public int combat_power;
        public int? rank;
        public int? equipped_skill1_id;
        public int? equipped_skill2_id;
        public int? equipped_skill3_id;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

[Serializable]
public class MailModel
{
    public int id;
    public int user_id;
    public string type;
    public string reward;
    public string created_at;
    public string expires_at;
    public bool is_read;
}

[Serializable]
public class GuildModel
{
    public int guild_id;
    public string guild_name;
    public int guild_leader;
}

[Serializable]
public class FriendModel
{
    public int player_id;
    public int friend_id;
}

[Serializable]
public class PlayerWeaponModel
{
    public int player_weapon_id;
    public int player_id;
    public int weapon_id;
    public int level;
    public int count;
    public int attack_power;
    public float critical_chance;
    public float critical_damage;
}

[Serializable]
public class SkillModel
{
    public int id;
    public string name;
    public string description;
    public int damage_percentage;
    public string image;
    public int cooldown;
}

[Serializable]
public class PlayerSkillModel
{
    public int player_skill_id;
    public int player_id;
    public int skill_id;
    public int level;
    public SkillModel skill;
}

[Serializable]
public class MissionProgressModel
{
    public int player_id;
    public int level_progress;
    public int combat_power_progress;
    public int awakening_progress;
    public int online_time_progress;
    public int weapon_level_sum_progress;
    public DateTime last_online_time_check;
    public int total_online_time;
}

[Serializable]
public class RewardModel
{
    public int id;
    public string name;
    public string description;
    public int reward;
}

[Serializable]
public class MonsterModel
{
    public int id;
    public string Stage;
    public string Type;
    public string Name;
    public int Health;
    public int Attack;
    public int DropMoney;
    public int DropElementStone;
    public float DropElementStoneChance;
}


[Serializable]
public class WeaponModel
{
    public int weapon_id;
    public int weapon_grade;
    public int attack_power;
    public float crit_rate;
    public float crit_damage;
    public long weapon_exp;
    public string prefab_name;
}

[Serializable]
public class SkillSlotModel
{
    public PlayerSkillModel playerSkill; 
    public bool is_empty;

    public SkillSlotModel(PlayerSkillModel playerSkill)
    {
        this.playerSkill = playerSkill;
        is_empty = playerSkill == null;
    }

    // 편의를 위한 프로퍼티들
    public int SkillId => playerSkill?.skill_id ?? -1;
    public string SkillName => playerSkill?.skill.name ?? "Empty";
    public string IconFileName => playerSkill?.skill.image ?? "empty_slot";
}

[Serializable]
public class AttendanceRewardResponse
{
    public string message;
    public int dayCount;
}