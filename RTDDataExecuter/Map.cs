using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RTDDataExecuter
{
    public class MapCell
    {
        public string CellData { get; set; }
        public Brush Foreground { get; set; }
        public Brush Background { get; set; }
        public FontWeight fontWeight { get; set; }

        public MapCell()
            : this(string.Empty)
        {
        }
        public MapCell(string cellData)
            : this(cellData, Brushes.Black, Brushes.Transparent, FontWeights.Normal)
        {
        }
        public MapCell(string cellData, Brush foreground, Brush background, FontWeight bold)
        {
            this.CellData = cellData;
            this.Foreground = foreground;
            this.Background = background;
            this.fontWeight = bold;
        }
        public override string ToString()
        {
            return CellData;
        }
    }
    public class MapRow
    {
        public List<MapCell> MapCells { get; set; }
        public MapRow()
        {
            MapCells = new List<MapCell>();
        }
    }
    public class MapTable
    {
        public List<MapRow> MapRows { get; set; }
        public MapTable()
        {
            MapRows = new List<MapRow>();
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
