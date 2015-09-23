﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using RTDDE.Executer.Util;
using RTDDE.Executer.Util.Map;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Map.xaml 的交互逻辑
    /// </summary>
    public partial class Map : UserControl
    {
        public Map()
        {
            InitializeComponent();
            if (Settings.Config.Map.IsShowDropInfo) {
                ShowDrop = true;
                MapShowDropToggleButton.IsChecked = true;
            }
        }
        private int Offset { get; set; }
        private MapTable CurrentMapTable { get; set; }
        private string CurrentLevelId { get; set; }
        private string CurrentQuestId { get; set; }
        private bool ShowDrop { get; set; }
        public void Load(string levelId = "-1",string dropId=null)
        {
            Unload();
            //add loading text
            MapGrid.Children.Add(new TextBlock() {
                Text = "Loading...",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right
            });
            if (string.IsNullOrEmpty(levelId)) {
                levelId = "-1";
            }
            if (string.IsNullOrEmpty(dropId)) {
                dropId = levelId;
            }
            CurrentLevelId = levelId;
            CurrentQuestId = dropId;
            Task<DataTable> initMonsterTask = new Task<DataTable>(GetMonsterData, levelId);
            initMonsterTask.ContinueWith(t =>
            {
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                MapMonsterGrid.ItemsSource = t.Result.DefaultView;
            }, MainWindow.UiTaskScheduler);

            var task = Task.Run(() =>
            {
                DataTable dt = DAL.GetDataTable("SELECT a.*,b.distance FROM level_data_master a left join quest_master b on a.level_data_id=b.id WHERE a.level_data_id=" + levelId);
                if (dt.Rows.Count == 0) {
                    //throw new Exception("NO MAP DATA.");
                    return;
                }
                DataRow levelData = dt.Rows[0];
                CurrentMapTable = new MapTable(levelData["map_data"].ToString(),
                Convert.ToInt32(levelData["width"]),
                Convert.ToInt32(levelData["height"]),
                Convert.ToInt32(levelData["start_x"]),
                Convert.ToInt32(levelData["start_y"]));
                CurrentMapTable.BindMonsterData(initMonsterTask.Result);
                if(ShowDrop) {
                    CurrentMapTable.BindDropData(MapData.GetEnemyInfo(dropId));
                }
            });
            task.ContinueWith(t =>
            {
                MapGrid.Children.Clear();
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                DrawMap();
            }, MainWindow.UiTaskScheduler);
            initMonsterTask.Start();
        }

        public void Unload()
        {
            MapMonsterGrid.ItemsSource = null;
            ClearMap();
            Offset = 0;
            CurrentMapTable = null;
        }

        private void ClearMap()
        {
            MapGrid.Children.Clear();
            MapGrid.ColumnDefinitions.Clear();
            MapGrid.RowDefinitions.Clear();
            MapMarkGrid.Children.Clear();
            MapMarkGrid.ColumnDefinitions.Clear();
            MapMarkGrid.RowDefinitions.Clear();
        }

        private void DrawMap() {
            if (CurrentMapTable == null) {
                MapGrid.Children.Add(new TextBlock()
                {
                    Text = "No map data, import GAME first.",
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Right
                });
                return;
            }
            //make offset to 0~CurrentMapTable.W, always positive
            int offsetRemain = (Offset%CurrentMapTable.W + CurrentMapTable.W)%CurrentMapTable.W;
            for (int row = 0; row < CurrentMapTable.Rows.Count; row++) {
                int offsetedRowIndex = (row + offsetRemain) %CurrentMapTable.W;
                MapRow r = CurrentMapTable[offsetedRowIndex];
                MapGrid.RowDefinitions.Add(new RowDefinition() {
                    Height = new GridLength(25)
                });
                for (int col = 0; col < r.Cells.Count; col++) {
                    MapCell c = r[col];
                    if (row == 0) {
                        MapGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                            Width = new GridLength(25)
                        });
                    }
                    TextBlock tb = new TextBlock() {
                        Text = c.Text,
                        Foreground = c.Foreground,
                        FontWeight = c.fontWeight
                    };
                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle() {
                        Fill = new LinearGradientBrush() {
                            StartPoint = new Point(0, 1),
                            EndPoint = new Point(1, 0),
                            MappingMode = BrushMappingMode.RelativeToBoundingBox,
                            GradientStops = new GradientStopCollection() {
                                new GradientStop(c.YorishiroColor, 1d/4d),
                                new GradientStop(Colors.Transparent, 1d/4d),
                                new GradientStop(Colors.Transparent, 3d/4d),
                                new GradientStop(c.AttributeColor, 3d/4d)
                            }
                        }
                    };
                    Border b = new Border() {
                        Background = c.Background,
                        Child = tb
                    };

                    MapGrid.Children.Add(rec);
                    rec.SetValue(Grid.RowProperty, row);
                    rec.SetValue(Grid.ColumnProperty, col);
                    rec.SetValue(Grid.ZIndexProperty, 2);

                    MapGrid.Children.Add(b);
                    b.SetValue(Grid.RowProperty, row);
                    b.SetValue(Grid.ColumnProperty, col);

                    //绘制tooltip
                    StringBuilder sb = new StringBuilder();
                    if (c.EnemyNo > 0) {
                        sb.AppendLine("#:" + c.EnemyNo);
                    }
                    if (c.HasDropInfo) {
                        sb.AppendLine("evo_pt:" + c.add_attribute_exp);
                        sb.AppendLine("exp:" + c.unit_exp);
                        if (c.drop_unit != null) {
                            sb.AppendLine(string.Format("drop:[{0}]{1}", c.drop_unit.g_id, c.drop_unit.name));
                        }
                    }
                    if (c.drop_id != 0) {
                        sb.AppendLine("drop_id:" + c.drop_id);
                        sb.AppendLine("gold:" + c.gold_pt);
                    }
                    if (string.IsNullOrWhiteSpace(sb.ToString().Trim()) == false) {
                        rec.ToolTip = new Run(sb.ToString().Trim());
                    }
                }
            }
            //绘制底部标记
            MapGrid.RowDefinitions.Add(new RowDefinition() {
                Height = new GridLength(25)
            });
            for (int j = 0; j < CurrentMapTable.H; j++) {
                TextBlock tb = new TextBlock() {
                    Text = (CurrentMapTable.ZeroMarkPlace - j).ToString()   //Now use BossStart as zero mark, farewell 3
                };
                Border b = new Border {Child = tb};
                MapGrid.Children.Add(b);

                b.SetValue(Grid.RowProperty, CurrentMapTable.W);
                b.SetValue(Grid.ColumnProperty, j);
            }
            //绘制左侧冻结行标记
            MapMarkGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(25)
            });
            for (int i = 0; i < CurrentMapTable.Rows.Count; i++) {
                MapMarkGrid.RowDefinitions.Add(new RowDefinition() {
                    Height = new GridLength(25)
                });
                TextBlock tb = new TextBlock() {
                    Text = CurrentMapTable.W-i > offsetRemain
                        ? (CurrentMapTable.Y - i - offsetRemain).ToString()
                        : ((CurrentMapTable.Y - i - offsetRemain + CurrentMapTable.W) % CurrentMapTable.W).ToString()
                };
                Border b = new Border {Child = tb};
                MapMarkGrid.Children.Add(b);

                b.SetValue(Grid.RowProperty, i);
                b.SetValue(Grid.ColumnProperty, 0);
            }
        }

        private DataTable GetMonsterData(object param) {
            string levelID = param.ToString();

            DataTable monsterData = new DataTable();
            monsterData.Columns.Add("#", typeof(string));
            monsterData.Columns.Add("id", typeof(int));
            monsterData.Columns.Add("lv_min", typeof(int));
            monsterData.Columns.Add("lv_max", typeof(int));
            monsterData.Columns.Add("rate", typeof(int));
            monsterData.Columns.Add("drop_id", typeof(int));
            monsterData.Columns.Add("bgm_id", typeof(int));

            QuestMaster qm = DAL.ToSingle<QuestMaster>("SELECT * FROM quest_master WHERE id=" + levelID);
            if (qm == null) {
                return monsterData;
            }
            DataTable enemyTableData =
                DAL.GetDataTable("SELECT * FROM enemy_table_master WHERE id=" + qm.enemy_table_id.ToString());
            if (enemyTableData.Rows.Count == 0) {
                //throw new Exception("NO ENEMY TABLE DATA.");
                return monsterData;
            }
            for (int i = 1; i <= 18; i++) {
                int setId = Convert.ToInt32(enemyTableData.Rows[0]["enemy" + i + "_set_id"]);
                if (setId != 0) {
                    monsterData.Rows.Add(
                        new object[] {
                            enemyTableData.Rows[0]["enemy" + i + "_id"],
                            setId,
                            enemyTableData.Rows[0]["enemy" + i + "_lv_min"],
                            enemyTableData.Rows[0]["enemy" + i + "_lv_max"],
                            enemyTableData.Rows[0]["enemy" + i + "_rate"],
                            enemyTableData.Rows[0]["enemy" + i + "_drop_id"]
                        });
                }
            }
            monsterData.Rows.Add(
                new object[] {
                    "DEATH", //实际上应该是DEATH，不知道为什么表名是boss
                    enemyTableData.Rows[0]["boss_set_id"],
                    enemyTableData.Rows[0]["boss_lv_min"],
                    enemyTableData.Rows[0]["boss_lv_max"],
                    enemyTableData.Rows[0]["boss_rate"],
                    enemyTableData.Rows[0]["boss_drop_id"]
                });
            monsterData.Rows.Add(
                new object[] {
                    "BOSS", //实际上应该是BOSS，不知道为什么表名是death
                    enemyTableData.Rows[0]["death_set_id"],
                    enemyTableData.Rows[0]["death_lv_min"],
                    enemyTableData.Rows[0]["death_lv_max"],
                    enemyTableData.Rows[0]["death_rate"],
                    enemyTableData.Rows[0]["death_drop_id"]
                });
            object boss01SetId = enemyTableData.Rows[0]["boss01_set_id"];
            if (boss01SetId != DBNull.Value && Convert.ToInt32(boss01SetId) != 0) {
                monsterData.Rows.Add(
                    new object[] {
                        "BOSS01",
                        boss01SetId,
                        enemyTableData.Rows[0]["boss01_lv_min"],
                        enemyTableData.Rows[0]["boss01_lv_max"],
                        enemyTableData.Rows[0]["boss01_rate"],
                        enemyTableData.Rows[0]["boss01_drop_id"],
                        enemyTableData.Rows[0]["boss01_bgm_id"]
                    });
            }
            object boss02SetId = enemyTableData.Rows[0]["boss02_set_id"];
            if (boss02SetId != DBNull.Value && Convert.ToInt32(boss02SetId) != 0) {
                monsterData.Rows.Add(
                    new object[] {
                        "BOSS02",
                        boss02SetId,
                        enemyTableData.Rows[0]["boss02_lv_min"],
                        enemyTableData.Rows[0]["boss02_lv_max"],
                        enemyTableData.Rows[0]["boss02_rate"],
                        enemyTableData.Rows[0]["boss02_drop_id"],
                        enemyTableData.Rows[0]["boss02_bgm_id"]
                    });
            }
            return monsterData;
        }

        private void MapMonsterGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapMonsterGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            DataRow mmInfoRow = ((DataRowView)MapMonsterGrid.SelectedItem).Row;
            MapEnemyInfo_Mark.Text = mmInfoRow["#"].ToString();
            MapEnemyInfo_id.Text = mmInfoRow["id"].ToString();
            MapEnemyInfo_rate.Text = mmInfoRow["rate"].ToString();
            MapEnemyInfo_lv_min.Text = mmInfoRow["lv_min"].ToString();
            MapEnemyInfo_lv_max.Text = mmInfoRow["lv_max"].ToString();
            MapEnemyInfo_drop_id.Text = mmInfoRow["drop_id"].ToString();
            string bgmId = mmInfoRow["bgm_id"].ToString();
            if (string.IsNullOrEmpty(bgmId) == false) {
                MapEnemyInfo_bgm.Text = Utility.ParseBgmName(Convert.ToInt32(bgmId));
            }

            if (Settings.Config.General.IsDefaultLvMax) {
                MapEnemyInfo_lv.Text = "99";
            }
            else {
                MapEnemyInfo_lv.Text = "1";
            }
            MapEnemyInfo_DataBind();
        }

        private void MapEnemyInfo_lv_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MapEnemyInfo_lv.IsFocused) {
                if (string.IsNullOrWhiteSpace(MapEnemyInfo_lv.Text)) {
                    MapEnemyInfo_lv.Text = "";
                }
                Regex r = new Regex("[^0-9]");
                if (r.Match(MapEnemyInfo_lv.Text).Success) {
                    if (Settings.Config.General.IsDefaultLvMax) {
                        MapEnemyInfo_lv.Text = "99";
                        return;
                    }
                    else {
                        MapEnemyInfo_lv.Text = "1";
                        return;
                    }
                }
                MapEnemyInfo_DataBind();
            }
        }

        private void MapEnemyInfo_DataBind()
        {
            if (string.IsNullOrWhiteSpace(MapEnemyInfo_id.Text)) {
                return;
            }
            string enemyId = MapEnemyInfo_id.Text;
            Task<EnemyUnitMaster> task = new Task<EnemyUnitMaster>(() =>
            {
                string sql = "SELECT * FROM enemy_unit_master WHERE id={0}";
                return DAL.ToSingle<EnemyUnitMaster>(String.Format(sql, enemyId));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (t.Result == null) {
                    return;
                }
                EnemyUnitMaster eum = t.Result;

                MapEnemyInfo_name.Text = eum.name;
                MapEnemyInfo_model.Text = eum.model;
                MapEnemyInfo_texture.Text = eum.texture;
                MapEnemyInfo_type.Text = Utility.ParseEnemyType(eum.type);
                MapEnemyInfo_isDragon.Text = Convert.ToBoolean(eum.flag).ToString();
                MapEnemyInfo_isUnitEnemy.Text = Utility.IsUnitEnemy(eum.type).ToString();
                MapEnemyInfo_attribute.Text = Utility.ParseAttributetype(eum.attribute);
                MapEnemyInfo_soul_pt.Text = eum.soul_pt.ToString();
                MapEnemyInfo_gold_pt.Text = eum.gold_pt.ToString();
                MapEnemyInfo_turn.Text = eum.turn.ToString();

                int lv = Convert.ToInt32(MapEnemyInfo_lv.Text);
                int lv_max = Convert.ToInt32(MapEnemyInfo_lv_max.Text);
                if (Settings.Config.General.IsEnableLevelLimiter && (lv > lv_max)) {
                    lv = lv_max;
                    MapEnemyInfo_lv.Text = lv.ToString("0");
                }
                MapEnemyInfo_life.Text = Utility.RealCalc(eum.life, eum.up_life, lv).ToString();
                int atk = Utility.RealCalc(eum.attack, eum.up_attack, lv);
                MapEnemyInfo_atk.Text = atk.ToString();
                MapEnemyInfo_atk.ToolTip = $"{(int) (atk*0.85)}~{(int) (atk*1.1)}";
                MapEnemyInfo_def.Text = Utility.RealCalc(eum.defense, eum.up_defense, lv).ToString();

                MapEnemyInfo_pat.Text = Utility.ParseAttackPattern(eum.pat);
                MapEnemyInfo_p0.Text = eum.p0.ToString();
                MapEnemyInfo_p1.Text = eum.p1.ToString();
                MapEnemyInfo_pat_01.Text = Utility.ParseAttackPattern(eum.pat_01);
                MapEnemyInfo_p0_01.Text = eum.p0_01.ToString();
                MapEnemyInfo_p1_01.Text = eum.p1_01.ToString();

            }, MainWindow.UiTaskScheduler);
            task.Start();
        }

        private void MapOffsetUpButton_OnClick(object sender, RoutedEventArgs e) {
            Offset++;
            ClearMap();
            DrawMap();
        }

        private void MapOffsetDownButton_OnClick(object sender, RoutedEventArgs e) {
            Offset--;
            ClearMap();
            DrawMap();
        }

        private void ImportLdbsButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = Utility.GetUiText("Config_LDBSFilter") + "|LDBS*_Msg.bytes";
            if (ofd.ShowDialog() == true) {
                ImportLdbsButton.SetResourceReference(Button.ContentProperty, "Config_ImportingLDBS");
                string filename = ofd.FileName;
                Task task = Task.Run(() => {
                    using (StreamReader sr = new StreamReader(filename)) {
                        var game = new MapData(sr.BaseStream);
                        DAL.FromSingle(game.LDM);
                        DAL.FromSingle(game.LDM.enemy_table_master);
                        DAL.FromSingle(game.LDM.unit_talk_master);
                        //foreach (var ecm in game.LDM.event_cutin_master)
                        //{
                        //    DAL.FromSingle(ecm);
                        //}
                    }
                });
                task.ContinueWith(t => {
                    if (t.Exception != null) {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportLdbsButton.SetResourceReference(Button.ContentProperty, "Config_ImportLDBSFail");
                    }
                    else {
                        ImportLdbsButton.SetResourceReference(Button.ContentProperty, "Config_ImportLDBSSuccess");
                        Load(CurrentQuestId);
                    }
                }, MainWindow.UiTaskScheduler);
            }
        }

        private void MapLoadSimilarButton_OnClick(object sender, RoutedEventArgs e) {
            string SimilarQuestId = DAL.Get<string>($@"select level_data_id from level_data_master 
left join quest_master on level_data_master.level_data_id==quest_master.id 
where quest_master.name=(select name from quest_master where id = {CurrentQuestId})");
            Load(SimilarQuestId, CurrentQuestId);
        }

        private void MapShowDropToggleButton_OnClick(object sender, RoutedEventArgs e) {
            ShowDrop = MapShowDropToggleButton.IsChecked.GetValueOrDefault();
            Load(CurrentLevelId, CurrentQuestId);
        }
    }
}
