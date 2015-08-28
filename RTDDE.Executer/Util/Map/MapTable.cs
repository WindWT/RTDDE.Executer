using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public MapTable(string mapData, int w, int h, int x, int y, int repeat = 1)
        {
            Rows = new List<MapRow>();
            MapData = mapData;
            W = w;
            H = h;
            X = x;
            Y = y;
            Repeat = repeat;
            //freeze all brush
            FireTransBrush.Freeze();
            WaterTransBrush.Freeze();
            LightTransBrush.Freeze();
            DarkTransBrush.Freeze();
        }
        public void InitMap()
        {
            int zeroMarkPlace = -1;
            for (int r = 0; r < Repeat; r++) {
                for (int i = 0; i < W; i++) {
                    var mapRow = new MapRow();
                    for (int j = 0; j < H; j++) {
                        var mapCell = new MapCell();

                        string cellData = MapData.Substring((j * W + i) * 2, 2);
                        int cellDataInt = int.Parse(cellData, System.Globalization.NumberStyles.HexNumber);
                        mapCell.RawCellData = cellDataInt;
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
                                        if (zeroMarkPlace == -1) {
                                            zeroMarkPlace = j;
                                        }
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
                var mapCellMark = new MapCell((zeroMarkPlace - j).ToString());  //Now use BossStart as zero mark, farewell 3
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
                    c.gold_pt = eum.gold_pt;
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
                        case ENEMY_TYPE.EVENT_EXP_N:
                        case ENEMY_TYPE.EVENT_EXP_SP:
                            {
                                c.Foreground = Brushes.Gold;
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
            EnemyInfo bossInfo = dropData.Find(o => o.enemy_id == 0);
            EnemyInfo boss01Info = dropData.Find(o => o.enemy_id == 100);
            EnemyInfo boss02Info = dropData.Find(o => o.enemy_id == 101);
            EnemyInfo deathInfo = dropData.Find(o => o.enemy_id == 99);
            foreach (MapRow r in this.Rows) {
                foreach (MapCell c in r.Cells) {
                    EnemyInfo ei = dropData.Find(o => o.x == c.x && o.y == c.y && !(o.x == 0 && o.y == 0));
                    if (c.RawCellData == 192) {
                        switch (c.x) {
                            case 0: {
                                c.drop_unit =
                                    DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" +
                                                             bossInfo.drop_unit_id.ToString());
                                c.add_attribute_exp = bossInfo.add_attribute_exp;
                                c.unit_exp = bossInfo.unit_exp;
                                c.HasDropInfo = true;
                                break;
                            }
                            case 1: {
                                if (boss01Info == null) {
                                    continue;
                                }
                                c.drop_unit =
                                    DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" +
                                                             boss01Info.drop_unit_id.ToString());
                                c.add_attribute_exp = boss01Info.add_attribute_exp;
                                c.unit_exp = boss01Info.unit_exp;
                                c.HasDropInfo = true;
                                break;
                            }
                            case 2: {
                                if (boss02Info == null) {
                                    continue;
                                }
                                c.drop_unit =
                                    DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" +
                                                             boss02Info.drop_unit_id.ToString());
                                c.add_attribute_exp = boss02Info.add_attribute_exp;
                                c.unit_exp = boss02Info.unit_exp;
                                c.HasDropInfo = true;
                                break;
                            }
                            default:
                                break;
                        }
                    }
                    else if (c.RawCellData == 160) {
                        c.drop_unit =
                            DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" +
                                                     deathInfo.drop_unit_id.ToString());
                        c.add_attribute_exp = deathInfo.add_attribute_exp;
                        c.unit_exp = deathInfo.unit_exp;
                        c.HasDropInfo = true;
                    }
                    else if (ei != null && ei.flag) {
                        c.drop_unit =
                            DAL.ToSingle<UnitMaster>("SELECT * FROM UNIT_MASTER WHERE id=" + ei.drop_unit_id.ToString());
                        c.add_attribute_exp = ei.add_attribute_exp;
                        c.unit_exp = ei.unit_exp;
                        c.HasDropInfo = true;
                    }
                    else if (string.IsNullOrEmpty(c.CellData) == false && c.CellData.StartsWith("E")) {
                        c.Foreground = Brushes.LightGray;
                    }
                    //set drop mark
                    if (c.drop_unit == null) {
                        continue;   //no enemy, no need to set drop mark
                    }
                    if (c.drop_unit.mix >= Settings.Config.Map.ExpValue) {
                        //exp
                        c.YorishiroColor = Settings.Config.Map.ExpColor;
                    }
                    else if (c.drop_unit.set_pt >= Settings.Config.Map.PtValue) {
                        //pt
                        c.YorishiroColor = Settings.Config.Map.PtColor;
                    }
                    else if (c.drop_unit.sale >= Settings.Config.Map.SaleValue) {
                        //sale
                        c.YorishiroColor = Settings.Config.Map.SaleColor;
                    }
                    var customColors = Settings.Config.Map.CustomDropColors;
                    if (customColors != null && customColors.ContainsKey(c.drop_unit.id)) {
                        c.YorishiroColor = customColors[c.drop_unit.id];
                    }
                }
            }
        }
    }
}
