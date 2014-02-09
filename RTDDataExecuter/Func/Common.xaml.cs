using RTDDataProvider;
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

namespace RTDDataExecuter
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
                {"LoginBonus","SELECT * FROM LOGIN_BONUS_MASTER order by day"}
            };
        }
        private void CommonSQLComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sql = CommonSQLTextBox.Text;
            Utility.BindData(CommonDataGrid, sql);
        }
        private void CommonRunSQL_Click(object sender, RoutedEventArgs e)
        {
            string sql = CommonSQLTextBox.Text;
            Utility.BindData(CommonDataGrid, sql);
        }
    }
}
