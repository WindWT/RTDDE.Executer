using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTDDataExecuter
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
    public enum PartySkillType
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
        CounterAttackUp
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
        RANDOM_EFFECT
    }
    public enum SkillPhase
    {
        MOVE = 1,
        BATTLE,
        ALL
    }
    public partial class MainWindow : Window
    {
        private static Dictionary<string, string> parseOpentype(string opentype, string opentypeParam)
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
        private static Dictionary<string, string> parsePresenttype(string presenttype, string presentParam)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("presenttype", "未知");
            result.Add("presentParam", "未知");
            switch (presenttype)
            {
                case "0":
                    {
                        result["presenttype"] = "无";
                        result["presentParam"] = String.Empty;
                        break;
                    }
                case "1":
                    {
                        result["presenttype"] = "COIN";
                        result["presentParam"] = presentParam;
                        break;
                    }
                case "2":
                    {
                        result["presenttype"] = "FP";
                        result["presentParam"] = presentParam;
                        break;
                    }
                case "3":
                    {
                        result["presenttype"] = "STONE";
                        result["presentParam"] = presentParam;
                        break;
                    }
                case "4":
                    {
                        result["presenttype"] = "UNIT";
                        result["presentParam"] = parseUnitName(presentParam);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return result;
        }
        private static string parseBonustype(string bonustype)
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
        private static string parseAttributetype(UnitAttribute attributetype)
        {
            return attributetype.ToString();
            /*switch (attributetype)
            {
                case UnitAttribute.NONE:
                    {
                        return "无";
                    }
                case UnitAttribute.LIGHT:
                    {
                        return "光";
                    }
                case UnitAttribute.DARK:
                    {
                        return "暗";
                    }
                case UnitAttribute.FIRE:
                    {
                        return "火";
                    }
                case UnitAttribute.WATER:
                    {
                        return "水";
                    }
                case UnitAttribute.ALL:
                    {
                        return "ALL";
                    }
                default: return string.Empty;
            }*/
        }
        private static string parseStyletype(Class styletype)
        {
            return styletype.ToString();
            /*switch (styletype)
            {
                case Class.KNIGHT:
                    {
                        return "剑";
                    }
                case Class.LANCER:
                    {
                        return "枪";
                    }
                case Class.ARCHER:
                    {
                        return "弓";
                    }
                case Class.WIZARD:
                    {
                        return "杖";
                    }
                case Class.ALL:
                    {
                        return "ALL";
                    }
                default: return string.Empty;
            }*/
        }
        private static string parseSkillType<T>(T skilltype)
        {
            return skilltype.ToString();
        }
        private static string parseRTDDate(string rtdDate)
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
        private static string parseUnitName(string unitId)
        {
            string sql = @"SELECT name FROM unit_master WHERE id={0}";
            DB db = new DB();
            return db.GetString(String.Format(sql, unitId));
        }
        private static string parseText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            text = text.Replace(@"\n", "\n");
            Regex r = new Regex(@"(\[[a-zA-Z0-9]{6}\])(.*?)(\[-\])");
            return r.Replace(text, new MatchEvaluator(parseTextEvaluator));
        }
        private static string parseTextEvaluator(Match m)
        {
            string color = m.Groups[1].Value.Trim(new char[] { '[', ']' });
            //return String.Format("<span style='color:#{0}'>{1}</span>", color, m.Groups[2].Value);
            return m.Groups[2].Value;
        }
        private static FlowDocument parseTextToDocument(string text)
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
        private static int RealCalc(int baseAttr, int up, int lv)
        {
            return (int)Math.Round(baseAttr * ((lv - 1) * (up * 0.01) + 1));
        }
    }
}
