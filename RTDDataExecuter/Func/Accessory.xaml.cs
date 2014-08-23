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
    /// Skill1.xaml 的交互逻辑
    /// </summary>
    public partial class Accessory : UserControl
    {
        public Accessory()
        {
            InitializeComponent();
            SkillTypeRadio_Party.IsChecked = true;  //set it here instead of xaml
            var attrDict = new Dictionary<string, string>()
            {
                {"------",""},
                {"NONE","1"},
                {"FIRE","2"},
                {"WATER","3"},
                {"LIGHT","4"},
                {"DARK","5"}
            };
            SkillSearch_attribute.ItemsSource = attrDict;
            //SkillSearch_sub_attr.ItemsSource = attrDict;
        }
        public void Refresh()
        {
            SkillTypeRadio_Party.IsChecked = false;
            SkillTypeRadio_Party.IsChecked = true;
        }
        private void SkillDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SkillDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string id = ((DataRowView)SkillDataGrid.SelectedItem).Row["id"].ToString();

            if (SkillTypeRadio_Party.IsChecked == true)
            {
                Task<SkillMaster> task = new Task<SkillMaster>(() =>
                {
                    DB db = new DB();
                    DataTable dt = db.GetData(string.Format("select * from party_skill_master where id={0}", id));
                    var sm =
                        from r in dt.AsEnumerable()
                        select new SkillMaster
                        {
                            id = Convert.ToInt32(r["id"]),
                            name = r["name"].ToString(),
                            type = Convert.ToInt32(r["type"]),
                            attribute = Convert.ToInt32(r["attribute"]),
                            sub_attr = Convert.ToInt32(r["sub_attr"]),
                            style = Convert.ToInt32(r["style"]),
                            num = Convert.ToInt32(r["num"]),
                            num_01 = Convert.ToInt32(r["num_01"]),
                            num_02 = Convert.ToInt32(r["num_02"]),
                            num_03 = Convert.ToInt32(r["num_03"]),
                            text = r["text"].ToString()
                        };
                    return sm.First<SkillMaster>();
                });
                Task<DataTable> taskSkillUnitRank = new Task<DataTable>(() =>
                {
                    DB db = new DB();
                    return db.GetData(string.Format(@"Select um.id,um.g_id,um.name,
        (CASE WHEN skill_01_09={0} THEN 1 ELSE 0 END) as s0109,
        (CASE WHEN skill_10_19={0} THEN 1 ELSE 0 END) as s1019,
        (CASE WHEN skill_20_29={0} THEN 1 ELSE 0 END) as s2029,
        (CASE WHEN skill_30_39={0} THEN 1 ELSE 0 END) as s3039,
        (CASE WHEN skill_40_49={0} THEN 1 ELSE 0 END) as s4049,
        (CASE WHEN skill_50_59={0} THEN 1 ELSE 0 END) as s5059,
        (CASE WHEN skill_60_69={0} THEN 1 ELSE 0 END) as s6069,
        (CASE WHEN skill_70_79={0} THEN 1 ELSE 0 END) as s7079,
        (CASE WHEN skill_80_89={0} THEN 1 ELSE 0 END) as s8089,
        (CASE WHEN skill_90_99={0} THEN 1 ELSE 0 END) as s9099,
        (CASE WHEN skill_100={0} THEN 1 ELSE 0 END) as s100
        From Unit_master as um
        LEFT JOIN Party_skill_rank_master AS psrm on um.p_skill_id=psrm.id
        ORDER BY um.g_id", id));
                });
                taskSkillUnitRank.ContinueWith(tUSR =>
                {
                    Task.WaitAll(task);
                    if (tUSR.Exception != null)
                    {
                        Utility.ShowException(tUSR.Exception.InnerException.Message);
                        return;
                    }
                    if (task.Exception != null)
                    {
                        Utility.ShowException(task.Exception.InnerException.Message);
                        return;
                    }
                    if (tUSR.Result == null || tUSR.Result.Rows.Count == 0)
                    {
                        return;
                    }
                    SkillMaster sm = task.Result;

                    partySkill_id.Text = sm.id.ToString();
                    partySkill_name.Text = sm.name;
                    partySkill_text.Document = Utility.parseTextToDocument(sm.text);
                    partySkill_type.Text = Utility.ParseSkillType((PassiveSkillType)sm.type);
                    partySkill_attribute.Text = Utility.ParseAttributetype(sm.attribute);
                    partySkill_sub_attr.Text = Utility.ParseAttributetype(sm.sub_attr);
                    partySkill_style.Text = Utility.ParseStyletype(sm.style);
                    partySkill_num.Text = sm.num.ToString();
                    partySkill_num_01.Text = sm.num_01.ToString();
                    partySkill_num_02.Text = sm.num_02.ToString();
                    partySkill_num_03.Text = sm.num_03.ToString();

                    SetSkillUnitRankInfo(tUSR.Result);

                }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
                task.Start();
                taskSkillUnitRank.Start();
            }
            else if (SkillTypeRadio_Active.IsChecked == true)
            {
                Task<SkillMaster> task = new Task<SkillMaster>(() =>
                {
                    DB db = new DB();
                    DataTable dt = db.GetData(string.Format("select * from active_skill_master where id={0}", id));
                    var sm =
                        from r in dt.AsEnumerable()
                        select new SkillMaster
                        {
                            id = Convert.ToInt32(r["id"]),
                            name = r["name"].ToString(),
                            type = Convert.ToInt32(r["type"]),
                            attribute = Convert.ToInt32(r["attribute"]),
                            sub_attr = Convert.ToInt32(r["sub_attr"]),
                            style = Convert.ToInt32(r["style"]),
                            num = Convert.ToInt32(r["num"]),
                            num_01 = Convert.ToInt32(r["num_01"]),
                            num_02 = Convert.ToInt32(r["num_02"]),
                            num_03 = Convert.ToInt32(r["num_03"]),
                            soul = Convert.ToInt32(r["soul"]),
                            phase = Convert.ToInt32(r["phase"]),
                            limit_num = Convert.ToInt32(r["limit_num"]),
                            text = r["text"].ToString()
                        };
                    return sm.First<SkillMaster>();
                });
                Task<DataTable> taskSkillUnitRank = new Task<DataTable>(() =>
                {
                    DB db = new DB();
                    return db.GetData(string.Format(@"Select um.id,um.g_id,um.name,
        (CASE WHEN skill_01_09={0} THEN 1 ELSE 0 END) as s0109,
        (CASE WHEN skill_10_19={0} THEN 1 ELSE 0 END) as s1019,
        (CASE WHEN skill_20_29={0} THEN 1 ELSE 0 END) as s2029,
        (CASE WHEN skill_30_39={0} THEN 1 ELSE 0 END) as s3039,
        (CASE WHEN skill_40_49={0} THEN 1 ELSE 0 END) as s4049,
        (CASE WHEN skill_50_59={0} THEN 1 ELSE 0 END) as s5059,
        (CASE WHEN skill_60_69={0} THEN 1 ELSE 0 END) as s6069,
        (CASE WHEN skill_70_79={0} THEN 1 ELSE 0 END) as s7079,
        (CASE WHEN skill_80_89={0} THEN 1 ELSE 0 END) as s8089,
        (CASE WHEN skill_90_99={0} THEN 1 ELSE 0 END) as s9099,
        (CASE WHEN skill_100={0} THEN 1 ELSE 0 END) as s100
        From Unit_master as um
        LEFT JOIN Active_skill_rank_master AS asrm on um.a_skill_id=asrm.id
        ORDER BY um.g_id", id));
                });
                taskSkillUnitRank.ContinueWith(tUSR =>
                {
                    Task.WaitAll(task);
                    if (tUSR.Exception != null)
                    {
                        Utility.ShowException(tUSR.Exception.InnerException.Message);
                        return;
                    }
                    if (task.Exception != null)
                    {
                        Utility.ShowException(task.Exception.InnerException.Message);
                        return;
                    }
                    if (tUSR.Result == null || tUSR.Result.Rows.Count == 0)
                    {
                        return;
                    }
                    SkillMaster sm = task.Result;

                    activeSkill_id.Text = sm.id.ToString();
                    activeSkill_name.Text = sm.name;
                    activeSkill_text.Document = Utility.parseTextToDocument(sm.text);
                    activeSkill_type.Text = Utility.ParseSkillType((ActiveSkillType)sm.type);
                    activeSkill_attribute.Text = Utility.ParseAttributetype(sm.attribute);
                    activeSkill_sub_attr.Text = Utility.ParseAttributetype(sm.sub_attr);
                    activeSkill_style.Text = Utility.ParseStyletype(sm.style);
                    activeSkill_num.Text = sm.num.ToString();
                    activeSkill_num_01.Text = sm.num_01.ToString();
                    activeSkill_num_02.Text = sm.num_02.ToString();
                    activeSkill_num_03.Text = sm.num_03.ToString();
                    activeSkill_soul.Text = sm.soul.ToString();
                    activeSkill_phase.Text = ((SkillPhase)sm.phase).ToString();
                    activeSkill_limit_num.Text = sm.limit_num.ToString();

                    SetSkillUnitRankInfo(tUSR.Result);

                }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
                task.Start();
                taskSkillUnitRank.Start();
            }
            else if (SkillTypeRadio_Panel.IsChecked == true)
            {
                Task<SkillMaster> task = new Task<SkillMaster>(() =>
                {
                    DB db = new DB();
                    DataTable dt = db.GetData(string.Format("select * from panel_skill_master where id={0}", id));
                    var sm =
                        from r in dt.AsEnumerable()
                        select new SkillMaster
                        {
                            id = Convert.ToInt32(r["id"]),
                            name = r["name"].ToString(),
                            type = Convert.ToInt32(r["type"]),
                            attribute = Convert.ToInt32(r["attribute"]),
                            style = Convert.ToInt32(r["style"]),
                            num = Convert.ToInt32(r["num"]),
                            num_01 = Convert.ToInt32(r["num_01"]),
                            num_02 = Convert.ToInt32(r["num_02"]),
                            num_03 = Convert.ToInt32(r["num_03"]),
                            phase = Convert.ToInt32(r["phase"]),
                            duplication = Convert.ToInt32(r["duplication"]),
                            text = r["text"].ToString()
                        };
                    return sm.First<SkillMaster>();
                });
                Task<DataTable> taskSkillUnitRank = new Task<DataTable>(() =>
                {
                    DB db = new DB();
                    return db.GetData(string.Format(@"Select um.id,um.g_id,um.name,
        (CASE WHEN skill_01_09={0} THEN 1 ELSE 0 END) as s0109,
        (CASE WHEN skill_10_19={0} THEN 1 ELSE 0 END) as s1019,
        (CASE WHEN skill_20_29={0} THEN 1 ELSE 0 END) as s2029,
        (CASE WHEN skill_30_39={0} THEN 1 ELSE 0 END) as s3039,
        (CASE WHEN skill_40_49={0} THEN 1 ELSE 0 END) as s4049,
        (CASE WHEN skill_50_59={0} THEN 1 ELSE 0 END) as s5059,
        (CASE WHEN skill_60_69={0} THEN 1 ELSE 0 END) as s6069,
        (CASE WHEN skill_70_79={0} THEN 1 ELSE 0 END) as s7079,
        (CASE WHEN skill_80_89={0} THEN 1 ELSE 0 END) as s8089,
        (CASE WHEN skill_90_99={0} THEN 1 ELSE 0 END) as s9099,
        (CASE WHEN skill_100={0} THEN 1 ELSE 0 END) as s100
        FROM Unit_master as um
        LEFT JOIN Panel_skill_rank_master AS psrm on um.panel_skill_id=psrm.id
        ORDER BY um.g_id", id));
                });
                taskSkillUnitRank.ContinueWith(tUSR =>
                {
                    Task.WaitAll(task);
                    if (tUSR.Exception != null)
                    {
                        Utility.ShowException(tUSR.Exception.InnerException.Message);
                        return;
                    }
                    if (task.Exception != null)
                    {
                        Utility.ShowException(task.Exception.InnerException.Message);
                        return;
                    }
                    if (tUSR.Result == null || tUSR.Result.Rows.Count == 0)
                    {
                        return;
                    }
                    SkillMaster sm = task.Result;

                    panelSkill_id.Text = sm.id.ToString();
                    panelSkill_name.Text = sm.name;
                    panelSkill_text.Document = Utility.parseTextToDocument(sm.text);
                    panelSkill_type.Text = Utility.ParseSkillType((PanelSkillType)sm.type);
                    panelSkill_attribute.Text = Utility.ParseAttributetype(sm.attribute);
                    panelSkill_style.Text = Utility.ParseStyletype(sm.style);
                    panelSkill_num.Text = sm.num.ToString();
                    panelSkill_num_01.Text = sm.num_01.ToString();
                    panelSkill_num_02.Text = sm.num_02.ToString();
                    panelSkill_num_03.Text = sm.num_03.ToString();
                    panelSkill_phase.Text = ((SkillPhase)sm.phase).ToString();
                    panelSkill_duplication.Text = sm.duplication == 1 ? "重複可" : sm.duplication == 2 ? "重複不可" : String.Empty;

                    SetSkillUnitRankInfo(tUSR.Result);

                }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
                task.Start();
                taskSkillUnitRank.Start();
            }
            else
            {
                return;
            }
        }
        private void SetSkillUnitRankInfo(DataTable dt)
        {
            SkillUnitRankInfo.Children.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                bool[] skillList = new bool[11]{
                Convert.ToBoolean(dr["s0109"]),
                Convert.ToBoolean(dr["s1019"]),
                Convert.ToBoolean(dr["s2029"]),
                Convert.ToBoolean(dr["s3039"]),
                Convert.ToBoolean(dr["s4049"]),
                Convert.ToBoolean(dr["s5059"]),
                Convert.ToBoolean(dr["s6069"]),
                Convert.ToBoolean(dr["s7079"]),
                Convert.ToBoolean(dr["s8089"]),
                Convert.ToBoolean(dr["s9099"]),
                Convert.ToBoolean(dr["s100"]),
                };
                bool hasSkill = false;
                foreach (bool s in skillList)
                {
                    if (s)
                    {
                        hasSkill = true;
                        break;
                    }
                }
                if (hasSkill)
                {
                    SkillUnitRankInfo.Children.Add(new TextBlock()
                    {
                        Text = dr["g_id"].ToString(),
                        Width = 25
                    });
                    var tb = new TextBox()
                    {
                        Tag = dr["id"].ToString(),
                        Text = dr["name"].ToString(),
                        Width = 300 - 11 * 10 - 25
                    };
                    tb.MouseDoubleClick += tb_MouseDoubleClick;
                    SkillUnitRankInfo.Children.Add(tb);
                    foreach (bool s in skillList)
                    {
                        SkillUnitRankInfo.Children.Add(new Rectangle()
                        {
                            Fill = s ? (SolidColorBrush)Application.Current.Resources["DefaultBrush"] : Brushes.Transparent,
                            Stroke = (SolidColorBrush)Application.Current.Resources["PressedBrush"],
                            StrokeThickness = 1,
                            Width = 10
                        });
                    }
                }
            }
            SkillUnitRankInfo.Children.Add(new Separator() { Width = 300 });
        }
        private void tb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var w = (MainWindow)Application.Current.MainWindow;
                string id = (sender as TextBox).Tag.ToString();
                ((TextBox)w.Unit.FindName("UnitInfo_id")).Text = id;
                if (Settings.IsDefaultLvMax)
                {
                    ((TextBox)w.Unit.FindName("UnitInfo_lv")).Text = "99";
                }
                else
                {
                    ((TextBox)w.Unit.FindName("UnitInfo_lv")).Text = "1";
                }
                w.Unit.UnitInfo_BindData(id);
                w.ChangeTab("Unit");
            }
        }
        private void SkillTypeRadio_Party_Checked(object sender, RoutedEventArgs e)
        {
            SkillInfo_PartySkill.Visibility = Visibility.Visible;
            SkillInfo_ActiveSkill.Visibility = Visibility.Collapsed;
            SkillInfo_PanelSkill.Visibility = Visibility.Collapsed;
            SkillUnitRankInfo.Children.Clear();
            Utility.BindData(SkillDataGrid, "select id,type,name from party_skill_master order by type,id");
        }
        private void SkillTypeRadio_Active_Checked(object sender, RoutedEventArgs e)
        {
            SkillInfo_PartySkill.Visibility = Visibility.Collapsed;
            SkillInfo_ActiveSkill.Visibility = Visibility.Visible;
            SkillInfo_PanelSkill.Visibility = Visibility.Collapsed;
            SkillUnitRankInfo.Children.Clear();
            Utility.BindData(SkillDataGrid, "select id,type,name from active_skill_master order by type,id");
        }
        private void SkillTypeRadio_Panel_Checked(object sender, RoutedEventArgs e)
        {
            SkillInfo_PartySkill.Visibility = Visibility.Collapsed;
            SkillInfo_ActiveSkill.Visibility = Visibility.Collapsed;
            SkillInfo_PanelSkill.Visibility = Visibility.Visible;
            SkillUnitRankInfo.Children.Clear();
            Utility.BindData(SkillDataGrid, "select id,type,name from panel_skill_master order by type,id");
        }
        private string GetSkillTableByRadioChecked()
        {
            string tableName = string.Empty;
            if (SkillTypeRadio_Party.IsChecked == true)
            {
                tableName = "party_skill_master";
            }
            else if (SkillTypeRadio_Active.IsChecked == true)
            {
                tableName = "active_skill_master";
            }
            else if (SkillTypeRadio_Panel.IsChecked == true)
            {
                tableName = "panel_skill_master";
            }
            return tableName;
        }

        private void SkillSearchClear_Click(object sender, RoutedEventArgs e)
        {
            SkillSearch_name.Text = string.Empty;
            SkillSearch_type.Text = string.Empty;
            SkillSearch_attribute.SelectedIndex = 0;
            //SkillSearch_sub_attr.SelectedIndex = 0;
            Utility.BindData(SkillDataGrid, "select id,type,name from " + GetSkillTableByRadioChecked() + " order by type,id");
        }
        private void SkillSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utility.BindData(SkillDataGrid, SkillSearch_BuildSQL());
        }
        private void SkillSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.BindData(SkillDataGrid, SkillSearch_BuildSQL());
        }
        private string SkillSearch_BuildSQL()
        {
            string sql = "select id,type,name from " + GetSkillTableByRadioChecked() + " WHERE ";
            if (String.IsNullOrWhiteSpace(SkillSearch_name.Text) == false)
            {
                sql += "name LIKE '%" + SkillSearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace(SkillSearch_type.Text) == false)
            {
                sql += "type=" + SkillSearch_type.Text.Trim() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)SkillSearch_attribute.SelectedValue) == false)
            {
                sql += "attribute=" + SkillSearch_attribute.SelectedValue.ToString() + " AND ";
            }
            //if (String.IsNullOrWhiteSpace((string)SkillSearch_sub_attr.SelectedValue) == false)
            //{
            //    sql += "sub_attr=" + SkillSearch_sub_attr.SelectedValue.ToString() + " AND ";
            //}
            sql += " 1=1 order by type,id";
            return sql;
        }
    }
}
