using System;
using System.Collections.Generic;

[Serializable]
public class PlayerModel
{
    public int player_id;
    public string player_username;
    public string player_nickname;
    public PlayerAttributes attributes;
}
[Serializable]
public class PlayerAttributes
{
    public int star_dust;
    public int element_stone;
    public int skill_summon_tickets;
    public int money;
    public int attack_power;
    public int max_health;
    public float critical_chance;
    public float critical_damage;
    public int level;
    public int awakening;
    public float fire_damage;
    public float water_damage;
    public float electric_damage;
    public float wind_damage;
    public float light_damage;
    public float dark_damage;
    public int fire_enhance;
    public int water_enhance;
    public int electric_enhance;
    public int wind_enhance;
    public int light_enhance;
    public int dark_enhance;
    public int combat_power;
    public string current_stage;
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
public class PlayerSkillModel
{
    public int player_skill_id;
    public int player_id;
    public int skill_id;
    public int level;
}

[Serializable]
public class PlayerBlessingModel
{
    public int player_blessing_id;
    public int player_id;
    public int blessing_id;
    public int level;
    public float attack_multiplier;
}

[Serializable]
public class MissionProgressModel
{
    public int player_id;
    public int last_level_check;
    public int last_combat_power_check;
    public int last_awakening_check;
    public string last_online_time_check;
}

[Serializable]
public class RewardModel
{
    public int id;
    public string type;
    public int requirement;
    public int reward;
}

[Serializable]
public class MonsterModel
{
    public int id;
    public string name;
    public int health;
    public int attack_power;
    public bool is_boss;
    public DropTable drop_table;
}

[Serializable]
public class DropTable
{
    public int money;
    public int star_dust;
    public int element_stone;
    public float star_dust_drop_chance;
    public float element_stone_drop_chance;
}

[Serializable]
public class StageModel
{
    public string stage_number;
    public List<MonsterModel> monsters;
}