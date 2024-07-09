using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

[Serializable]
public class PlayerModel
{
    public int player_id;
    public string player_username;
    public string player_nickname;
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
    public string Ability1;
    public string Ability2;
    public string Ability3;


    [JsonIgnore]
    public Attributes attributes;

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        attributes = new Attributes
        {
            element_stone = this.element_stone,
            skill_summon_tickets = this.skill_summon_tickets,
            money = this.money,
            attack_power = this.attack_power,
            max_health = this.max_health,
            critical_chance = this.critical_chance,
            critical_damage = this.critical_damage,
            current_stage = this.current_stage,
            level = this.level,
            awakening = this.awakening,
            guild_id = this.guild_id,
            combat_power = this.combat_power,
            rank = this.rank,
            equipped_skill1_id = this.equipped_skill1_id,
            equipped_skill2_id = this.equipped_skill2_id,
            equipped_skill3_id = this.equipped_skill3_id,
            Ability1 = this.Ability1,
            Ability2 = this.Ability2,
            Ability3 = this.Ability3

        };
    }

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
        public string Ability1;
        public string Ability2;
        public string Ability3;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
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
    [NonSerialized]
    public int CurrentHealth;

    public void Initialize()
    {
        CurrentHealth = Health;
    }

    public float GetHealthPercentage()
    {
        return (float)CurrentHealth / Health * 100f;
    }
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