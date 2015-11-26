using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Accessory.xaml 的交互逻辑
    /// </summary>
    public partial class Accessory : UserControl
    {
        public Accessory()
        {
            InitializeComponent();
            Utility.DisableBindData = true;
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
            Utility.DisableBindData = false;
            //AccessorySearch_sub_attr.ItemsSource = attrDict;
        }
        private void AccessoryTab_Initialized(object sender, EventArgs e)
        {
            Utility.BindData(AccessoryDataGrid, "SELECT id,type,name FROM Accessory_MASTER order by type,id");
        }

        private async void AccessoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (AccessoryDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string id = ((DataRowView) AccessoryDataGrid.SelectedItem).Row["id"].ToString();

            Task<AccessoryMaster> taskAccessory = Task.Run(() => DAL.ToSingle<AccessoryMaster>($"SELECT * FROM ACCESSORY_MASTER WHERE id={id}"));
            Task<UnitMaster> taskAccessoryUnit = Task.Run(() => DAL.ToSingle<UnitMaster>($"SELECT * FROM UNIT_MASTER WHERE id={id}"));

            AccessoryMaster am;
            UnitMaster um;
            try {
                am = await taskAccessory;
                um = await taskAccessoryUnit;
            }
            catch (Exception ex) {
                Utility.ShowException(ex.Message);
                return;
            }

            if (am == null) {
                return;
            }
            Accessory_id.Text = id;
            Accessory_name.Text = am.name;
            if (um != null) {
                Accessory_unit_id.Text = um.id.ToString();
                Accessory_unit_g_id.Text = um.g_id.ToString();
                Accessory_unit_name.Text = um.name;
            }
            Accessory_detail.Document = Utility.ParseTextToDocument(am.detail);
            Accessory_type.Text = Utility.ParseAccessoryType(am.type);
            Accessory_attribute.Text = Utility.ParseAttributetype(am.attribute);
            Accessory_su_a1.Text = am.su_a1.ToString(); //not sub attribute
            Accessory_style.Text = Utility.ParseStyletype(am.style);
            Accessory_num_01.Text = am.num_01.ToString();
            Accessory_num_02.Text = am.num_02.ToString();
            Accessory_num_03.Text = am.num_03.ToString();
            Accessory_num_04.Text = am.num_04.ToString();
            Accessory_conv_money.Text = am.conv_money.ToString();
            Accessory_icon.Text = am.icon;
        }

        private void AccessorySearchClear_Click(object sender, RoutedEventArgs e)
        {
            AccessorySearch_name.Text = string.Empty;
            AccessorySearch_type.Text = string.Empty;
            AccessorySearch_type.SelectedIndex = 0;
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
            sql += " 1=1 order by type,id";
            return sql;
        }

        private void AccessoryInfoToUnitButton_OnClick(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(Accessory_unit_id.Text) == false) {
                Utility.GoToItemById<Unit>(Convert.ToInt32(Accessory_unit_id.Text));
            }
        }
    }
}
