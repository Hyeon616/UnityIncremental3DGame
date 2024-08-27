using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class PlayerModel
{
    public int player_id { get; set; }
    public string player_username { get; set; }
    public string player_nickname { get; set; }
    public int base_element_stone { get; set; }
    public int base_skill_summon_tickets { get; set; }
    public int base_money { get; set; }
    public int base_attack_power { get; set; }
    public int base_max_health { get; set; }
    public float base_critical_chance { get; set; }
    public float base_critical_damage { get; set; }
    public string current_stage { get; set; }
    public int level { get; set; }
    public int awakening { get; set; }
    public int? guild_id { get; set; }
    public int element_stone { get; set; }
    public int skill_summon_tickets { get; set; }
    public int money { get; set; }
    public int attack_power { get; set; }
    public int max_health { get; set; }
    public float critical_chance { get; set; }
    public float critical_damage { get; set; }
    public int combat_power { get; set; }
    public int? rank { get; set; }
    public int? equipped_skill1_id { get; set; }
    public int? equipped_skill2_id { get; set; }
    public int? equipped_skill3_id { get; set; }
    public string Ability1 { get; set; }
    public string Ability2 { get; set; }
    public string Ability3 { get; set; }
    public string Ability4 { get; set; }
    public string Ability5 { get; set; }
    public string Ability6 { get; set; }
    public string Ability7 { get; set; }
    public string Ability8 { get; set; }
    public string Ability9 { get; set; }
    public string Ability10 { get; set; }
    public string Ability11 { get; set; }
    public string Ability12 { get; set; }

    [JsonIgnore]
    public Attributes attributes;

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        attributes = new Attributes
        {
            base_element_stone = this.base_element_stone,
            base_skill_summon_tickets = this.base_skill_summon_tickets,
            base_money = this.base_money,
            base_attack_power = this.base_attack_power,
            base_max_health = this.base_max_health,
            base_critical_chance = this.base_critical_chance,
            base_critical_damage = this.base_critical_damage,
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
            Ability3 = this.Ability3,
            Ability4 = this.Ability4,
            Ability5 = this.Ability5,
            Ability6 = this.Ability6,
            Ability7 = this.Ability7,
            Ability8 = this.Ability8,
            Ability9 = this.Ability9,
            Ability10 = this.Ability10,
            Ability11 = this.Ability11,
            Ability12 = this.Ability12
        };
    }

    public class Attributes
    {
        public int base_element_stone;
        public int base_skill_summon_tickets;
        public int base_money;
        public int base_attack_power;
        public int base_max_health;
        public float base_critical_chance;
        public float base_critical_damage;
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
        public string Ability4;
        public string Ability5;
        public string Ability6;
        public string Ability7;
        public string Ability8;
        public string Ability9;
        public string Ability10;
        public string Ability11;
        public string Ability12;

        public List<string> GetAbilitySet(int index)
        {
            switch (index)
            {
                case 0: return new List<string> { Ability1, Ability2, Ability3 };
                case 1: return new List<string> { Ability4, Ability5, Ability6 };
                case 2: return new List<string> { Ability7, Ability8, Ability9 };
                case 3: return new List<string> { Ability10, Ability11, Ability12 };
                default: return new List<string>();
            }
        }
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
    public string PrefabName;
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