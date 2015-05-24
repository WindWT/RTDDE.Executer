using System.Windows;
using System.Windows.Media;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Util.Map
{
    public class MapCell
    {
        public int RawCellData { get; set; }
        public string CellData { get; set; }
        public Brush Foreground { get; set; }
        public Brush Background { get; set; }
        public Color YorishiroColor { get; set; }
        public Color AttributeColor { get; set; }
        public FontWeight fontWeight { get; set; }
        public UnitMaster drop_unit { get; set; }
        public int add_attribute_exp { get; set; }
        public int unit_exp { get; set; }
        public int gold_pt { get; set; }
        public int drop_id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool HasDropInfo { get; set; }

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
        public MapCell(string cellData, Brush foreground, Brush background, FontWeight bold, Color yorishiroColor)
        {
            this.CellData = cellData;
            this.Foreground = foreground;
            this.Background = background;
            this.fontWeight = bold;
            this.YorishiroColor = yorishiroColor;
            this.AttributeColor = Colors.Transparent;
        }
        public override string ToString()
        {
            return CellData;
        }
    }
}