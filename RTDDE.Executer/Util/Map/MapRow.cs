using System.Collections.Generic;

namespace RTDDE.Executer.Util.Map
{
    public class MapRow
    {
        public List<MapCell> Cells { get; set; }
        public MapRow()
        {
            Cells = new List<MapCell>();
        }
    }
}