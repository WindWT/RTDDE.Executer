using RTDDE.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTDDE.Executer
{
    /// <summary>
    /// Common.xaml 的交互逻辑
    /// </summary>
    public partial class Common : UserControl
    {
        public Common()
        {
            InitializeComponent();
        }
        private void CommonTab_Initialized(object sender, EventArgs e)
        {
            CommonSQLComboBox.ItemsSource = new Dictionary<string, string>()
            {
                {"------",""},
                {"Rank","SELECT * FROM USER_RANK_MASTER"},
                {"MDB","SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"},
                {"LoginBonus",@"SELECT *,(CASE 
WHEN present_type=3 THEN 'STONE' 
WHEN present_type=4 THEN (SELECT unit_master.name FROM unit_master WHERE unit_master.id=LOGIN_BONUS_MASTER.present_param)
ELSE '' END) as present_name 
FROM LOGIN_BONUS_MASTER order by day"}
            };
        }
        private void CommonSQLComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sql = CommonSQLTextBox.Text;
            if (String.IsNullOrWhiteSpace(sql) == false)
            {
                Utility.BindData(CommonDataGrid, sql);
            }
        }
        private void CommonRunSQL_Click(object sender, RoutedEventArgs e)
        {
            string sql = CommonSQLTextBox.Text;
            Utility.BindData(CommonDataGrid, sql);
            Utility.ShowException("SQL Executed.");
        }
    }
}
