using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class PartySkillMaster
    {
        public int id;
        public string name;
        public int type;
        public int attribute;
        public int sub_attr;
        public int style;
        public int num;
        public int num_01;
        public int num_02;
        public int num_03;
        public string text;
    }
}
