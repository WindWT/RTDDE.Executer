using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RTDDE.Executer.Util
{
    interface IRedirectable
    {
        void GoToItemById(int firstId, int lastId = -1);
    }
}
