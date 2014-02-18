using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTDDataProvider.MasterData
{
    [Serializable]
    public class ShopProductMaster
    {
        public string id;
        public string name;
        public uint price;
        public uint point;
    }
}
