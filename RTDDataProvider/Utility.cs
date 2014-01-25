using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

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
        HeartCall,
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
        SoulPunisher
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
        ENEMY_DAMAGE_AND_DEFENCE_DOWN
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
        JELLY
    }
    public enum AttackPattern
    {
        NORMAL,
        QUICK,
        DELAY,
        DOWN_SOUL,
        MAX
    }
    public static class Utility
    {
        public static Dictionary<string, string> parseOpentype(string opentype, string opentypeParam)
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
                        result["opentypeParam"] = parseRTDDate(opentypeParam);
                        break;
                    }
                case "5":
                    {
                        result["opentype"] = "结束日期";
                        result["opentypeParam"] = parseRTDDate(opentypeParam);
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
                case "14":
                    {
                        result["opentype"] = "队长限定";
                        result["opentypeParam"] = parseUnitName(opentypeParam);
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
        public static string parsePresenttype(string presenttype)
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
        public static string parseBonustype(string bonustype)
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
        public static string parseAttributetype(int attributetype)
        {
            return parseRealAttributetype(attributetype).ToString();
        }
        public static UnitAttribute parseRealAttributetype(int attributetype)
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
        public static string parseStyletype(int styletype)
        {
            return parseRealStyletype(styletype).ToString();
        }
        public static Class parseRealStyletype(int styletype)
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
        public static string parseUnitKind(int kind)
        {
            return parseRealUnitKind(kind).ToString(); ;
        }
        public static AssignID parseRealUnitKind(int kind)
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
                default:
                    switch (kind)
                    {
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
                        default:
                            switch (kind)
                            {
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
                                default:
                                    switch (kind)
                                    {
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
            }
        }
        public static string parseQuestKind(string kind)
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
        public static string parseZBTNKind(string kind)
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
        public static string parseSkillType<T>(T skilltype)
        {
            return skilltype.ToString();
        }
        public static string parseEnemyType(int type)
        {
            return ((ENEMY_TYPE)type).ToString();
        }
        public static string parseAttackPattern(int type)
        {
            return ((AttackPattern)type).ToString();
        }
        public static string parseRTDDate(string rtdDate)
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
        public static string parseUnitName(string unitId)
        {
            string sql = @"SELECT name FROM unit_master WHERE id={0}";
            DB db = new DB();
            return db.GetString(String.Format(sql, unitId));
        }
        public static string parseText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            text = text.Replace(@"\n", "\n");
            Regex r = new Regex(@"(\[[a-zA-Z0-9]{6}\])(.*?)(\[-\])");
            return r.Replace(text, new MatchEvaluator(parseTextEvaluator));
        }
        public static string parseTextEvaluator(Match m)
        {
            string color = m.Groups[1].Value.Trim(new char[] { '[', ']' });
            //return String.Format("<span style='color:#{0}'>{1}</span>", color, m.Groups[2].Value);
            return m.Groups[2].Value;
        }
        public static FlowDocument parseTextToDocument(string text)
        {
            var flowDoc = new FlowDocument();
            //string[] textParas = text.Split(new string[] { "\\n" }, StringSplitOptions.None);
            text = text.Replace(@"\n", "\n");
            Paragraph pr = new Paragraph(); //prprpr
            pr.Margin = new Thickness(0);
            Regex rSplit = new Regex(@"(\[[a-zA-Z0-9]{6}\])(.*?)(\[-\])");
            Regex rColor = new Regex(@"(\[[a-zA-Z0-9]{6}\])");
            var textParts = rSplit.Split(text);
            var nowFontColor = Brushes.Black;
            foreach (string textPart in textParts)
            {
                Span span = new Span();
                if (rColor.Match(textPart).Success)
                {
                    string color = textPart.Trim(new char[] { '[', ']' });
                    nowFontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + color));
                    continue;
                }
                if (textPart == "[-]")
                {
                    nowFontColor = Brushes.Black;
                    continue;
                }
                span.Inlines.Add(new Run(textPart));
                span.Foreground = nowFontColor;
                pr.Inlines.Add(span);
            }
            flowDoc.Blocks.Add(pr);
            return flowDoc;
        }
        public static int RealCalc(int baseAttr, int up, int lv)
        {
            return (int)Math.Round(baseAttr * ((lv - 1) * (up * 0.01) + 1));
        }
        public static string parseBgmFileName(int no)
        {
            return "bgm_rtd_" + no.ToString("D2");
        }
    }
}
