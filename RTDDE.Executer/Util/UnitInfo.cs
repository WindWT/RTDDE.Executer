using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDE.Executer
{
    public class UnitInfo : RTDDE.Provider.MasterData.UnitMaster
    {
        public int rev_unit_g_id { get; set; }
        public string rev_unit_name { get; set; }
        public int ultimate_rev_unit_g_id_fire { get; set; }
        public string ultimate_rev_unit_name_fire { get; set; }
        public int ultimate_rev_unit_g_id_water { get; set; }
        public string ultimate_rev_unit_name_water { get; set; }
        public int ultimate_rev_unit_g_id_shine { get; set; }
        public string ultimate_rev_unit_name_shine { get; set; }
        public int ultimate_rev_unit_g_id_dark { get; set; }
        public string ultimate_rev_unit_name_dark { get; set; }
    }
}
