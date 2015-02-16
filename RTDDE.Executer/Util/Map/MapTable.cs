using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Media;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Util.Map
{
    public class MapTable
    {
        public List<MapRow> Rows { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int H { get; private set; }
        public int W { get; private set; }
        public int Distance { get; private set; }
        public string MapData { get; private set; }
        public int Repeat { get; private set; }

        #region brush
        private static readonly Brush FireTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.9f, 0.4f, 0.3f));
        private static readonly Brush WaterTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.4f, 0.89f, 0.9f));
        private static readonly Brush LightTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.9f, 0.9f, 0.3f));
        private static readonly Brush DarkTransBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 0.76f, 0.58f, 0.9f));
        private static readonly Color FireColor = Colors.Tomato;
        private static readonly Color WaterColor = Colors.DodgerBlue;
        private static readonly Color LightColor = Colors.Orange;
        private static readonly Color DarkColor = Colors.Purple;
        #endregion

        public MapTable(string mapData, int w, int h, int x, int y, int distance, int repeat = 1)
        {
            Rows = new List<MapRow>();
            MapData = mapData;
            W = w;
            H = h;
            X = x;
            Y = y;
            Distance = distance;
            Repeat = repeat;
            //freeze all brush
            FireTransBrush.Freeze();
            WaterTransBrush.Freeze();
            LightTransBrush.Freeze();
            DarkTransBrush.Freeze();
        }
        public void InitMap()
        {
            for (int r = 0; r < Repeat; r++) {
                for (int i = 0; i < W; i++) {
                    var mapRow = new MapRow();
                    for (int j = 0; j < H; j++) {
                        var mapCell = new MapCell();

                        string cellData = MapData.Substring((j * W + i) * 2, 2);
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
                            if (Settings.Config.Map.IsShowBoxInfo) {
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
                        if (i == Y && j == X) {
                            cellData = "★";
                        }

                        mapCell.CellData = cellData;
                        mapCell.x = i;
                        mapCell.y = j;
                        mapRow.Cells.Add(mapCell);
                    }
                    this.Rows.Add(mapRow);
                }
            }

            //添加底部标记
            var mapMarkRow = new MapRow();
            for (int j = 0; j < H; j++) {
                var mapCellMark = new MapCell((Distance - j + 3).ToString());    //magic number 3!
                mapMarkRow.Cells.Add(mapCellMark);
            }
            this.Rows.Add(mapMarkRow);
        }
        public void BindMonsterData(DataTable monsterTable)
        {
            foreach (MapRow r in this.Rows) {
                foreach (MapCell c in r.Cells) {
                    string enemyNo = c.CellData;
                    DataRow[] foundRow = monsterTable.Select("[#] = '" + enemyNo + "'");
                    if (foundRow.Length < 1) {
                        continue;
                    }
                    c.drop_id = Convert.ToInt32(foundRow[0]["drop_id"]);
                    string enemyId = foundRow[0]["id"].ToString();
                    EnemyUnitMaster eum =
                        DAL.ToSingle<EnemyUnitMaster>("SELECT * FROM ENEMY_UNIT_MASTER WHERE id=" + enemyId);
                    if (Settings.Config.Map.IsShowEnemyAttribute) {
                        switch (Utility.ParseAttributetype(eum.attribute)) {
                            case "FIRE": {
                                    c.AttributeColor = FireColor;
                                    break;
                                }
                            case "WATER": {
                                    c.AttributeColor = WaterColor;
                                    break;
                                }
                            case "LIGHT": {
                                    c.AttributeColor = LightColor;
                                    break;
                                }
                            case "DARK": {
                                    c.AttributeColor = DarkColor;
                                    break;
                                }
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
        }
        public void BindDropData(List<EnemyInfo> dropData)
        {
            if (dropData == null || dropData.Count == 0) {
                return;
            }
            foreach (MapRow r in this.Rows) {
                foreach (MapCell c in r.Cells) {
                    EnemyInfo ei = dropData.Find(o => o.x == c.x && o.y == c.y && !(o.x == 0 && o.y == 0));
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
            MapCell lastCell = this.Rows.Find(o => { return true; }).Cells.FindLast(o => { return true; });
            EnemyInfo bossInfo = dropData.Find(o => { return o.enemy_id == 0; });
            lastCell.drop_unit = DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" + bossInfo.drop_unit_id.ToString());
            lastCell.add_attribute_exp = bossInfo.add_attribute_exp;
            lastCell.unit_exp = bossInfo.unit_exp;
            lastCell.HasDropInfo = true;
        }
    }
}
