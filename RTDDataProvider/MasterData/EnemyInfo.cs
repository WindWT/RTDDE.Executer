using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class EnemyInfo
    {
        public int enemy_id;
        public int enemy_set_id;
        public int x;
        public int y;
        public bool flag;
        public int drop_data_id;
        public int drop_unit_id;
        public int drop_unit_level;
        public int sp_event_point;
        public int add_attribute_exp;
    }
}
