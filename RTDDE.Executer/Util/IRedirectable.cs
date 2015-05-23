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
        /// 接口内部需要切换type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        DataGrid GetTargetDataGrid(string type = null);
    }
}
