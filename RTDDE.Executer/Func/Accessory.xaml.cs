using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RTDDE.Provider;
using RTDDE.Provider.Enums;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Accessory.xaml 的交互逻辑
    /// </summary>
    public partial class Accessory : UserControl, IRefreshable
    {
        public Accessory()
        {
            InitializeComponent();
            var attrDict = new Dictionary<string, string>()
            {
                {"------",""},
                {"NONE","1"},
                {"FIRE","2"},
                {"WATER","3"},
                {"LIGHT","4"},
                {"DARK","5"}
            };
            AccessorySearch_attribute.ItemsSource = attrDict;
            var typeDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (AccessoryType type in Enum.GetValues(typeof(AccessoryType)))
            {
                string id = ((int) type).ToString();
                typeDict.Add(string.Format("{0}_{1}", id, type.ToString()), id);
            }
            AccessorySearch_type.ItemsSource = typeDict;
            //AccessorySearch_sub_attr.ItemsSource = attrDict;
        }
        public void Refresh()
        {
            Utility.BindData(AccessoryDataGrid, "SELECT id,type,name FROM Accessory_MASTER order by type,id");
        }
        private void AccessoryTab_Initialized(object sender, EventArgs e)
        {
            Refresh();
        }
        private void AccessoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccessoryDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string id = ((DataRowView)AccessoryDataGrid.SelectedItem).Row["id"].ToString();

            Task<DataTable> task = new Task<DataTable>(() =>
                {
                    return DAL.GetDataTable(string.Format("select Accessory_master.*,unit_master.g_id,unit_master.name AS unitname from Accessory_master LEFT JOIN unit_master ON Accessory_master.id=unit_master.id where Accessory_master.id={0}", id));
                });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (t.Result == null || t.Result.Rows.Count == 0)
                {
                    return;
                }
                DataRow dr = t.Result.Rows[0];
                Accessory_id.Text = dr["id"].ToString();
                Accessory_name.Text = dr["name"].ToString();
                Accessory_unit_g_id.Text = dr["g_id"].ToString();
                Accessory_unit_name.Text = dr["unitname"].ToString();
                Accessory_detail.Document = Utility.ParseTextToDocument(dr["detail"].ToString());
                Accessory_type.Text = Utility.ParseAccessoryType(Convert.ToInt32(dr["type"]));
                Accessory_attribute.Text = Utility.ParseAttributetype(Convert.ToInt32(dr["attribute"]));
                Accessory_su_a1.Text = dr["su_a1"].ToString();  //not sub attribute
                Accessory_style.Text = Utility.ParseStyletype(Convert.ToInt32(dr["style"]));
                Accessory_num_01.Text = dr["num_01"].ToString();
                Accessory_num_02.Text = dr["num_02"].ToString();
                Accessory_num_03.Text = dr["num_03"].ToString();
                Accessory_num_04.Text = dr["num_04"].ToString();
                Accessory_conv_money.Text = dr["conv_money"].ToString();
                Accessory_icon.Text = dr["icon"].ToString();
            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }

        private void AccessorySearchClear_Click(object sender, RoutedEventArgs e)
        {
            AccessorySearch_name.Text = string.Empty;
            AccessorySearch_type.Text = string.Empty;
            AccessorySearch_attribute.SelectedIndex = 0;
            //AccessorySearch_sub_attr.SelectedIndex = 0;
            Utility.BindData(AccessoryDataGrid, "SELECT id,type,name FROM Accessory_MASTER order by type,id");
        }
        private void AccessorySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utility.BindData(AccessoryDataGrid, AccessorySearch_BuildSQL());
        }
        private void AccessorySearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.BindData(AccessoryDataGrid, AccessorySearch_BuildSQL());
        }
        private string AccessorySearch_BuildSQL()
        {
            string sql = "SELECT id,type,name FROM Accessory_MASTER WHERE ";
            if (String.IsNullOrWhiteSpace(AccessorySearch_name.Text) == false)
            {
                sql += "name LIKE '%" + AccessorySearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace((string)AccessorySearch_type.SelectedValue) == false)
            {
                sql += "type=" + AccessorySearch_type.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)AccessorySearch_attribute.SelectedValue) == false)
            {
                sql += "attribute=" + AccessorySearch_attribute.SelectedValue.ToString() + " AND ";
            }
            //if (String.IsNullOrWhiteSpace((string)AccessorySearch_sub_attr.SelectedValue) == false)
            //{
            //    sql += "sub_attr=" + AccessorySearch_sub_attr.SelectedValue.ToString() + " AND ";
            //}
            sql += " 1=1 order by type,id";
            return sql;
        }
    }
}
