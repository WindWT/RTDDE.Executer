using System.Windows;
using System.Windows.Media;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Util.Map
{
    public class MapCell
    {
        public int RawCellData { get; set; }
        public string Text { get; set; }
        public int EnemyNo { get; set; }
        public int EnemyRate { get; set; }
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
        public MapCell(string text)
            : this(text, Brushes.Black, Brushes.White, FontWeights.Normal)
        {
        }
        public MapCell(string text, Brush foreground, Brush background, FontWeight bold)
            : this(text, foreground, background, bold, Colors.Transparent)
        {
        }
        public MapCell(string text, Brush foreground, Brush background, FontWeight bold, Color yorishiroColor)
        {
            this.Text = text;
            this.Foreground = foreground;
            this.Background = background;
            this.fontWeight = bold;
            this.YorishiroColor = yorishiroColor;
            this.AttributeColor = Colors.Transparent;
        }
        public override string ToString()
        {
            return Text;
        }
    }
}