using System;
using System.Collections.Generic;
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
        ATTACK_GRADEUP,
        RECOVER_GRADEUP,
        INITIAL_SOUL_GRADEUP,
        DAMAGE_GRADEDOWN,
        SOUL_GRADEUP,
        ACTIVESKILL_GRADEUP,
        TURNSPEED_GRADEUP,
        DRAGON_KILLER,
        DEATH_KILLER,
        LUCK_GRADEUP,
        RECOVER_PANELL_GRADEUP,
        SUPERCRITICAL_GRADEUP,
        SUPER_DEFFENCE,
        SUPER_ATTACK,
        CHAIN_GRADEUP,
        ATTRIBUTE_PANEL_GRADEUP,
        ATTRIBUTE_KILLER,
        MOVE_LIFE_RECOVER,
        GET_TREASURE_SOUL,
        LIFE_ATTRIBUTE_UP,
        LIFE_ATTRIBITE_DRAIN,
        COUNTER,
        DOUBLE_ATTACK,
        SOUL_STEP,
        DRACOTTER,
        GRAVITY_ATTACK,
        SKILL_ZERO,
        SOUL_HUNT,
        HEALING_AND_ASSULT,
        ASSULT_AND_LIFE,
        LIFE_AND_HEALING,
        DOUBLE_KILLER,
        MASTER_OF_PANEL,
        QUICK_BLAST,
        ARCANE_CROW,
        SERIAL_KILLER,
        LIFE_GUARD,
        GUARDIAN_HEAL,
        LIFE_DOUBLE_COUNTER,
        SOUL_DOUBLE_COUNTER,
        ATTRIBUTE_SOUL_DRAIN,
        MIND_RECOVER_UP,
        MIND_ASSULT_UP,
        MIND_LIFE_UP,
        MIND_SOUL_UP,
        SkillAbsorb,
        GodReflect,
        Dragon_Slayer,
        CHASE_SOUL_GET,
        LIFE_SOULFREE,
        MINIMUM_POWER,
        MIN_SOUL_POWER,
        COLOR_ASSULT,
        LEADER_COPY,
        PANEL_HEALER,
        LINK_PANEL_DELAY,
        WEAK_ATTACK_DELAY,
        DELAY_CHASE_ATTACK,
        DELAY_HEAL_LIFE,
        ATTRIBUTEUP_SOUL_GET,
        TRIPLE_SUPPORT,
        CHASE_SUPPORT,
        LINK_PANEL_HEAL,
        MOB_KILLER,
        SERIAL_LIFE_UP,
        DIFFER_CLASS_ATTACK,
        ELEMENT_POWER,
        AKIBA_RECOVER,
        AKIBA_ASSULT,
        AKIBA_LIFEUP,
        AKIBA_SOULUP,
        FUSION_BLAST,
        CLASS_SUPPORT,
        GET_TREASURE_ENHANCE,
        DROP_MONEYUP,
        TURN_ENHANCE,
        DAMAGE_ENHANCE,
        DAMAGE_DRAIN,
        SERIAL_HEAL_UP,
        ATTACK_ENHANCE,
        MOVE_ENHANCE,
        ATTRIBUTE_DRAIN_AND_KILLER,
        UNIT_ENEMY_KILLER,
        SP_PANEL_UP,
        BUFFER
    }
    public enum ActiveSkillType
    {
        HealLife1 = 1,
        HealLife2,
        DirectAttack1,
        DirectAttack2,
        PanelChange,
        AttributePanelUp,
        DefenceUp,
        DefenceDown,
        Gravity,
        AnlimitedPanel,
        Astral,
        Acute,
        Enhance,
        Arcane,
        Delirium,
        AttributeAttack,
        Destruct,
        Escape,
        SuperDefenceUp,
        AttributeStock,
        DeliriumHeven,
        MultiAttack,
        SoulFree,
        EnemyScan,
        TimeStop,
        GravityAttack,
        PanelCopy,
        PerfectDefence,
        Curse,
        StrongStyle,
        MindOfZero,
        HealCanon,
        DragonInjection,
        Resurrection,
        GodBurst,
        ColorEnhance,
        InfiniteAttack,
        AllPanelAttack,
        DelayDamageAttack,
        HeartPanelAttack,
        PanelCall,
        DragonBind,
        Change_AttackHeal,
        HealEnhance,
        HealAndAttackUp,
        HealAndDefenceUp,
        AstralHeal,
        FinishBurst,
        TreasureHunt,
        AttributeChange,
        DelayDamageGravity,
        FullHealAndGetSoul,
        MultiBurst,
        ExpansionDestruct,
        JumpAttack,
        MoneyAttack,
        LimitEnhance,
        DamageAttack,
        CounterAttackUp,
        ExPanelAttack,
        AttrAtkAndAtkUp,
        DefenceUpAndEnhance,
        DragonEnhance,
        SoulOfZero,
        AttackCanon
    }
    public enum PanelSkillType
    {
        LIFERECOVER_CALC = 1,
        LIFERECOVER_FIX,
        SOULGET,
        CHASE_ATTACK,
        ENEMY_DEFENCE_DOWN,
        ENEMY_DAMAGE,
        LIFE_AND_SOUL,
        ATTACK_UP,
        GRAVITY,
        SHUFFLE,
        ATTRIBUTE_ADD,
        SOULGET_2,
        CURSE,
        ARCANE,
        LIMIT_ENHANCE,
        LIMIT_SUPERATTACK,
        MOVESOUL_OR_BATTLELIFE,
        BATTLE_EVASION,
        LIMIT_LIFERECOVER_FIX,
        LIFE_UP_LIMIT,
        RANDOM_EFFECT,
        ENEMY_DAMAGE_AND_DEFENCE_DOWN,
        HEAL_CANON
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
        MAX
    }
    public enum ENEMY_TYPE
    {
        SLIME = 1,
        SQUIRREL,
        WOLF,
        DRAGON_EGG,
        BEAR,
        SCORPION,
        DRAGON1,
        BOAR,
        DRAGON2,
        DRAGON3,
        DRAGON4,
        DRAGON5,
        DEATH,
        SILVER_SLIME,
        GOLD_SLIME,
        TREASURE,
        STAIRS = 20,
        MOVING_PANEL,
        UNIT_ENEMY_GHOST,
        TEMP_0,
        UNIT_ENEMY_HUMAN,
        FISH,
        SHARK,
        OCTOPUS,
        DRAGON_HIGH_NECK,
        WALL,
        UNIT_ENEMY_POKKULU,
        UNIT_ENEMY_ORG,
        FLIGHT,
        UNIT_ENEMY_ONLY_ORG,
        HUGE_DRAGON_HEAD,
        HUGE_DRAGON_TAIL,
        HUGE_DRAGON_WING,
        CHARACTER_WING,
        SLIME_WING,
        ELEMENT,
        GIANT,
        JELLY,
        GIANT_LOBO,
        ORG_GUN
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
        MAX
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
        ALL
    }
    public class UtilityBase
    {
        public static Dictionary<string, string> ParseOpentype(string opentype, string opentypeParam)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("opentype", "未知");
            result.Add("opentypeParam", "未知");
            switch (opentype)
            {
                case "0":
                    {
                        result["opentype"] = "无";
                        result["opentypeParam"] = string.Empty;
                        break;
                    }
                case "1":
                    {
                        result["opentype"] = "每周";
                        switch (opentypeParam)
                        {
                            case "0": result["opentypeParam"] = "日"; break;
                            case "1": result["opentypeParam"] = "一"; break;
                            case "2": result["opentypeParam"] = "二"; break;
                            case "3": result["opentypeParam"] = "三"; break;
                            case "4": result["opentypeParam"] = "四"; break;
                            case "5": result["opentypeParam"] = "五"; break;
                            case "6": result["opentypeParam"] = "六"; break;
                            case "7": result["opentypeParam"] = "日一二三四五六"; break;
                            default: break;
                        }
                        break;
                    }
                case "2":
                    {
                        result["opentype"] = "完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        DB db = new DB();
                        result["opentypeParam"] = db.GetString(String.Format(sql, opentypeParam));
                        break;
                    }
                case "3":   //unknown
                    {
                        result["opentype"] += "3";
                        result["opentypeParam"] += opentypeParam;
                        break;
                    }
                case "4":
                    {
                        result["opentype"] = "开始日期";
                        result["opentypeParam"] = ParseRTDDate(opentypeParam);
                        break;
                    }
                case "5":
                    {
                        result["opentype"] = "结束日期";
                        result["opentypeParam"] = ParseRTDDate(opentypeParam);
                        break;
                    }
                case "6":
                    {
                        result["opentype"] = "是否关闭";
                        result["opentypeParam"] = opentypeParam;
                        break;
                    }
                case "7":
                    {
                        result["opentype"] = "完成关卡?";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        DB db = new DB();
                        result["opentypeParam"] = db.GetString(String.Format(sql, opentypeParam));
                        break;
                    }
                case "8":
                    {
                        result["opentype"] = "SubQuestOnly";
                        result["opentypeParam"] = opentypeParam;
                        break;
                    }
                case "9":
                    {
                        result["opentype"] = "不完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        DB db = new DB();
                        result["opentypeParam"] = db.GetString(String.Format(sql, opentypeParam));
                        break;
                    }
                case "10":
                    {
                        result["opentype"] = "自身等级大于等于";
                        result["opentypeParam"] = opentypeParam;
                        break;
                    }
                case "11":
                    {
                        result["opentype"] = "自身等级小于等于";
                        result["opentypeParam"] = opentypeParam;
                        break;
                    }
                case "12":
                    {
                        result["opentype"] = "教程通过";
                        result["opentypeParam"] = opentypeParam;
                        break;
                    }
                case "13":
                    {
                        result["opentype"] = "教程未通过";
                        result["opentypeParam"] = opentypeParam;
                        break;
                    }
                case "14":
                    {
                        result["opentype"] = "队长限定";
                        result["opentypeParam"] = ParseUnitName(opentypeParam);
                        break;
                    }
                default:
                    {
                        result["opentype"] += opentype;
                        result["opentypeParam"] += opentypeParam;
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
                default: return string.Empty;
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
                default: return string.Empty;
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
        public static string ParseUnitKind(int kind)
        {
            return ParseRealUnitKind(kind).ToString(); ;
        }
        private static AssignID ParseRealUnitKind(int kind)
        {
            {
                switch (kind)
                {
                    case 1:
                        return AssignID.SWORD;
                    case 2:
                        return AssignID.GREATSWORD;
                    case 3:
                        return AssignID.TWO_SWORD;
                    case 4:
                        return AssignID.CHAOS_SWD;
                    case 5:
                        return AssignID.RAPIA;
                    case 6:
                        return AssignID.ORG_SWORD;
                    case 7:
                        return AssignID.GRT_FEMALE;
                    case 8:
                        return AssignID.POK_SWORD;
                    case 9:
                        return AssignID.WIN_SWORD;
                    case 101:
                        return AssignID.LANCE;
                    case 102:
                        return AssignID.PILEBANKER;
                    case 103:
                        return AssignID.TWIN_LANCE;
                    case 104:
                        return AssignID.CHAOS_BAN;
                    case 105:
                        return AssignID.KUNGFU;
                    case 106:
                        return AssignID.POK_LANCE;
                    case 107:
                        return AssignID.LACNE_FEMALE;
                    case 108:
                        return AssignID.ORG_LAN;
                    case 109:
                        return AssignID.WIN_LANCE;
                    case 201:
                        return AssignID.BOW;
                    case 202:
                        return AssignID.GUN;
                    case 203:
                        return AssignID.CANNON;
                    case 204:
                        return AssignID.CHAOS_GUN;
                    case 205:
                        return AssignID.BOW_MALE;
                    case 206:
                        return AssignID.ORG_GUN;
                    case 207:
                        return AssignID.POK_CAN;
                    case 208:
                        return AssignID.WIN_BOW;
                    case 209:
                        return AssignID.GOD_BOW;
                    case 301:
                        return AssignID.STICK;
                    case 302:
                        return AssignID.ARTIFACT;
                    case 303:
                        return AssignID.STICK_FEMALE;
                    case 304:
                        return AssignID.CHAOS_ART;
                    case 305:
                        return AssignID.POK_STICK;
                    case 306:
                        return AssignID.ART_FEMALE;
                    case 307:
                        return AssignID.WIN_STICK;
                    case 308:
                        return AssignID.ORG_STICK;
                    case 309:
                        return AssignID.POK_ART;
                    default:
                        return AssignID.SWORD;
                }

            }
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
            return t.ToString("yyyy-MM-dd HH:mm");
        }
        public static string ParseUnitName(string unitId)
        {
            string sql = @"SELECT name FROM unit_master WHERE id={0}";
            DB db = new DB();
            return db.GetString(String.Format(sql, unitId));
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
        public static bool IsUnitEnemy(int type)
        {
            switch (type)
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
                    switch (type - 22)
                    {
                        case 0:
                            return true;
                        case 2:
                            return true;
                        default:
                            return false;
                    }
            }
        }
    }
}
