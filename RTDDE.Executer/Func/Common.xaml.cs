using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RTDDE.Executer.Func
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
                {"------", ""},
                {"Rank", "SELECT * FROM USER_RANK_MASTER"},
                {"MDB", "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"},
                {"LoginBonus", @"SELECT *,(CASE 
WHEN present_type=3 THEN 'STONE' 
WHEN present_type=4 THEN (SELECT unit_master.name FROM unit_master WHERE unit_master.id=LOGIN_BONUS_MASTER.present_param)
ELSE '' END) as present_name 
FROM LOGIN_BONUS_MASTER order by day"},
                {"Gacha", @"SELECT * FROM GACHA_ITEM_MASTER order by id desc"},
                {"Yorishiro",@"select unit_master.g_id,name,memo from SPIRITS_TREE_COLLECTION left join unit_master on SPIRITS_TREE_COLLECTION.unit_id=unit_master.id" },
                {"5.8->5.9 Update",@"ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss01_id;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss01_set_id;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss01_lv_min;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss01_lv_max;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss01_rate;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss01_drop_id;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss01_bgm_id;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss02_id;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss02_set_id;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss02_lv_min;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss02_lv_max;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss02_rate;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss02_drop_id;
ALTER TABLE ENEMY_TABLE_MASTER ADD COLUMN boss02_bgm_id;" }
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
