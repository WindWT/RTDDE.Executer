using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using RTDDataProvider;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace RTDDataExecuter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private TaskScheduler uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        #region QuestViewer
        private static string QuestViewerSQL = @"SELECT id,
       name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
       ( CASE
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                ELSE 0 
       END ) AS [end]
  FROM quest_master
 ORDER BY start DESC,end DESC,id DESC;";

        private void QuestViewerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestViewerDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string eqInfo_id = ((DataRowView)QuestViewerDataGrid.SelectedItem).Row["id"].ToString();
            QuestInfo_id.Text = eqInfo_id;
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = @"SELECT id,name,pt_num,difficulty,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
(select text from quest_category_master where quest_category_master.id=category) as text,
open_type_1,open_param_1,
open_type_2,open_param_2,
open_type_3,open_param_3,
open_type_4,open_param_4,
open_type_5,open_param_5,
open_type_6,open_param_6,
bonus_type,bonus_start,bonus_end,
panel_sword,panel_lance,panel_archer,panel_cane,panel_heart,panel_sp,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_0) as challenge0,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_1) as challenge1,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_2) as challenge2
  FROM quest_master WHERE id={0}";
                DB db = new DB();
                return db.GetData(String.Format(sql, eqInfo_id));
            });
            Task<List<Dictionary<string, string>>> taskParse = new Task<List<Dictionary<string, string>>>(() =>
                {
                    List<Dictionary<string, string>> opentypeList = new List<Dictionary<string, string>>();
                    Task.WaitAll(task);
                    DataRow dr = task.Result.Rows[0];
                    if (dr == null || dr.ItemArray.Length == 0)
                    {
                        return null;
                    }
                    opentypeList.Add(parseOpentype(dr["open_type_1"].ToString(), dr["open_param_1"].ToString()));
                    opentypeList.Add(parseOpentype(dr["open_type_2"].ToString(), dr["open_param_2"].ToString()));
                    opentypeList.Add(parseOpentype(dr["open_type_3"].ToString(), dr["open_param_3"].ToString()));
                    opentypeList.Add(parseOpentype(dr["open_type_4"].ToString(), dr["open_param_4"].ToString()));
                    opentypeList.Add(parseOpentype(dr["open_type_5"].ToString(), dr["open_param_5"].ToString()));
                    opentypeList.Add(parseOpentype(dr["open_type_6"].ToString(), dr["open_param_6"].ToString()));
                    return opentypeList;
                }
            );
            taskParse.ContinueWith(t =>
            {
                DataRow dr = task.Result.Rows[0];
                if (dr == null || dr.ItemArray.Length == 0)
                {
                    return;
                }
                QuestInfo_name.Text = dr["name"].ToString();
                QuestInfo_pt.Text = dr["pt_num"].ToString();
                QuestInfo_difficulty.Text = dr["difficulty"].ToString();
                QuestInfo_stamina.Text = dr["stamina"].ToString();
                QuestInfo_category.Text = dr["category"].ToString();
                QuestInfo_text.Text = parseText(dr["text"].ToString());

                QuestInfo_opentype1_name.Text = t.Result[0]["opentype"];
                QuestInfo_opentype1_value.Text = t.Result[0]["opentypeParam"];
                QuestInfo_opentype2_name.Text = t.Result[1]["opentype"];
                QuestInfo_opentype2_value.Text = t.Result[1]["opentypeParam"];
                QuestInfo_opentype3_name.Text = t.Result[2]["opentype"];
                QuestInfo_opentype3_value.Text = t.Result[2]["opentypeParam"];
                QuestInfo_opentype4_name.Text = t.Result[3]["opentype"];
                QuestInfo_opentype4_value.Text = t.Result[3]["opentypeParam"];
                QuestInfo_opentype5_name.Text = t.Result[4]["opentype"];
                QuestInfo_opentype5_value.Text = t.Result[4]["opentypeParam"];
                QuestInfo_opentype6_name.Text = t.Result[5]["opentype"];
                QuestInfo_opentype6_value.Text = t.Result[5]["opentypeParam"];

                QuestInfo_bonus.Text = parseBonustype(dr["bonus_type"].ToString());
                QuestInfo_bonus_start.Text = parseRTDDate(dr["bonus_start"].ToString());
                QuestInfo_bonus_end.Text = parseRTDDate(dr["bonus_end"].ToString());

                QuestInfo_panel_sword.Text = dr["panel_sword"].ToString();
                QuestInfo_panel_lance.Text = dr["panel_lance"].ToString();
                QuestInfo_panel_archer.Text = dr["panel_archer"].ToString();
                QuestInfo_panel_cane.Text = dr["panel_cane"].ToString();
                QuestInfo_panel_heart.Text = dr["panel_heart"].ToString();
                QuestInfo_panel_sp.Text = dr["panel_sp"].ToString();

                QuestInfo_challenge0.Text = parseText(dr["challenge0"].ToString());
                QuestInfo_challenge1.Text = parseText(dr["challenge1"].ToString());
                QuestInfo_challenge2.Text = parseText(dr["challenge2"].ToString());
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskParse.Start();
        }

        private void QuestTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            QuestViewerSQL = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
       ( CASE
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                ELSE 0 
       END ) AS [end]
  FROM quest_master
 ORDER BY start DESC,end DESC,id DESC;";
            QuestViewerDataGrid_BindData();
        }

        private void QuestTypeRadio_Daily_Checked(object sender, RoutedEventArgs e)
        {
            string today = DateTime.Today.AddHours(1).ToString("yyyyMMddHH");
            QuestViewerSQL = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
(select text from quest_category_master where quest_category_master.id=category) as text,
       ( CASE
                WHEN open_type_1 = 1 THEN open_param_1 
                WHEN open_type_2 = 1 THEN open_param_2 
                WHEN open_type_3 = 1 THEN open_param_3 
                WHEN open_type_4 = 1 THEN open_param_4 
                WHEN open_type_5 = 1 THEN open_param_5 
                WHEN open_type_6 = 1 THEN open_param_6 
                ELSE -1
       END ) AS DayOfWeek,
       ( CASE
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                ELSE 0 
       END ) AS [end],
       ( CASE
                WHEN open_type_1 = 6 THEN open_param_1 
                WHEN open_type_2 = 6 THEN open_param_2 
                WHEN open_type_3 = 6 THEN open_param_3 
                WHEN open_type_4 = 6 THEN open_param_4 
                WHEN open_type_5 = 6 THEN open_param_5 
                WHEN open_type_6 = 6 THEN open_param_6 
                ELSE 0 
       END ) AS isDisabled
FROM QUEST_MASTER
WHERE DayOfWeek>=0
AND isDisabled=0
AND ([end]>" + today + @" OR [end]=0)
ORDER BY DayOfWeek,id DESC";
            QuestViewerDataGrid_BindData();
        }

        private void QuestTypeRadio_Main_Checked(object sender, RoutedEventArgs e)
        {
            QuestViewerSQL = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category
FROM QUEST_MASTER
WHERE category<1000
ORDER BY id DESC";
            QuestViewerDataGrid_BindData();
        }

        private void QuestViewerDataGrid_BindData()
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(QuestViewerSQL);
            });
            task.ContinueWith(t =>
            {
                QuestViewerDataGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
        #endregion

        #region utility
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
                        string sql = @"SELECT name FROM unit_master WHERE id={0}";
                        DB db = new DB();
                        result["opentypeParam"] = db.GetString(String.Format(sql, opentypeParam));
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
        #endregion

        #region MapViewer

        private void InitMap(string levelID, int repeat = 1)
        {
            MapGrid.Children.Clear();
            MapGrid.ColumnDefinitions.Clear();
            MapGrid.RowDefinitions.Clear();

            Task<DataTable> initMonsterTask = new Task<DataTable>(getMonsterData, levelID);
            initMonsterTask.ContinueWith(t =>
                {
                    MapMonsterGrid.ItemsSource = t.Result.DefaultView;
                }, uiTaskScheduler);

            Task<DataTable> task = new Task<DataTable>(() =>
                {
                    DB db = new DB();
                    DataTable dt = db.GetData("SELECT a.*,b.distance FROM level_data_master a left join quest_master b on a.level_data_id=b.id WHERE a.level_data_id=" + levelID);
                    return dt;
                }
            );
            task.ContinueWith(t =>
            {
                if (t.Result.Rows.Count == 0)
                {
                    return;
                }
                else
                {
                    DataRow levelData = t.Result.Rows[0];
                    DrawMap(
                    levelData["map_data"].ToString(),
                    Convert.ToInt32(levelData["width"]),
                    Convert.ToInt32(levelData["height"]),
                    Convert.ToInt32(levelData["start_x"]),
                    Convert.ToInt32(levelData["start_y"]),
                    Convert.ToInt32(levelData["distance"]),
                    repeat
                    );
                    ReDrawMap(levelID, initMonsterTask.Result);
                }
            }, uiTaskScheduler);
            task.Start();
            initMonsterTask.Start();
        }

        private void DrawMap(string mapData, int w, int h, int x, int y, int distance, int repeat)
        {
            if (repeat < 1)
            {
                repeat = 1;
            }
            for (int j = 0; j < h + 1; j++)
            {
                var colDef = new ColumnDefinition();
                colDef.Width = new GridLength(25);
                MapGrid.ColumnDefinitions.Add(colDef);
            }
            for (int r = 0; r < repeat; r++)
            {
                for (int i = 0; i < w; i++)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(25);
                    MapGrid.RowDefinitions.Add(rowDef);

                    //添加行标记
                    TextBox tbMark = new TextBox();
                    tbMark.Text = (y - i).ToString();

                    MapGrid.Children.Add(tbMark);
                    tbMark.SetValue(Grid.RowProperty, i);
                    tbMark.SetValue(Grid.ColumnProperty, 0);

                    for (int j = 0; j < h; j++)
                    {
                        TextBox tb = new TextBox();
                        //tb.Style = tbStyle;

                        string cellData = mapData.Substring((j * w + i) * 2, 2);
                        if (i == y && j == x)
                        {
                            cellData = "★";
                        }
                        if (cellData != "★")
                        {
                            int cellDataInt = int.Parse(cellData, System.Globalization.NumberStyles.HexNumber);
                            int num = 7 & cellDataInt >> 5;
                            if (num > 0)
                            {
                                switch (1 << (num - 1))
                                {
                                    //AttributeTypeLight,
                                    case 1:
                                        {
                                            tb.Background = Brushes.Yellow;
                                            break;
                                        }
                                    //AttributeTypeDark,
                                    case 2:
                                        {
                                            tb.Background = Brushes.Purple;
                                            tb.Foreground = Brushes.White;
                                            break;
                                        }
                                    //AttributeTypeFire = 4,
                                    case 4:
                                        {
                                            tb.Background = Brushes.Pink;
                                            break;
                                        }
                                    //AttributeTypeWater = 8,
                                    case 8:
                                        {
                                            tb.Background = Brushes.Aqua;
                                            break;
                                        }
                                    //AttributeTypeBossStart = 16,
                                    case 16:
                                        {
                                            tb.Background = Brushes.Silver;
                                            break;
                                        }
                                    //AttributeTypeBoss = 32,
                                    case 32:
                                        {
                                            tb.Background = Brushes.Black;
                                            break;
                                        }
                                }
                            }

                            var num2 = 31 & cellDataInt;
                            if (num2 >= 24)
                            {
                                //this.TreasureID = num2 - 23;
                                //this.EnemyUnitID = 0;
                                //cellData = "箱" + (num2 - 23);
                                cellData = "箱";
                            }
                            else
                            {
                                //this.EnemyUnitID = num2;
                                //this.TreasureID = 0;
                                cellData = "E" + (num2);
                                if (num2 != 0)
                                {
                                    //td.Attributes.Add("enemy", cellData);
                                }
                                if (num2 >= 17)
                                {
                                    //E17以上一般都是史莱姆
                                    tb.Foreground = Brushes.Red;
                                    tb.FontWeight = FontWeights.Bold;
                                }
                            }
                            if ((cellDataInt == 0) || (num2 == 0))
                            {
                                cellData = "";
                            }
                        }
                        tb.Text = cellData;

                        MapGrid.Children.Add(tb);
                        tb.SetValue(Grid.RowProperty, i);
                        tb.SetValue(Grid.ColumnProperty, j + 1);
                    }
                }
            }
            //添加标记
            TextBox tbEmpty = new TextBox();
            var rowDefMark = new RowDefinition();
            rowDefMark.Height = new GridLength(25);
            MapGrid.RowDefinitions.Add(rowDefMark);

            MapGrid.Children.Add(tbEmpty);
            tbEmpty.SetValue(Grid.RowProperty, w * repeat);
            tbEmpty.SetValue(Grid.ColumnProperty, 0);

            for (int j = 0; j < h; j++)
            {
                TextBox tbMark = new TextBox();
                tbMark.Text = (distance - j + 3).ToString();    //magic number 3!

                MapGrid.Children.Add(tbMark);
                tbMark.SetValue(Grid.RowProperty, w * repeat);
                tbMark.SetValue(Grid.ColumnProperty, j + 1);
            }
        }

        private DataTable getMonsterData(object param)
        {
            string levelID = param.ToString();
            DB db = new DB();
            DataTable questData = db.GetData("SELECT * FROM quest_master WHERE id=" + levelID);
            DataTable monsterData = new DataTable();
            monsterData.Columns.Add("#", typeof(string));
            monsterData.Columns.Add("id", typeof(int));
            monsterData.Columns.Add("lv_min", typeof(int));
            monsterData.Columns.Add("lv_max", typeof(int));
            monsterData.Columns.Add("rate", typeof(int));
            monsterData.Columns.Add("drop_id", typeof(int));
            monsterData.Columns.Add("isDragon", typeof(int));
            monsterData.Columns.Add("name", typeof(string));
            monsterData.Columns.Add("model", typeof(string));
            monsterData.Columns.Add("texture", typeof(string));
            monsterData.Columns.Add("type", typeof(int));
            monsterData.Columns.Add("attribute", typeof(int));
            monsterData.Columns.Add("soul_pt", typeof(int));
            monsterData.Columns.Add("gold_pt", typeof(int));
            monsterData.Columns.Add("turn", typeof(int));
            //monsterData.Columns.Add("LvMinLife", typeof(int));
            monsterData.Columns.Add("LvMaxLife", typeof(int));
            //monsterData.Columns.Add("LvMinATK", typeof(int));
            monsterData.Columns.Add("LvMaxATK", typeof(int));
            //monsterData.Columns.Add("LvMinDEF", typeof(int));
            monsterData.Columns.Add("LvMaxDEF", typeof(int));

            if (questData.Rows.Count == 0)
            {
                return monsterData;
            }
            string enemy_table_id = questData.Rows[0]["enemy_table_id"].ToString();
            DataTable enemyTableData = db.GetData("SELECT * FROM enemy_table_master WHERE id=" + enemy_table_id);
            for (int i = 1; i <= 18; i++)
            {
                if (enemyTableData.Rows[0]["enemy" + i + "_set_id"].ToString() != "0")
                {
                    monsterData.Rows.Add(
                    new object[]{
                        "E"+enemyTableData.Rows[0]["enemy"+i+"_id"],
                        enemyTableData.Rows[0]["enemy"+i+"_set_id"],
                        enemyTableData.Rows[0]["enemy"+i+"_lv_min"],
                        enemyTableData.Rows[0]["enemy"+i+"_lv_max"],
                        enemyTableData.Rows[0]["enemy"+i+"_rate"],
                        enemyTableData.Rows[0]["enemy"+i+"_drop_id"]
                    });
                }
            }
            monsterData.Rows.Add(
               new object[]{
                "DEATH",//实际上应该是DEATH，不知道为什么表名是boss
                enemyTableData.Rows[0]["boss_set_id"],
                enemyTableData.Rows[0]["boss_lv_min"],
                enemyTableData.Rows[0]["boss_lv_max"],
                enemyTableData.Rows[0]["boss_rate"],
                enemyTableData.Rows[0]["boss_drop_id"]
                });
            monsterData.Rows.Add(
               new object[]{
                "BOSS",//实际上应该是BOSS，不知道为什么表名是death
                enemyTableData.Rows[0]["death_set_id"],
                enemyTableData.Rows[0]["death_lv_min"],
                enemyTableData.Rows[0]["death_lv_max"],
                enemyTableData.Rows[0]["death_rate"],
                enemyTableData.Rows[0]["death_drop_id"]
                });
            foreach (DataRow dr in monsterData.Rows)
            {
                DataTable enemyUnitTable = db.GetData("SELECT * FROM enemy_unit_master WHERE id=" + dr["id"]);
                if (enemyUnitTable.Rows.Count == 0)
                {
                    continue;
                }
                DataRow enemyUnitRow = enemyUnitTable.Rows[0];
                dr["isDragon"] = enemyUnitRow["flag"];
                dr["name"] = enemyUnitRow["name"];
                dr["model"] = enemyUnitRow["model"];
                dr["texture"] = enemyUnitRow["texture"];
                dr["type"] = enemyUnitRow["type"];
                dr["attribute"] = enemyUnitRow["attribute"];
                dr["soul_pt"] = enemyUnitRow["soul_pt"];
                dr["gold_pt"] = enemyUnitRow["gold_pt"];
                dr["turn"] = enemyUnitRow["turn"];
                //dr["LvMinLife"] = RealCalc(Convert.ToInt32(enemyUnitRow["life"]), Convert.ToInt32(enemyUnitRow["up_life"]), Convert.ToInt32(dr["lv_min"]));
                dr["LvMaxLife"] = RealCalc(Convert.ToInt32(enemyUnitRow["life"]), Convert.ToInt32(enemyUnitRow["up_life"]), Convert.ToInt32(dr["lv_max"]));
                //dr["LvMinATK"] = RealCalc(Convert.ToInt32(enemyUnitRow["attack"]), Convert.ToInt32(enemyUnitRow["up_attack"]), Convert.ToInt32(dr["lv_min"]));
                dr["LvMaxATK"] = RealCalc(Convert.ToInt32(enemyUnitRow["attack"]), Convert.ToInt32(enemyUnitRow["up_attack"]), Convert.ToInt32(dr["lv_max"]));
                //dr["LvMinDEF"] = RealCalc(Convert.ToInt32(enemyUnitRow["defense"]), Convert.ToInt32(enemyUnitRow["up_defense"]), Convert.ToInt32(dr["lv_min"]));
                dr["LvMaxDEF"] = RealCalc(Convert.ToInt32(enemyUnitRow["defense"]), Convert.ToInt32(enemyUnitRow["up_defense"]), Convert.ToInt32(dr["lv_max"]));
            }

            return monsterData;
        }

        private void ReDrawMap(string levelID, DataTable monsterData)
        {
            if (MapGrid.Children.Count == 0)
            {
                return;
            }
            foreach (TextBox tb in MapGrid.Children)
            {
                string enemyNo = tb.Text;
                DataRow[] foundRow = monsterData.Select("[#] = '" + enemyNo + "'");
                if (foundRow.Length < 1)
                    continue;
                string enemyId = foundRow[0]["id"].ToString();
                switch (enemyId)
                {
                    case "65002":   //移動床_上
                        {
                            int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                            enemyDropId -= 99;
                            tb.Text = enemyDropId.ToString() + "↑";
                            break;
                        }
                    case "65003":   //移動床_直進
                        {
                            int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                            enemyDropId -= 299;
                            tb.Text = enemyDropId.ToString() + "→";
                            break;
                        }
                    case "65004":   //移動床_下
                        {
                            int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                            enemyDropId -= 199;
                            tb.Text = enemyDropId.ToString() + "↓";
                            break;
                        }
                    case "22000":   //宝箱
                        {
                            tb.Text = "箱";
                            tb.Foreground = Brushes.Red;
                            tb.FontWeight = FontWeights.Bold;
                            break;
                        }
                    case "40100":   //上り階段
                        {
                            int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                            //td.Text = "↗" + enemyDropId.ToString();
                            //tb.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↗</a>";
                            tb.Text = "↗";
                            break;
                        }
                    case "65001":   //下り階段
                        {
                            int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                            //td.Text = "↘" + enemyDropId.ToString();
                            //tb.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↘</a>";
                            tb.Text = "↘";
                            break;
                        }
                    default: break;
                }
            }
        }

        private static int RealCalc(int baseAttr, int up, int lv)
        {
            return (int)Math.Round(baseAttr * ((lv - 1) * (up * 0.01) + 1));
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(SQLTextBox.Text);
            });
            task.ContinueWith(t =>
            {
                CommonViewerDataGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }

        private void MainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (QuestViewerTabItem.IsSelected)
                {
                    QuestViewerDataGrid_BindData();
                }
                else if (MapViewerTabItem.IsSelected)
                {
                    if (!string.IsNullOrWhiteSpace(QuestInfo_id.Text))
                    {
                        InitMap(QuestInfo_id.Text);
                    }
                }
                else if (CommonViewerTabItem.IsSelected)
                {
                    Task<DataTable> task = new Task<DataTable>(() =>
                    {
                        DB db = new DB();
                        return db.GetData("SELECT * FROM USER_RANK_MASTER");
                    });
                    task.ContinueWith(t =>
                        {
                            CommonViewerDataGrid.ItemsSource = t.Result.DefaultView;
                        }, uiTaskScheduler);    //this Task work on ui thread
                    task.Start();
                }
            }
        }
        #region settings
        private void ImportMDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "MDBS File|MDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string xmlMDB = sr.ReadToEnd();
                    try
                    {
                        DataSet ds = XMLParser.ParseMDB(xmlMDB);
                        DB db = new DB();
                        db.ImportDataSet(ds, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
        private void ImportLDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "LDBS File|LDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string xmlLDB = sr.ReadToEnd();
                    try
                    {
                        DataSet ds = XMLParser.ParseMDB(xmlLDB);
                        DB db = new DB();
                        db.ImportDataSet(ds, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
        #endregion
    }
}
