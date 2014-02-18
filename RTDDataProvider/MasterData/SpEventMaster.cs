using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class SpEventMaster
    {
        public int sp_event_id;
        public string icon;
        public string target_name;
        public string info_message_0;
        public string info_message_1;
        public string lock_message;
        public string unlock_message;
    }
}
