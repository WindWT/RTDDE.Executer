﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RTDDataProvider;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class LevelDataMaster
    {
        [DAL(PrimaryKey = true)]
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
        //public EventCutinMaster[] event_cutin_master;
        public uint clear_talk_id;
    }
}
