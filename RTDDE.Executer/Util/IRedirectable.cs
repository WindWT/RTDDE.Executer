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
        /// <summary>
        /// 获取目标DataGrid接口
        /// 接口调用前tab为初始状态
        /// 接口内部需要设置DataGrid的数据
        /// </summary>
        /// <returns></returns>
        DataGrid GetTargetDataGrid(int firstId, int lastId = -1, string type = null);
    }
}
