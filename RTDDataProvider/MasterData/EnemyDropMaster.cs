using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class EnemyDropMaster
    {
        public int id;
        public int unit1_id;
        public int unit1_lv_min;
        public int unit1_lv_max;
        public int unit1_rate;
        public int unit2_id;
        public int unit2_lv_min;
        public int unit2_lv_max;
        public int unit2_rate;
        public int unit3_id;
        public int unit3_lv_min;
        public int unit3_lv_max;
        public int unit3_rate;
        public int unit4_id;
        public int unit4_lv_min;
        public int unit4_lv_max;
        public int unit4_rate;
        public int sp_event_point;
        public int add_attribute_exp;
    }
}
