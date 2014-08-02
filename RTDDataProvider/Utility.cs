using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace RTDDataProvider
{
    public enum UnitAttribute : byte
    {
        NONE,
        LIGHT,
        DARK,
        FIRE,
        WATER,
        ALL
    }
    public enum Class : byte
    {
        KNIGHT,
        LANCER,
        ARCHER,
        WIZARD,
        ALL
    }
    public enum PassiveSkillType
    {
        MAX_LIFE_GRADEUP = 1,
        ATTACK_GRADEUP = 2,
        RECOVER_GRADEUP = 3,
        INITIAL_SOUL_GRADEUP = 4,
        DAMAGE_GRADEDOWN = 5,
        SOUL_GRADEUP = 6,
        ACTIVESKILL_GRADEUP = 7,
        TURNSPEED_GRADEUP = 8,
        DRAGON_KILLER = 9,
        DEATH_KILLER = 10,
        LUCK_GRADEUP = 11,
        RECOVER_PANELL_GRADEUP = 12,
        SUPERCRITICAL_GRADEUP = 13,
        SUPER_DEFFENCE = 14,
        SUPER_ATTACK = 15,
        CHAIN_GRADEUP = 16,
        ATTRIBUTE_PANEL_GRADEUP = 17,
        ATTRIBUTE_KILLER = 18,
        MOVE_LIFE_RECOVER = 19,
        GET_TREASURE_SOUL = 20,
        LIFE_ATTRIBUTE_UP = 21,
        LIFE_ATTRIBITE_DRAIN = 22,
        COUNTER = 23,
        DOUBLE_ATTACK = 24,
        SOUL_STEP = 25,
        DRACOTTER = 26,
        GRAVITY_ATTACK = 27,
        SKILL_ZERO = 28,
        SOUL_HUNT = 29,
        HEALING_AND_ASSULT = 30,
        ASSULT_AND_LIFE = 31,
        LIFE_AND_HEALING = 32,
        DOUBLE_KILLER = 33,
        MASTER_OF_PANEL = 34,
        QUICK_BLAST = 35,
        ARCANE_CROW = 36,
        SERIAL_KILLER = 37,
        LIFE_GUARD = 38,
        GUARDIAN_HEAL = 39,
        LIFE_DOUBLE_COUNTER = 40,
        SOUL_DOUBLE_COUNTER = 41,
        ATTRIBUTE_SOUL_DRAIN = 42,
        MIND_RECOVER_UP = 43,
        MIND_ASSULT_UP = 44,
        MIND_LIFE_UP = 45,
        MIND_SOUL_UP = 46,
        SkillAbsorb = 47,
        GodReflect = 48,
        Dragon_Slayer = 49,
        CHASE_SOUL_GET = 50,
        LIFE_SOULFREE = 51,
        MINIMUM_POWER = 52,
        MIN_SOUL_POWER = 53,
        COLOR_ASSULT = 54,
        LEADER_COPY = 55,
        PANEL_HEALER = 56,
        LINK_PANEL_DELAY = 57,
        WEAK_ATTACK_DELAY = 58,
        DELAY_CHASE_ATTACK = 59,
        DELAY_HEAL_LIFE = 60,
        ATTRIBUTEUP_SOUL_GET = 61,
        TRIPLE_SUPPORT = 62,
        CHASE_SUPPORT = 63,
        LINK_PANEL_HEAL = 64,
        MOB_KILLER = 65,
        SERIAL_LIFE_UP = 66,
        DIFFER_CLASS_ATTACK = 67,
        ELEMENT_POWER = 68,
        AKIBA_RECOVER = 69,
        AKIBA_ASSULT = 70,
        AKIBA_LIFEUP = 71,
        AKIBA_SOULUP = 72,
        FUSION_BLAST = 73,
        CLASS_SUPPORT = 74,
        GET_TREASURE_ENHANCE = 75,
        DROP_MONEYUP = 76,
        TURN_ENHANCE = 77,
        DAMAGE_ENHANCE = 78,
        DAMAGE_DRAIN = 79,
        SERIAL_HEAL_UP = 80,
        ATTACK_ENHANCE = 81,
        MOVE_ENHANCE = 82,
        ATTRIBUTE_DRAIN_AND_KILLER = 83,
        UNIT_ENEMY_KILLER = 84,
        SP_PANEL_UP = 85,
        PANEL_CHAIN_ENHANCE = 86,
        SOUL_STEP_AND_SOUL_GET = 87,
        REFLECTION = 88,
        PANEL_MIX_DAMAGE = 89,
        ATTACK_ENHANCE_BY_UNITCLASS = 90,
        LIFE_UP_BY_UNITCLASS = 91,
        ATTACK_ENHANCE_BY_MAXLIFE = 92,
        WEAK_ATTACK_DAMAGE_UP = 93,
        DAMAGE_UP_BY_LIFE_RATIO = 94,
        DAMAGE_USE_PANELS = 95,
        RETANTION_PANEL_DAMAGE_SCALE = 96,
        PLAYER_AND_ENEMY_DAMAGE_UP = 97,
        BUFFER = 98,
    }
    public enum ActiveSkillType
    {
        HealLife1 = 1,
        HealLife2 = 2,
        DirectAttack1 = 3,
        DirectAttack2 = 4,
        PanelChange = 5,
        AttributePanelUp = 6,
        DefenceUp = 7,
        DefenceDown = 8,
        Gravity = 9,
        AnlimitedPanel = 10,
        Astral = 11,
        Acute = 12,
        Enhance = 13,
        Arcane = 14,
        Delirium = 15,
        AttributeAttack = 16,
        Destruct = 17,
        Escape = 18,
        SuperDefenceUp = 19,
        AttributeStock = 20,
        DeliriumHeven = 21,
        MultiAttack = 22,
        SoulFree = 23,
        EnemyScan = 24,
        TimeStop = 25,
        GravityAttack = 26,
        PanelCopy = 27,
        PerfectDefence = 28,
        Curse = 29,
        StrongStyle = 30,
        MindOfZero = 31,
        HealCanon = 32,
        DragonInjection = 33,
        Resurrection = 34,
        GodBurst = 35,
        ColorEnhance = 36,
        InfiniteAttack = 37,
        AllPanelAttack = 38,
        DelayDamageAttack = 39,
        ClassPanelAttack = 40,
        PanelCall = 41,
        DragonBind = 42,
        Change_AttackHeal = 43,
        HealEnhance = 44,
        HealAndAttackUp = 45,
        HealAndDefenceUp = 46,
        AstralHeal = 47,
        FinishBurst = 48,
        TreasureHunt = 49,
        AttributeChange = 50,
        DelayDamageGravity = 51,
        FullHealAndGetSoul = 52,
        MultiBurst = 53,
        ExpansionDestruct = 54,
        JumpAttack = 55,
        MoneyAttack = 56,
        LimitEnhance = 57,
        DamageAttack = 58,
        CounterAttackUp = 59,
        ExPanelAttack = 60,
        AttrAtkAndAtkUp = 61,
        DefenceUpAndEnhance = 62,
        DragonEnhance = 63,
        SoulOfZero = 64,
        AttackCanon = 65,
        EnemyScanDefDown = 66,
        ElementBind = 67,
        AttributeNextAtkUp = 68,
    }
    public enum PanelSkillType
    {
        LIFERECOVER_CALC = 1,
        LIFERECOVER_FIX = 2,
        SOULGET = 3,
        CHASE_ATTACK = 4,
        ENEMY_DEFENCE_DOWN = 5,
        ENEMY_DAMAGE = 6,
        LIFE_AND_SOUL = 7,
        ATTACK_UP = 8,
        GRAVITY = 9,
        SHUFFLE = 10,
        ATTRIBUTE_ADD = 11,
        SOULGET_2 = 12,
        CURSE = 13,
        ARCANE = 14,
        LIMIT_ENHANCE = 15,
        LIMIT_SUPERATTACK = 16,
        MOVESOUL_OR_BATTLELIFE = 17,
        BATTLE_EVASION = 18,
        LIMIT_LIFERECOVER_FIX = 19,
        LIFE_UP_LIMIT = 20,
        RANDOM_EFFECT = 21,
        ENEMY_DAMAGE_AND_DEFENCE_DOWN = 22,
        HEAL_CANON = 23,
        DEFENCE_PREPARATIONS = 24,
        LIMIT_ATTACKUP_COLOR = 25,
        IGNORE_DEFENCE = 26,
        UNLIMITED_WEAPON = 27,
        DAMAGE_AND_UNLIMITED = 28,
        ONE_SOULGET_TWO_LIFEFULLHEAL = 29,
    }
    public enum SkillPhase
    {
        MOVE = 1,
        BATTLE,
        ALL
    }
    public enum AssignID
    {
        SWORD,
        GREATSWORD,
        LANCE,
        PILEBANKER,
        BOW,
        GUN,
        STICK,
        ARTIFACT,
        STICK_FEMALE,
        TWO_SWORD,
        TWIN_LANCE,
        CANNON,
        CHAOS_SWD,
        CHAOS_GUN,
        CHAOS_BAN,
        CHAOS_ART,
        BOW_MALE,
        KUNGFU,
        POK_LANCE,
        POK_STICK,
        LACNE_FEMALE,
        RAPIA,
        ORG_SWORD,
        ORG_GUN,
        GRT_FEMALE,
        ART_FEMALE,
        POK_SWORD,
        POK_CAN,
        ORG_LAN,
        WIN_SWORD,
        WIN_LANCE,
        WIN_BOW,
        WIN_STICK,
        GOD_BOW,
        ORG_STICK,
        POK_ART,
        RVS_GUN,
        RVS_ART,
        RVS_SWD,
        RVS_BAN,
        ORG_FNG,
        POK_THR,
        RVS_LBO,
        PLAYER_END,
        MS01SLA,
        MS01SQU,
        MS01WOLF,
        MS01DRA,
        MS01BEAR,
        MS01SCO,
        MS02DRA,
        MS02FOUR,
        MS03DRA,
        MS03FOUR,
        MS03FLY,
        MS04LEGEND,
        MS00DEATH,
        MS01SLAsilver,
        MS02SLAgold,
        MS00TREunit,
        MS05RYUunit,
        MS01BIRD,
        MS01CRAB,
        MS00KAIDAN,
        MS00FLOOR,
        MS01ELEMENT,
        MS03FOUR02,
        MS01UNIT,
        MS01FISH,
        MS02SHARK,
        MS03OCT,
        MS03SEA,
        MS00WALL,
        MS01ORG,
        MS01POK,
        MS01FLY,
        MS03ORG_SWORD,
        MS04_DRA_HEAD,
        MS04_DRA_TAIL,
        MS04_DRA_WING,
        MS01WIN,
        MS01_SLAWING,
        MS03_BD_CHAU,
        MS03_BD_GIGA,
        MS03_BD_RUSA,
        MS03_SR_TIAMAT,
        MS01_ORG_GUN,
        MS03FLY_02,
        MS01RVS,
        MAX,
    }
    public enum ENEMY_TYPE
    {
        SLIME = 1,
        SQUIRREL = 2,
        WOLF = 3,
        DRAGON_EGG = 4,
        BEAR = 5,
        SCORPION = 6,
        DRAGON1 = 7,
        BOAR = 8,
        DRAGON2 = 9,
        DRAGON3 = 10,
        DRAGON4 = 11,
        DRAGON5 = 12,
        DEATH = 13,
        SILVER_SLIME = 14,
        GOLD_SLIME = 15,
        TREASURE = 16,
        STAIRS = 20,
        MOVING_PANEL = 21,
        UNIT_ENEMY_GHOST = 22,
        TEMP_0 = 23,
        UNIT_ENEMY_HUMAN = 24,
        FISH = 25,
        SHARK = 26,
        OCTOPUS = 27,
        DRAGON_HIGH_NECK = 28,
        WALL = 29,
        UNIT_ENEMY_POKKULU = 30,
        UNIT_ENEMY_ORG = 31,
        FLIGHT = 32,
        UNIT_ENEMY_ONLY_ORG = 33,
        HUGE_DRAGON_HEAD = 34,
        HUGE_DRAGON_TAIL = 35,
        HUGE_DRAGON_WING = 36,
        CHARACTER_WING = 37,
        SLIME_WING = 38,
        ELEMENT = 39,
        GIANT = 40,
        JELLY = 41,
        GIANT_LOBO = 42,
        ORG_GUN = 43,
        DRAGON4_EX = 44,
        ANIMAL_LEG = 45,
    }
    public enum AttackPattern
    {
        NORMAL,
        QUICK,
        DELAY,
        DOWN_SOUL,
        POWUP_BY_LIFE,
        FIRST_DEADLY,
        FIRST_CURSE,
        GUARD_COMBO,
        INVISIBLE,
        REFLECT,
        MAX,
    }
    public enum Message_Name
    {
        QUEST_START1,
        QUEST_START2,
        QUEST_START3,
        MOVE_SOUL_5,
        MOVE_SOUL_3,
        MOVE_SOUL_0,
        MOVE_NONE_EVENT_MOVE,
        MOVE_BOSS_5,
        MOVE_DRAGON_5,
        BATTLE_START_NORMAL1,
        BATTLE_START_NORMAL2,
        BATTLE_START_NORMAL3,
        BATTLE_START_NORMAL4,
        BATTLE_START_NORMAL5,
        BATTLE_START_SOUL5,
        BATTLE_START_LIFE50,
        BATTLE_START_DEATH,
        BATTLE_START_BOSS,
        BATTLE_START_DRAGON,
        BATTLE_START_LASTBOSS1,
        BATTLE_START_LASTBOSS2,
        BATTLE_START_LASTBOSS3,
        BATTLE_START_TREASURE,
        BATTLE_DAMAGE30,
        BATTLE_SOUL3,
        BATTLE_GAMEOVER1,
        BATTLE_GAMEOVER2,
        BATTLE_GAMEOVER3,
        BATTLE_CONTINUE1,
        BATTLE_CONTINUE2,
        BATTLE_CONTINUE3,
        BATTLE_FINISH_NORMAL1,
        BATTLE_FINISH_NORMAL2,
        BATTLE_FINISH_NORMAL3,
        BATTLE_FINISH_LIFE50,
        BATTLE_FINISH_SOUL5,
        BATTLE_FINISH_BOSS1,
        BATTLE_FINISH_BOSS2,
        BATTLE_FINISH_BOSS3,
        BATTLE_FINISH_BOSS4,
        BATTLE_FINISH_TREASURE,
        BATTLE_FINISH_DESTROY_5,
        BATTLE_FINISH_DESTROY_10,
        BATTLE_FINISH_DESTROY_15,
        BATTLE_FINISH_DESTROY_20,
        BATTLE_FINISH_DESTROY_25,
        BATTLE_FINISH_DESTROY_30,
        BATTLE_FINISH_DESTROY_35,
        BATTLE_FINISH_DESTROY_40,
        BATTLE_FINISH_DESTROY_45,
        BATTLE_FINISH_DESTROY_50,
        FREE01,
        FREE02,
        FREE03,
        FREE04,
        FREE05,
        FREE06,
        FREE07,
        FREE08,
        FREE09,
        FREE10,
        GHOST_EVENT_01,
        GHOST_EVENT_02,
        GHOST_EVENT_03,
        BOSS_ATTACK_01,
        BOSS_ATTACK_02,
        BOSS_ATTACK_03,
        DistanceEvent_1,
        DistanceEvent_3,
        DistanceEvent_6,
        DistanceEvent_9,
        DistanceEvent_12,
        DistanceEvent_15,
        DistanceEvent_18,
        DistanceEvent_21,
        DistanceEvent_24,
        DistanceEvent_27,
        DistanceEvent_30,
        DistanceEvent_33,
        EnemyLogic_Quick,
        EnemyLogic_Delay,
        EnemyLogic_DelayAttack,
        EnemyLogic_SoulDown,
        EnemyLogic_PowupByLife,
        EnemyLogic_Deadly,
        EnemyLogic_Curse,
        EnemyLogic_GuardCombo,
        EnemyLogic_Invisible,
        EnemyLogic_Reflect,
        ALL,
    }
    public struct EnemyEffectAssign
    {
        public short m_EffectIndex;
        public string m_MainAtkEffectName;
        public string m_SubAtkEffectName;
        public EnemyEffectAssign(short id, string mainName, string subName)
        {
            this.m_EffectIndex = id;
            this.m_MainAtkEffectName = mainName;
            this.m_SubAtkEffectName = subName;
        }
    }
    public class OpenType
    {
        public string Type { get; set; }
        public string Param { get; set; }
        public OpenType()
        {
            Type = "未知";
            Param = string.Empty;
        }
        public OpenType(string type, string param)
        {
            Type = type;
            Param = param;
        }
    }
    public class UtilityBase
    {
        public static OpenType ParseOpentype(string type, string param)
        {
            OpenType result = new OpenType();
            switch (type)
            {
                case "0":
                    {
                        result.Type = string.Empty;
                        result.Param = string.Empty;
                        break;
                    }
                case "1":
                    {
                        result.Type = "每周";
                        switch (param)
                        {
                            case "0": result.Param = "日"; break;
                            case "1": result.Param = "一"; break;
                            case "2": result.Param = "二"; break;
                            case "3": result.Param = "三"; break;
                            case "4": result.Param = "四"; break;
                            case "5": result.Param = "五"; break;
                            case "6": result.Param = "六"; break;
                            case "7": result.Param = "日一二三四五六"; break;
                            default: break;
                        }
                        break;
                    }
                case "2":
                    {
                        result.Type = "完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        DB db = new DB();
                        result.Param = param + "|" + db.GetString(String.Format(sql, param));
                        break;
                    }
                case "3":   //not used
                    {
                        result.Type += "3";
                        result.Param += param;
                        break;
                    }
                case "4":
                    {
                        result.Type = "开始日期";
                        result.Param = ParseRTDDate(param);
                        break;
                    }
                case "5":
                    {
                        result.Type = "结束日期";
                        result.Param = ParseRTDDate(param);
                        break;
                    }
                case "6":
                    {
                        result.Type = "是否关闭";
                        result.Param = param;
                        break;
                    }
                case "7":
                    {
                        result.Type = "完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        DB db = new DB();
                        result.Param = param + "|" + db.GetString(String.Format(sql, param));
                        break;
                    }
                case "8":
                    {
                        result.Type = "支线任务";
                        result.Param = param;
                        break;
                    }
                case "9":
                    {
                        result.Type = "不完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        DB db = new DB();
                        result.Param = param + "|" + db.GetString(String.Format(sql, param));
                        break;
                    }
                case "10":
                    {
                        result.Type = "自身等级大于等于";
                        result.Param = param;
                        break;
                    }
                case "11":
                    {
                        result.Type = "自身等级小于等于";
                        result.Param = param;
                        break;
                    }
                case "12":
                    {
                        result.Type = "教程通过";
                        result.Param = param;
                        break;
                    }
                case "13":
                    {
                        result.Type = "教程未通过";
                        result.Param = param;
                        break;
                    }
                case "14":
                    {
                        result.Type = "队长限定";
                        //result.OpentypeParam = opentypeParam + "|" + ParseUnitName(opentypeParam);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("角色组|" + param);
                        sb.Append(ParseUnitGroupName(param));
                        result.Param = sb.ToString();
                        break;
                    }
                default:
                    {
                        result.Type += type;
                        result.Param += param;
                        break;
                    }
            }
            return result;
        }
        public static string ParsePresenttype(string presenttype)
        {
            switch (presenttype)
            {
                case "0": return "无";
                case "1": return "COIN";
                case "2": return "FP";
                case "3": return "STONE";
                case "4": return "UNIT";
                default: return "未知" + presenttype.ToString();
            }
        }
        public static string ParseBonustype(string bonustype)
        {
            switch (bonustype)
            {
                case "0":
                    {
                        return "无";
                    }
                case "1":
                    {
                        return "体力半减";
                    }
                case "2":
                    {
                        return "双倍钱";
                    }
                case "3":
                    {
                        return "双倍经验";
                    }
                case "4":
                    {
                        return "双倍魂?";
                    }
                case "5":
                    {
                        return "双倍掉落";
                    }
                default: return "未知" + bonustype.ToString();
            }
        }
        public static string ParseAttributetype(int attributetype)
        {
            return ParseRealAttributetype(attributetype).ToString();
        }
        private static UnitAttribute ParseRealAttributetype(int attributetype)
        {
            switch (attributetype)
            {
                case 1:
                    return UnitAttribute.NONE;
                case 2:
                    return UnitAttribute.FIRE;
                case 3:
                    return UnitAttribute.WATER;
                case 4:
                    return UnitAttribute.LIGHT;
                case 5:
                    return UnitAttribute.DARK;
                default:
                    return UnitAttribute.NONE;
            }
        }
        public static string ParseStyletype(int styletype)
        {
            if (styletype == 0)
            {
                return "NONE";
            }
            else
            {
                return ParseRealStyletype(styletype).ToString();
            }
        }
        private static Class ParseRealStyletype(int styletype)
        {
            switch (styletype)
            {
                case 1:
                    return Class.KNIGHT;
                case 2:
                    return Class.LANCER;
                case 3:
                    return Class.ARCHER;
                case 4:
                    return Class.WIZARD;
                default:
                    return Class.KNIGHT;
            }
        }
        #region AssignID

        public static readonly EnemyEffectAssign[] m_EffectList = new EnemyEffectAssign[]	{
		new EnemyEffectAssign(0, "ef_damage", null),
		new EnemyEffectAssign(1, "ef_damage", null),
		new EnemyEffectAssign(2, "ef_damage", "ef_fire_01"),
		new EnemyEffectAssign(3, "ef_tentacle_01", null),
		new EnemyEffectAssign(4, "ef_beam_01", null),
		new EnemyEffectAssign(5, "ef_atk_arrow_01_ms", null),
		new EnemyEffectAssign(6, "ef_atk_arrow_02_ms", null),
		new EnemyEffectAssign(7, "ef_atk_arrow_03_ms", null),
		new EnemyEffectAssign(8, "ef_atk_arrow_04_ms", null),
		new EnemyEffectAssign(9, "ef_atk_blade_01_ms", null),
		new EnemyEffectAssign(10, "ef_atk_blade_02_ms", null),
		new EnemyEffectAssign(11, "ef_atk_blade_03_ms", null),
		new EnemyEffectAssign(12, "ef_atk_blade_04_ms", null),
		new EnemyEffectAssign(13, "ef_atk_blade_05_ms", null),
		new EnemyEffectAssign(14, "ef_atk_magic_01_ms", null),
		new EnemyEffectAssign(15, "ef_atk_magic_03_ms", null),
		new EnemyEffectAssign(16, "ef_atk_magic_04_ms", null),
		new EnemyEffectAssign(17, "ef_atk_spear_01_ms", null),
		new EnemyEffectAssign(18, "ef_atk_spear_02_ms", null),
		new EnemyEffectAssign(19, "ef_atk_spear_03_ms", null),
		new EnemyEffectAssign(20, "ef_atk_spear_04_ms", null),
		new EnemyEffectAssign(21, "ef_atk_spear_05_ms", null),
		new EnemyEffectAssign(22, "ef_atk_spear_06_ms", null),
		new EnemyEffectAssign(23, "ef_wall_01", null),
		new EnemyEffectAssign(24, "ef_btl_thunder_01", null),
		new EnemyEffectAssign(25, "ef_btl_hurricane_01", null),
		new EnemyEffectAssign(26, "ef_btl_meteo_01", null),
		new EnemyEffectAssign(27, "ef_atk_arrow_05_ms", null),
		new EnemyEffectAssign(28, "ef_atk_blade_06_ms", null),
		new EnemyEffectAssign(29, "ef_atk_spear_07_ms", null),
		new EnemyEffectAssign(30, "ef_atk_arrow_06_ms", null),
		new EnemyEffectAssign(31, "ef_atk_magic_05_ms", null),
		new EnemyEffectAssign(32, null, "ef_btl_impact_01"),
		new EnemyEffectAssign(33, null, "ef_btl_breath_dark"),
		new EnemyEffectAssign(34, null, "ef_btl_blizzard"),
		new EnemyEffectAssign(35, "ef_fire_02", null),
		new EnemyEffectAssign(36, null, "ef_beam_02"),
		new EnemyEffectAssign(37, null, "ef_btl_sparkle"),
		new EnemyEffectAssign(38, "ef_btl_meteo_02", null),
		new EnemyEffectAssign(39, "ef_atk_blade_07_ms", null),
		new EnemyEffectAssign(40, "ef_atk_spear_08_ms", null),
		new EnemyEffectAssign(41, "ef_atk_blade_08_ms", null),
		new EnemyEffectAssign(42, "ef_atk_arrow_07_ms", null)
	};
        #endregion
        public static AssignID ParseUnitKind(int kind)
        {
            AssignID assignId = AssignID.SWORD;
            int length = m_EffectList.Length;
            for (int index = 0; index < length; ++index)
            {
                EnemyEffectAssign enemyEffectAssign = m_EffectList[index];
                if (kind == (int)enemyEffectAssign.m_EffectIndex)
                    assignId = (AssignID)index;
            }
            return assignId;
        }
        public static int ParseAssignID(AssignID category)
        {
            int num = 1;
            int index = (int)category;
            int length = m_EffectList.Length;
            if (index >= 0 && index < length)
                num = (int)m_EffectList[index].m_EffectIndex;
            return num;
        }
        public static string ParseQuestKind(string kind)
        {
            switch (kind)
            {
                case "0": return "NORMAL";
                case "1000": return "EVENT";
                case "1010": return "RUINS";
                case "1011": return "CAVE";
                case "1020": return "SP_EVENT";
                default: return string.Empty;
            }
        }
        public static string ParseZBTNKind(string kind)
        {
            switch (kind)
            {
                case "0": return "NORMAL";
                case "1": return "EVENT";
                case "2": return "LARGE_EVENT";
                case "3": return "SPECIAL";
                default: return string.Empty;
            }
        }
        public static string ParseSkillType<T>(T skilltype)
        {
            return skilltype.ToString();
        }
        public static string ParseEnemyType(int type)
        {
            return ((ENEMY_TYPE)type).ToString();
        }
        public static string ParseAttackPattern(int type)
        {
            return ((AttackPattern)type).ToString();
        }
        public static string ParseMessageName(int type)
        {
            return ((Message_Name)type).ToString();
        }
        public static string ParseRTDDate(string rtdDate)
        {
            if (string.IsNullOrWhiteSpace(rtdDate))
            {
                return string.Empty;
            }
            int i = int.Parse(rtdDate);
            if (i == 0)
            {
                return string.Empty;
            }
            int hour = i % 100;
            i /= 100;
            int day = i % 100;
            i /= 100;
            int month = i % 100;
            i /= 100;
            int year = i % 10000;
            DateTime t = new DateTime(year, month, day, hour, 0, 0);
            return t.ToString("yyyy-MM-dd HH:mm ddd");
        }
        public static string ParseUnitName(string unitId)
        {
            string sql = @"SELECT name FROM unit_master WHERE id={0}";
            DB db = new DB();
            return db.GetString(String.Format(sql, unitId));
        }
        public static string ParseUnitGroupName(string unitUiId)
        {
            string sql = @"SELECT id,name FROM unit_master WHERE ui_id={0}";
            DB db = new DB();
            DataTable dt = db.GetData(String.Format(sql, unitUiId));
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendFormat("{0}|{1}", dr["id"].ToString(), dr["name"].ToString());
                sb.AppendLine();
            }
            return sb.ToString().Trim();
        }
        public static string ParseText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            text = text.Replace(@"\n", "\n");
            Regex r = new Regex(@"(\[[a-zA-Z0-9]{6}\])(.*?)(\[-\])");
            return r.Replace(text, new MatchEvaluator(ParseTextEvaluator));
        }
        public static string ParseTextEvaluator(Match m)
        {
            string color = m.Groups[1].Value.Trim(new char[] { '[', ']' });
            //return String.Format("<span style='color:#{0}'>{1}</span>", color, m.Groups[2].Value);
            return m.Groups[2].Value;
        }
        public static int RealCalc(int baseAttr, int up, int lv)
        {
            return (int)Math.Round(baseAttr * ((lv - 1) * (up * 0.01) + 1));
        }
        public static string ParseBgmFileName(int no)
        {
            return "bgm_rtd_" + no.ToString("D2");
        }
        public static bool IsUnitEnemy(int num)
        {
            switch (num)
            {
                case 30:
                    return true;
                case 31:
                    return true;
                case 33:
                    return true;
                case 37:
                    return true;
                default:
                    switch (num - 22)
                    {
                        case 0:
                            return true;
                        case 2:
                            return true;
                        default:
                            return num == 45;
                    }
            }
        }
    }
}
