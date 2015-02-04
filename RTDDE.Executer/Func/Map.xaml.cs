﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Map.xaml 的交互逻辑
    /// </summary>
    public partial class Map : UserControl, IRefreshable
    {
        public Map()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            Unload();
        }
        public void Load(string levelID = "-1", int repeat = 1)
        {
            if (string.IsNullOrEmpty(levelID)) {
                levelID = "-1";
            }
            Task<DataTable> initMonsterTask = new Task<DataTable>(GetMonsterData, levelID);
            initMonsterTask.ContinueWith(t =>
            {
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                MapMonsterGrid.ItemsSource = t.Result.DefaultView;
            }, MainWindow.UiTaskScheduler);

            Task<MapTable> task = new Task<MapTable>(() =>
            {
                DataTable dt = DAL.GetDataTable("SELECT a.*,b.distance FROM level_data_master a left join quest_master b on a.level_data_id=b.id WHERE a.level_data_id=" + levelID);
                if (dt.Rows.Count == 0) {
                    //throw new Exception("NO MAP DATA.");
                    return null;
                }
                DataRow levelData = dt.Rows[0];
                return BindDropDataToMap(BindMonsterDataToMap(InitMapData(
                levelData["map_data"].ToString(),
                Convert.ToInt32(levelData["width"]),
                Convert.ToInt32(levelData["height"]),
                Convert.ToInt32(levelData["start_x"]),
                Convert.ToInt32(levelData["start_y"]),
                Convert.ToInt32(levelData["distance"]),
                repeat
                ), initMonsterTask.Result), Settings.Config.General.IsShowDropInfo ? MapData.GetEnemyInfo(levelID) : null);
            }
            );
            task.ContinueWith(t =>
            {
                ClearMap();
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                else if (t.Result == null) {
                    MapGrid.Children.Add(new TextBlock()
                    {
                        Text = "No map data, import GAME first.",
                        FontWeight = FontWeights.Bold,
                        Padding = new Thickness(24)
                    });
                }
                else {
                    DrawMap(t.Result);
                }
            }, MainWindow.UiTaskScheduler);
            task.Start();
            initMonsterTask.Start();
        }

        public void Unload()
        {
            MapMonsterGrid.ItemsSource = null;
            ClearMap();
        }

        /*按列生成地图
        private void DrawMap(string mapData, int w, int h, int x, int y, int distance, int repeat)
        {
            if (repeat < 1)
            {
                repeat = 1;
            }
            var mapTable = new MapTable();
            //循环生成列
            //-1列用于序号
            for (int j = -1; j < h; j++)
            {
                var mapColumn = new MapColumn();
                for (int i = 0; i < (w * repeat) + 1; i++)
                {
                    var mapCell = new MapCell();
                    if (j == -1)    //序号列
                    {
                        mapCell.CellData = ((y - i) % w).ToString();
                    }
                    else if (i % w == y && j == x)  //起点
                    {
                        mapCell.CellData = "★";
                    }
                    else if (i == w * repeat)   //序号行
                    {
                        mapCell.CellData = (distance - j + 3).ToString();    //magic number 3!
                    }
                    else
                    {
                        string cellData = mapData.Substring((j * w + i) * 2, 2);
                        int cellDataInt = int.Parse(cellData, System.Globalization.NumberStyles.HexNumber);
                        int num = 7 & cellDataInt >> 5;
                        if (num > 0)
                        {
                            switch (1 << (num - 1))
                            {
                                //AttributeTypeLight,
                                case 1:
                                    {
                                        mapCell.Background = Brushes.Yellow;
                                        break;
                                    }
                                //AttributeTypeDark,
                                case 2:
                                    {
                                        mapCell.Background = Brushes.Purple;
                                        mapCell.Foreground = Brushes.White;
                                        break;
                                    }
                                //AttributeTypeFire = 4,
                                case 4:
                                    {
                                        mapCell.Background = Brushes.Pink;
                                        break;
                                    }
                                //AttributeTypeWater = 8,
                                case 8:
                                    {
                                        mapCell.Background = Brushes.Aqua;
                                        break;
                                    }
                                //AttributeTypeBossStart = 16,
                                case 16:
                                    {
                                        mapCell.Background = Brushes.Silver;
                                        break;
                                    }
                                //AttributeTypeBoss = 32,
                                case 32:
                                    {
                                        mapCell.Background = Brushes.Black;
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
                                mapCell.Foreground = Brushes.Red;
                                mapCell.fontWeight = FontWeights.Bold;
                            }
                        }
                        if ((cellDataInt == 0) || (num2 == 0))
                        {
                            cellData = "";
                        }
                        mapCell.CellData = cellData;
                    }
                    mapColumn.MapCells.Add(mapCell);
                }
                mapTable.MapColumns.Add(mapColumn);
            }
            MapDataGrid.DataContext = mapTable;
        }*/

        #region brush
        //private static readonly Brush FireTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.9f, 0.4f, 0.3f));
        //private static readonly Brush WaterTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.4f, 0.89f, 0.9f));
        //private static readonly Brush LightTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.9f, 0.9f, 0.3f));
        //private static readonly Brush DarkTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.76f, 0.58f, 0.9f));
        private static readonly Brush FireTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.9f, 0.4f, 0.3f));
        private static readonly Brush WaterTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.4f, 0.89f, 0.9f));
        private static readonly Brush LightTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.9f, 0.9f, 0.3f));
        private static readonly Brush DarkTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.76f, 0.58f, 0.9f));
        private static readonly Color FireColor = Colors.Tomato;
        private static readonly Color WaterColor = Colors.DodgerBlue;
        private static readonly Color LightColor = Colors.Orange;
        private static readonly Color DarkColor = Colors.Purple;
        #endregion
        private MapTable InitMapData(string mapData, int w, int h, int x, int y, int distance, int repeat)
        {
            if (repeat < 1) {
                repeat = 1;
            }
            var mapTable = new MapTable();
            mapTable.x = x;
            mapTable.y = y;
            mapTable.h = h;
            mapTable.w = w;
            mapTable.repeat = repeat;

            for (int r = 0; r < repeat; r++) {
                for (int i = 0; i < w; i++) {
                    var mapRow = new MapRow();
                    for (int j = 0; j < h; j++) {
                        var mapCell = new MapCell();

                        string cellData = mapData.Substring((j * w + i) * 2, 2);
                        int cellDataInt = int.Parse(cellData, System.Globalization.NumberStyles.HexNumber);
                        int num = 7 & cellDataInt >> 5;
                        if (num > 0) {
                            switch (1 << (num - 1)) {
                                //AttributeTypeLight,
                                case 1: {
                                        mapCell.Background = LightTransBrush;
                                        break;
                                    }
                                //AttributeTypeDark,
                                case 2: {
                                        mapCell.Background = DarkTransBrush;
                                        break;
                                    }
                                //AttributeTypeFire = 4,
                                case 4: {
                                        mapCell.Background = FireTransBrush;
                                        break;
                                    }
                                //AttributeTypeWater = 8,
                                case 8: {
                                        mapCell.Background = WaterTransBrush;
                                        break;
                                    }
                                //AttributeTypeBossStart = 16,
                                case 16: {
                                        mapCell.Background = Brushes.Silver;
                                        break;
                                    }
                                //AttributeTypeBoss = 32,
                                case 32: {
                                        mapCell.Background = Brushes.Black;
                                        break;
                                    }
                            }
                        }

                        var num2 = 31 & cellDataInt;
                        if (num2 >= 24) {
                            if (Settings.Config.General.IsShowBoxInfo) {
                                switch (num2 - 23) {
                                    case 1: cellData = "?"; break;
                                    case 2: cellData = "?$"; break;
                                    case 3: cellData = "♥"; break;
                                    case 4: cellData = "魂"; break;
                                    case 5: cellData = "+1"; break;
                                    case 6: cellData = "$"; break;
                                    default: cellData = "箱" + (num2 - 23).ToString(); break;
                                }
                                mapCell.fontWeight = FontWeights.Bold;
                            }
                            else {
                                cellData = "箱";
                            }
                        }
                        else {
                            //this.EnemyUnitID = num2;
                            //this.TreasureID = 0;
                            cellData = "E" + (num2);
                            if (num2 != 0) {
                                //td.Attributes.Add("enemy", cellData);
                            }
                        }
                        if ((cellDataInt == 0) || (num2 == 0)) {
                            cellData = "";
                        }
                        if (i == y && j == x) {
                            cellData = "★";
                        }

                        mapCell.CellData = cellData;
                        mapCell.x = i;
                        mapCell.y = j;
                        mapRow.Cells.Add(mapCell);
                    }
                    mapTable.Rows.Add(mapRow);
                }
            }

            //添加底部标记
            var mapMarkRow = new MapRow();
            for (int j = 0; j < h; j++) {
                var mapCellMark = new MapCell((distance - j + 3).ToString());    //magic number 3!
                mapMarkRow.Cells.Add(mapCellMark);
            }
            mapTable.Rows.Add(mapMarkRow);

            return mapTable;
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

        private void DrawMap(MapTable map)
        {
            bool isFirstColDef = false;
            int row = 0;
            foreach (MapRow r in map.Rows) {
                int col = 0;
                MapGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(25)
                });
                foreach (MapCell c in r.Cells) {
                    if (!isFirstColDef) {
                        MapGrid.ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Width = new GridLength(25)
                        });
                    }
                    TextBlock tb = new TextBlock()
                    {
                        Text = c.CellData,
                        Foreground = c.Foreground,
                        FontWeight = c.fontWeight
                    };
                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle()
                    {
                        Fill = new LinearGradientBrush()
                        {
                            StartPoint = new Point(0, 1),
                            EndPoint = new Point(1, 0),
                            MappingMode = BrushMappingMode.RelativeToBoundingBox,
                            GradientStops = new GradientStopCollection()
                            {
                                new GradientStop(c.YorishiroColor,1d/4d),
                                new GradientStop(Colors.Transparent,1d/4d),
                                new GradientStop(Colors.Transparent,3d/4d),
                                new GradientStop(c.AttributeColor,3d/4d)
                            }
                        }
                    };
                    Border b = new Border()
                    {
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

                    StringBuilder sb = new StringBuilder();
                    if (c.HasDropInfo) {
                        sb.AppendLine("觉醒pt:" + c.add_attribute_exp);
                        sb.AppendLine("exp:" + c.unit_exp);
                        if (c.drop_unit != null) {
                            sb.AppendLine(string.Format("掉落:[{0}]{1}", c.drop_unit.g_id, c.drop_unit.name));
                        }
                    }
                    if (c.drop_id != 0) {
                        sb.AppendLine("掉落表id:" + c.drop_id);
                    }
                    if (string.IsNullOrWhiteSpace(sb.ToString().Trim()) == false) {
                        rec.ToolTip = new Run(sb.ToString().Trim());
                    }
                    col++;
                }
                isFirstColDef = true;
                row++;
            }
            //绘制冻结行标记
            MapMarkGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(25)
            });
            for (int i = 0; i < row; i++) {
                MapMarkGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(25)
                });
                TextBlock tb = new TextBlock()
                {
                    Text = (i == (row - 1)) ? string.Empty : ((map.y - i) % map.h).ToString()
                };
                Border b = new Border();
                b.Child = tb;
                MapMarkGrid.Children.Add(b);

                b.SetValue(Grid.RowProperty, i);
                b.SetValue(Grid.ColumnProperty, 0);
            }
        }

        private DataTable GetMonsterData(object param)
        {
            string levelID = param.ToString();

            DataTable monsterData = new DataTable();
            monsterData.Columns.Add("#", typeof(string));
            monsterData.Columns.Add("id", typeof(int));
            monsterData.Columns.Add("lv_min", typeof(int));
            monsterData.Columns.Add("lv_max", typeof(int));
            monsterData.Columns.Add("rate", typeof(int));
            monsterData.Columns.Add("drop_id", typeof(int));

            QuestMaster qm = DAL.ToSingle<QuestMaster>("SELECT * FROM quest_master WHERE id=" + levelID);
            if (qm == null) {
                return monsterData;
            }
            DataTable enemyTableData = DAL.GetDataTable("SELECT * FROM enemy_table_master WHERE id=" + qm.enemy_table_id.ToString());
            if (enemyTableData.Rows.Count == 0) {
                //throw new Exception("NO ENEMY TABLE DATA.");
                return monsterData;
            }
            for (int i = 1; i <= 18; i++) {
                if (enemyTableData.Rows[0]["enemy" + i + "_set_id"].ToString() != "0") {
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
                MapEnemyInfo_atk.Text = Utility.RealCalc(eum.attack, eum.up_attack, lv).ToString();
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

        private MapTable BindMonsterDataToMap(MapTable map, DataTable monsterData)
        {
            foreach (MapRow r in map.Rows) {
                foreach (MapCell c in r.Cells) {
                    string enemyNo = c.CellData;
                    DataRow[] foundRow = monsterData.Select("[#] = '" + enemyNo + "'");
                    if (foundRow.Length < 1) {
                        continue;
                    }
                    c.drop_id = Convert.ToInt32(foundRow[0]["drop_id"]);
                    string enemyId = foundRow[0]["id"].ToString();
                    EnemyUnitMaster eum =
                        DAL.ToSingle<EnemyUnitMaster>("SELECT * FROM ENEMY_UNIT_MASTER WHERE id=" + enemyId);
                    switch (Utility.ParseAttributetype(eum.attribute))
                    {
                        case "FIRE":
                        {
                            c.AttributeColor = FireColor;
                            break;
                        }
                        case "WATER":
                        {
                            c.AttributeColor = WaterColor;
                            break;
                        }
                        case "LIGHT":
                        {
                            c.AttributeColor = LightColor;
                            break;
                        }
                        case "DARK":
                        {
                            c.AttributeColor = DarkColor;
                            break;
                        }
                    }
                    switch ((ENEMY_TYPE)eum.type) {
                        case ENEMY_TYPE.STAIRS: {
                                switch (eum.id) {
                                    case 40100: //上り階段
                                {
                                            int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                            //td.Text = "↗" + enemyDropId.ToString();
                                            //tb.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↗</a>";
                                            c.CellData = "↗";
                                            break;
                                        }
                                    case 65001: //下り階段
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    //td.Text = "↘" + enemyDropId.ToString();
                                    //tb.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↘</a>";
                                    c.CellData = "↘";
                                    break;
                                }
                                }
                                break;
                            }
                        case ENEMY_TYPE.MOVING_PANEL: {
                                switch (eum.id) {
                                    case 65002: //移動床_上
                                {
                                            int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                            enemyDropId -= 99;
                                            c.CellData = enemyDropId.ToString() + "↑";
                                            break;
                                        }
                                    case 65003: //移動床_直進
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    enemyDropId -= 299;
                                    c.CellData = enemyDropId.ToString() + "→";
                                    break;
                                }
                                    case 65004: //移動床_下
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    enemyDropId -= 199;
                                    c.CellData = enemyDropId.ToString() + "↓";
                                    break;
                                }
                                }
                                break;
                            }
                        case ENEMY_TYPE.TREASURE: {
                                c.CellData = "箱";
                                c.Foreground = Brushes.Red;
                                c.fontWeight = FontWeights.Bold;
                                break;
                            }
                        case ENEMY_TYPE.SILVER_SLIME: {
                                c.CellData = c.CellData.Replace("E", "S");
                                c.Foreground = Brushes.Silver;
                                c.fontWeight = FontWeights.Bold;
                                break;
                            }
                        case ENEMY_TYPE.GOLD_SLIME: {
                                c.CellData = c.CellData.Replace("E", "G");
                                c.Foreground = Brushes.Gold;
                                c.fontWeight = FontWeights.Bold;
                                break;
                            }
                        case ENEMY_TYPE.MINE: {
                                c.CellData = "✹";
                                c.Foreground = Brushes.Gray;
                                break;
                            }
                        case ENEMY_TYPE.GOLD: {
                                c.CellData = "$";
                                c.Foreground = Brushes.Gray;
                                break;
                            }
                        case ENEMY_TYPE.NOTHING: {
                                c.CellData = string.Empty;
                                c.Background = Brushes.Black;
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            return map;
        }

        private MapTable BindDropDataToMap(MapTable map, List<EnemyInfo> DropData)
        {
            if (DropData == null || DropData.Count == 0) {
                return map;
            }
            foreach (MapRow r in map.Rows) {
                foreach (MapCell c in r.Cells) {
                    EnemyInfo ei = DropData.Find(o => o.x == c.x && o.y == c.y && !(o.x == 0 && o.y == 0));
                    if (ei != null && ei.flag) {
                        c.HasDropInfo = true;
                        c.drop_unit =
                            DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" + ei.drop_unit_id.ToString());
                        c.add_attribute_exp = ei.add_attribute_exp;
                        c.unit_exp = ei.unit_exp;
                        if (c.drop_unit == null) {
                            continue;
                        }
                        //if (c.drop_unit.kind != 311) {   //not yorishiro, skip
                        //    continue;
                        //}
                        if (c.drop_unit.id == 15022) {
                            //very bad yorishiro
                            c.YorishiroColor = Colors.Black;
                        }
                        else if (c.drop_unit.mix > 30000) {
                            //many exp
                            c.YorishiroColor = Color.FromRgb(98, 255, 98);
                        }
                        else if (c.drop_unit.mix >= 25000) {
                            //exp
                            c.YorishiroColor = Color.FromRgb(255, 98, 98);
                        }
                        else if (c.drop_unit.set_pt >= 250) {
                            //pt
                            c.YorishiroColor = Color.FromRgb(98, 98, 255);
                        }
                        else if (c.drop_unit.sale >= 20000) {
                            //sale
                            c.YorishiroColor = Colors.Silver;
                        }
                    }
                    else {
                        if (string.IsNullOrEmpty(c.CellData) == false && c.CellData.StartsWith("E")) {
                            c.Foreground = Brushes.LightGray;
                        }
                    }
                }
            }
            MapCell lastCell = map.Rows.Find(o => { return true; }).Cells.FindLast(o => { return true; });
            EnemyInfo bossInfo = DropData.Find(o => { return o.enemy_id == 0; });
            lastCell.drop_unit = DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" + bossInfo.drop_unit_id.ToString());
            lastCell.add_attribute_exp = bossInfo.add_attribute_exp;
            lastCell.unit_exp = bossInfo.unit_exp;
            lastCell.HasDropInfo = true;
            return map;
        }
    }
}
