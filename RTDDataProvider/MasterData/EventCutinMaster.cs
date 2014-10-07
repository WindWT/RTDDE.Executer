using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public struct EventCutinMaster
    {
        public EventCutinMaster.CutinMaster[] cutin_master;
        public int cutin_flag;
        public int cutin_w;

        [Serializable]
        public class CutinMaster
        {
            public int draw_pos;
            public string icon;
            public string text;
            public int start_msec;
            public int end_msec;
            public int flag;
        }
    }
}
