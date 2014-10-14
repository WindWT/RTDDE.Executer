using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RTDDE.Executer
{
    public class MapCell
    {
        public string CellData { get; set; }
        public Brush Foreground { get; set; }
        public Brush Background { get; set; }
        public Color OverlayColor { get; set; }
        public FontWeight fontWeight { get; set; }
        public string drop_unit_id { get; set; }
        public string add_attribute_exp { get; set; }
        public string drop_id { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public MapCell()
            : this(string.Empty)
        {
        }
        public MapCell(string cellData)
            : this(cellData, Brushes.Black, Brushes.White, FontWeights.Normal)
        {
        }
        public MapCell(string cellData, Brush foreground, Brush background, FontWeight bold)
            : this(cellData, foreground, background, bold, Colors.Transparent)
        {
        }
        public MapCell(string cellData, Brush foreground, Brush background, FontWeight bold, Color overlayColor)
        {
            this.CellData = cellData;
            this.Foreground = foreground;
            this.Background = background;
            this.fontWeight = bold;
            this.OverlayColor = overlayColor;
            this.drop_unit_id = string.Empty;
            this.add_attribute_exp = string.Empty;
            this.drop_id = string.Empty;
        }
        public override string ToString()
        {
            return CellData;
        }
    }
    public class MapRow
    {
        public List<MapCell> Cells { get; set; }
        public MapRow()
        {
            Cells = new List<MapCell>();
        }
    }
    public class MapTable
    {
        public List<MapRow> Rows { get; set; }
        public MapTable()
        {
            Rows = new List<MapRow>();
        }
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public int repeat { get; set; }
    }
    /*public class MapColumn
    {
        public ObservableCollection<MapCell> MapCells { get; set; }
        public MapColumn()
        {
            MapCells = new ObservableCollection<MapCell>();
        }
    }
    public class MapTable
    {
        public ObservableCollection<MapColumn> MapColumns { get; set; }
        public MapTable()
        {
            MapColumns = new ObservableCollection<MapColumn>();
        }
    }*/
}
