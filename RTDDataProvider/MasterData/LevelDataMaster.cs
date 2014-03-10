using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class LevelDataMaster
    {
        public int level_data_id;
        public int format;
        public int start_x;
        public int start_y;
        public int width;
        public int height;
        public string map_data;
        public string hash;
        //public EnemyTableMaster enemy_table_master;
        //public UnitTalkMaster unit_talk_master;
    }
}
