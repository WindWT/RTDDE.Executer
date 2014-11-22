using RTDDE.Provider;
using RTDDE.Provider.MasterData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Enemy.xaml 的交互逻辑
    /// </summary>
    public partial class Enemy : UserControl
    {
        public Enemy()
        {
            InitializeComponent();
        }
        public void Refresh()
        {
            EnemyTab_Initialized(null, null);
        }
        private void EnemyTab_Initialized(object sender, EventArgs e)
        {
            Utility.BindData(EnemyDataGrid, "SELECT id,name FROM Enemy_Unit_MASTER order by id");
            EnemySearch_category.ItemsSource = new Dictionary<string, string>()
            {
                {"------",""},
                {"★","1"},
                {"★★","2"},
                {"★★★","3"},
                {"★★★★","4"},
                {"★★★★★","5"},
                {"★×6","6"}
            };
            EnemySearch_style.ItemsSource = new Dictionary<string, string>()
            {
                {"------",""},
                {"KNIGHT","1"},
                {"LANCER","2"},
                {"ARCHER","3"},
                {"WIZARD","4"}
            };
            var attrDict = new Dictionary<string, string>()
            {
                {"------",""},
                {"NONE","1"},
                {"FIRE","2"},
                {"WATER","3"},
                {"LIGHT","4"},
                {"DARK","5"}
            };
            EnemySearch_attribute.ItemsSource = attrDict;
            EnemySearch_sub_a1.ItemsSource = attrDict;
            var kindDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (AssignID kind in Enum.GetValues(typeof(AssignID)))
            {
                if (kind.ToString().StartsWith("MS0") == false)
                {
                    kindDict.Add(kind.ToString(), Utility.ParseAssignID(kind).ToString());
                }
            }
            EnemySearch_kind.ItemsSource = kindDict;
        }

        private void EnemyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemyDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string Enemyid = ((DataRowView)EnemyDataGrid.SelectedItem).Row["id"].ToString();
            EnemyInfo_id.Text = Enemyid;

            if (Settings.IsDefaultLvMax)
            {
                EnemyInfo_lv.Text = "99";
            }
            else
            {
                EnemyInfo_lv.Text = "1";
            }

            EnemyInfo_BindData(Enemyid);
        }
        private void EnemyInfo_lv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EnemyInfo_lv.Text))
            {
                EnemyInfo_lv.Text = "";
            }
            Regex r = new Regex("[^0-9]");
            if (r.Match(EnemyInfo_lv.Text).Success)
            {
                if (Settings.IsDefaultLvMax)
                {
                    EnemyInfo_lv.Text = "99";     //This will trigger itself again
                    return;
                }
                else
                {
                    EnemyInfo_lv.Text = "1";
                    return;
                }
            }
            EnemyInfo_BindData(EnemyInfo_id.Text);
        }

        public void EnemyInfo_BindData(string Enemyid)
        {
            if (string.IsNullOrWhiteSpace(EnemyInfo_lv.Text))
            {
                return;
            }
            double thislevel = Convert.ToDouble(Convert.ToInt32(EnemyInfo_lv.Text));

            Task<EnemyUnitMaster> task = new Task<EnemyUnitMaster>(() =>
            {
                string sql = @"
Select *
from Enemy_unit_master as eum
WHERE eum.id={0}";
                //return DAL.GetDataTable(String.Format(sql, Enemyid));
                return DAL.ToSingle<EnemyUnitMaster>(String.Format(sql, Enemyid));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (task.Result == null)
                {
                    return;
                }
                EnemyUnitMaster ui = task.Result;
                EnemyInfo_name.Text = ui.name;
                EnemyInfo_attribute.Text = Utility.ParseAttributetype(ui.attribute);

                EnemyInfo_lv.Text = thislevel.ToString("0");

            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }

        private void EnemySearchClear_Click(object sender, RoutedEventArgs e)
        {
            EnemySearch_id.Text = string.Empty;
            EnemySearch_g_id.Text = string.Empty;
            EnemySearch_name.Text = string.Empty;
            EnemySearch_category.SelectedIndex = 0;
            EnemySearch_style.SelectedIndex = 0;
            EnemySearch_attribute.SelectedIndex = 0;
            EnemySearch_sub_a1.SelectedIndex = 0;
            Utility.BindData(EnemyDataGrid, "SELECT id,name FROM Enemy_Unit_MASTER order by id");
        }
        private void EnemySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utility.BindData(EnemyDataGrid, EnemySearch_BuildSQL());
        }
        private void EnemySearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.BindData(EnemyDataGrid, EnemySearch_BuildSQL());
        }
        private string EnemySearch_BuildSQL()
        {
            string sql = @"SELECT id,name FROM Enemy_unit_MASTER WHERE ";
            if (String.IsNullOrWhiteSpace(EnemySearch_id.Text) == false)
            {
                sql += "id=" + EnemySearch_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(EnemySearch_name.Text) == false)
            {
                sql += "name LIKE '%" + EnemySearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_category.SelectedValue) == false)
            {
                sql += "category=" + EnemySearch_category.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_kind.SelectedValue) == false)
            {
                sql += "kind=" + EnemySearch_kind.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_style.SelectedValue) == false)
            {
                sql += "style=" + EnemySearch_style.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_attribute.SelectedValue) == false)
            {
                sql += "attribute=" + EnemySearch_attribute.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_sub_a1.SelectedValue) == false)
            {
                sql += "sub_a1=" + EnemySearch_sub_a1.SelectedValue.ToString() + " AND ";
            }
            sql += " 1=1 ORDER BY id";
            return sql;
        }

        public void SelectEnemyById(int id)
        {
            foreach (var item in EnemyDataGrid.Items)
            {
                if ((item as DataRowView).Row["id"].ToString() == id.ToString())
                {
                    EnemyDataGrid.SelectedItem = item;
                    EnemyDataGrid.ScrollIntoView(item);
                    break;
                }
            }
        }
    }
}
