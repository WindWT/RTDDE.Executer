using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class UserRankMaster
    {
        public int rank;
        public int need_exp;
        public int flags;
        public int game_money;
        public int friend_point;
        public int unit_id;
    }
}
