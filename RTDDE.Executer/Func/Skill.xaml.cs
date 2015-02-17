using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    public partial class Skill : UserControl
    {
        public Skill()
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
        [DAL(UseProperty = true)]
        private class SkillUnitRank
        {
            public int id { get; set; }
            public int g_id { get; set; }
            public string name { get; set; }
            public int skill_01_09 { get; set; }
            public int skill_10_19 { get; set; }
            public int skill_20_29 { get; set; }
            public int skill_30_39 { get; set; }
            public int skill_40_49 { get; set; }
            public int skill_50_59 { get; set; }
            public int skill_60_69 { get; set; }
            public int skill_70_79 { get; set; }
            public int skill_80_89 { get; set; }
            public int skill_90_99 { get; set; }
            public int skill_100 { get; set; }
            public bool HasSkill(int skill)
            {
                if (skill == skill_01_09 || skill == skill_10_19 || skill == skill_20_29 ||
                    skill == skill_30_39 || skill == skill_40_49 || skill == skill_50_59 ||
                    skill == skill_60_69 || skill == skill_70_79 || skill == skill_80_89 ||
                    skill == skill_90_99 || skill == skill_100)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public IEnumerable<int> Skills
            {
                get
                {
                    yield return skill_01_09;
                    yield return skill_10_19;
                    yield return skill_20_29;
                    yield return skill_30_39;
                    yield return skill_40_49;
                    yield return skill_50_59;
                    yield return skill_60_69;
                    yield return skill_70_79;
                    yield return skill_80_89;
                    yield return skill_90_99;
                    yield return skill_100;
                }
            }
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
                FillPartySkillInfo(id);
            }
            else if (SkillTypeRadio_Active.IsChecked == true)
            {
                FillActiveSkillInfo(id);
            }
            else if (SkillTypeRadio_Panel.IsChecked == true)
            {
                FillPanelSkillInfo(id);
            }
            else if (SkillTypeRadio_Limit.IsChecked == true)
            {
                FillLimitSkillInfo(id);
            }
            else
            {
                return;
            }
        }
        private void FillPartySkillInfo(string skillId)
        {
            Task<PartySkillMaster> task = new Task<PartySkillMaster>(() =>
            {
                return DAL.ToSingle<PartySkillMaster>(string.Format("select * from party_skill_master where id={0}", skillId));
            });
            Task<List<SkillUnitRank>> taskSkillUnitRank = new Task<List<SkillUnitRank>>(() =>
            {
                return DAL.ToList<SkillUnitRank>(@"Select um.id,um.g_id,um.name,
IFNULL(skill_01_09,0) as skill_01_09,IFNULL(skill_10_19,0) as skill_10_19,
IFNULL(skill_20_29,0) as skill_20_29,IFNULL(skill_30_39,0) as skill_30_39,IFNULL(skill_40_49,0) as skill_40_49,
IFNULL(skill_50_59,0) as skill_50_59,IFNULL(skill_60_69,0) as skill_60_69,IFNULL(skill_70_79,0) as skill_70_79,
IFNULL(skill_80_89,0) as skill_80_89,IFNULL(skill_90_99,0) as skill_90_99,IFNULL(skill_100,0) as skill_100
        From Unit_master as um
        LEFT JOIN Party_skill_rank_master AS psrm on um.p_skill_id=psrm.id
        ORDER BY um.g_id");
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
                if (tUSR.Result == null || tUSR.Result.Count == 0)
                {
                    return;
                }
                PartySkillMaster skill = task.Result;

                partySkill_id.Text = skill.id.ToString();
                partySkill_name.Text = skill.name;
                partySkill_text.Document = Utility.ParseTextToDocument(skill.text);
                partySkill_attribute.Text = Utility.ParseAttributetype(skill.attribute);
                partySkill_sub_attr.Text = Utility.ParseAttributetype(skill.sub_attr);
                partySkill_style.Text = Utility.ParseStyletype(skill.style);
                partySkill_type_id.Text = skill.type.ToString();
                partySkill_type.Text = Utility.ParseSkillType((PassiveSkillType)skill.type);
                partySkill_num.Text = skill.num.ToString();
                partySkill_num_01.Text = skill.num_01.ToString();
                partySkill_num_02.Text = skill.num_02.ToString();
                partySkill_num_03.Text = skill.num_03.ToString();

                SetSkillUnitRankInfo(tUSR.Result, Convert.ToInt32(skillId));

            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskSkillUnitRank.Start();
        }
        private void FillActiveSkillInfo(string skillId)
        {
            Task<ActiveSkillMaster> task = new Task<ActiveSkillMaster>(() =>
            {
                return DAL.ToSingle<ActiveSkillMaster>(string.Format("select * from active_skill_master where id={0}", skillId));
            });
            Task<List<SkillUnitRank>> taskSkillUnitRank = new Task<List<SkillUnitRank>>(() =>
            {
                return DAL.ToList<SkillUnitRank>(@"Select um.id,um.g_id,um.name,
IFNULL(skill_01_09,0) as skill_01_09,IFNULL(skill_10_19,0) as skill_10_19,
IFNULL(skill_20_29,0) as skill_20_29,IFNULL(skill_30_39,0) as skill_30_39,IFNULL(skill_40_49,0) as skill_40_49,
IFNULL(skill_50_59,0) as skill_50_59,IFNULL(skill_60_69,0) as skill_60_69,IFNULL(skill_70_79,0) as skill_70_79,
IFNULL(skill_80_89,0) as skill_80_89,IFNULL(skill_90_99,0) as skill_90_99,IFNULL(skill_100,0) as skill_100
        From Unit_master as um
        LEFT JOIN active_skill_rank_master AS srm on um.a_skill_id=srm.id
        ORDER BY um.g_id");
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
                if (tUSR.Result == null || tUSR.Result.Count == 0)
                {
                    return;
                }
                ActiveSkillMaster skill = task.Result;

                activeSkill_id.Text = skill.id.ToString();
                activeSkill_name.Text = skill.name;
                activeSkill_text.Document = Utility.ParseTextToDocument(skill.text);
                activeSkill_attribute.Text = Utility.ParseAttributetype(skill.attribute);
                activeSkill_sub_attr.Text = Utility.ParseAttributetype(skill.sub_attr);
                activeSkill_style.Text = Utility.ParseStyletype(skill.style);
                activeSkill_type.Text = Utility.ParseSkillType((ActiveSkillType)skill.type);
                activeSkill_type_id.Text = skill.type.ToString();
                activeSkill_num.Text = skill.num.ToString();
                activeSkill_num_01.Text = skill.num_01.ToString();
                activeSkill_num_02.Text = skill.num_02.ToString();
                activeSkill_num_03.Text = skill.num_03.ToString();
                activeSkill_soul.Text = skill.soul.ToString();
                activeSkill_phase.Text = ((SkillPhase)skill.phase).ToString();
                activeSkill_limit_num.Text = skill.limit_num.ToString();

                SetSkillUnitRankInfo(tUSR.Result, Convert.ToInt32(skillId));

            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskSkillUnitRank.Start();
        }
        private void FillPanelSkillInfo(string skillId)
        {
            Task<PanelSkillMaster> task = new Task<PanelSkillMaster>(() =>
            {
                return DAL.ToSingle<PanelSkillMaster>(string.Format("select * from panel_skill_master where id={0}", skillId));
            });
            Task<List<SkillUnitRank>> taskSkillUnitRank = new Task<List<SkillUnitRank>>(() =>
            {
                return DAL.ToList<SkillUnitRank>(@"Select um.id,um.g_id,um.name,
IFNULL(skill_01_09,0) as skill_01_09,IFNULL(skill_10_19,0) as skill_10_19,
IFNULL(skill_20_29,0) as skill_20_29,IFNULL(skill_30_39,0) as skill_30_39,IFNULL(skill_40_49,0) as skill_40_49,
IFNULL(skill_50_59,0) as skill_50_59,IFNULL(skill_60_69,0) as skill_60_69,IFNULL(skill_70_79,0) as skill_70_79,
IFNULL(skill_80_89,0) as skill_80_89,IFNULL(skill_90_99,0) as skill_90_99,IFNULL(skill_100,0) as skill_100
        From Unit_master as um
        LEFT JOIN panel_skill_rank_master AS srm on um.panel_skill_id=srm.id
        ORDER BY um.g_id");
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
                if (tUSR.Result == null || tUSR.Result.Count == 0)
                {
                    return;
                }
                PanelSkillMaster skill = task.Result;

                panelSkill_id.Text = skill.id.ToString();
                panelSkill_name.Text = skill.name;
                panelSkill_text.Document = Utility.ParseTextToDocument(skill.text);
                panelSkill_attribute.Text = Utility.ParseAttributetype(skill.attribute);
                panelSkill_style.Text = Utility.ParseStyletype(skill.style);
                panelSkill_type.Text = Utility.ParseSkillType((PanelSkillType)skill.type);
                panelSkill_type_id.Text = skill.type.ToString();
                panelSkill_num.Text = skill.num.ToString();
                panelSkill_num_01.Text = skill.num_01.ToString();
                panelSkill_num_02.Text = skill.num_02.ToString();
                panelSkill_num_03.Text = skill.num_03.ToString();
                panelSkill_phase.Text = ((SkillPhase)skill.phase).ToString();
                panelSkill_duplication.Text = skill.duplication == 1 ? true.ToString() : skill.duplication == 2 ? false.ToString() : String.Empty;

                SetSkillUnitRankInfo(tUSR.Result, Convert.ToInt32(skillId));

            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskSkillUnitRank.Start();
        }
        private void FillLimitSkillInfo(string skillId)
        {
            Task<Skills> task = new Task<Skills>(() =>
            {
                return Skills.FromLimitSkillId(Convert.ToInt32(skillId));
            });
            Task<List<SkillUnitRank>> taskSkillUnitRank = new Task<List<SkillUnitRank>>(() =>
            {
                return DAL.ToList<SkillUnitRank>(@"Select um.id,um.g_id,um.name,
IFNULL(skill_01_09,0) as skill_01_09,IFNULL(skill_10_19,0) as skill_10_19,
IFNULL(skill_20_29,0) as skill_20_29,IFNULL(skill_30_39,0) as skill_30_39,IFNULL(skill_40_49,0) as skill_40_49,
IFNULL(skill_50_59,0) as skill_50_59,IFNULL(skill_60_69,0) as skill_60_69,IFNULL(skill_70_79,0) as skill_70_79,
IFNULL(skill_80_89,0) as skill_80_89,IFNULL(skill_90_99,0) as skill_90_99,IFNULL(skill_100,0) as skill_100
        From Unit_master as um
        LEFT JOIN limit_skill_rank_master AS srm on um.limit_skill_id=srm.id
        ORDER BY um.g_id");
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
                if (tUSR.Result == null || tUSR.Result.Count == 0)
                {
                    return;
                }
                LimitSkillMaster skill = task.Result.limitSkill;

                limitSkill_id.Text = skill.id.ToString();
                limitSkill_name.Text = skill.name;
                limitSkill_general_text.Document = Utility.ParseTextToDocument(skill.general_text);
                limitSkill_coefficient.Text = skill.coefficient.ToString();

                SkillInfo_LimitSkill_AS.Children.Clear();
                for (int i = 0; i < 3; i++)
                {
                    ActiveSkillMaster askill = task.Result.limitActiveSkill[i];
                    Grid grid = new Grid();
                    if (askill.id == 0)
                    {
                        continue;
                    }
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    TextBlock tblTitle = new TextBlock() { FontWeight = FontWeights.Bold, Text = string.Format("L_AS{0}", i.ToString()) };
                    tblTitle.SetValue(Grid.ColumnProperty, 0);
                    tblTitle.SetValue(Grid.RowProperty, 0);
                    grid.Children.Add(tblTitle);
                    TextBox tbId = new TextBox() { Text = askill.id.ToString() };
                    tbId.SetValue(Grid.ColumnProperty, 1);
                    tbId.SetValue(Grid.RowProperty, 0);
                    grid.Children.Add(tbId);
                    TextBox tbName = new TextBox() { Text = askill.name };
                    tbName.SetValue(Grid.ColumnProperty, 2);
                    tbName.SetValue(Grid.RowProperty, 0);
                    tbName.SetValue(Grid.ColumnSpanProperty, 2);
                    grid.Children.Add(tbName);
                    RichTextBox rtb = new RichTextBox() { Document = Utility.ParseTextToDocument(askill.text) };
                    rtb.SetValue(Grid.ColumnProperty, 0);
                    rtb.SetValue(Grid.RowProperty, 1);
                    rtb.SetValue(Grid.ColumnSpanProperty, 4);
                    grid.Children.Add(rtb);
                    TextBlock tblType = new TextBlock() { Text = "type" };
                    tblType.SetValue(Grid.ColumnProperty, 0);
                    tblType.SetValue(Grid.RowProperty, 2);
                    grid.Children.Add(tblType);
                    TextBox tbTypeId = new TextBox() { Text = askill.type.ToString() };
                    tbTypeId.SetValue(Grid.ColumnProperty, 1);
                    tbTypeId.SetValue(Grid.RowProperty, 2);
                    grid.Children.Add(tbTypeId);
                    TextBox tbType = new TextBox() { Text = Utility.ParseSkillType((ActiveSkillType)askill.type) };
                    tbType.SetValue(Grid.ColumnProperty, 2);
                    tbType.SetValue(Grid.RowProperty, 2);
                    tbType.SetValue(Grid.ColumnSpanProperty, 2);
                    grid.Children.Add(tbType);
                    TextBlock tblAttr = new TextBlock() { Text = "attribute" };
                    tblAttr.SetValue(Grid.ColumnProperty, 0);
                    tblAttr.SetValue(Grid.RowProperty, 3);
                    grid.Children.Add(tblAttr);
                    TextBox tbAttr = new TextBox() { Text = Utility.ParseAttributetype(askill.attribute) };
                    tbAttr.SetValue(Grid.ColumnProperty, 1);
                    tbAttr.SetValue(Grid.RowProperty, 3);
                    grid.Children.Add(tbAttr);
                    TextBox tbSubAttr = new TextBox() { Text = Utility.ParseAttributetype(askill.sub_attr) };
                    tbSubAttr.SetValue(Grid.ColumnProperty, 2);
                    tbSubAttr.SetValue(Grid.RowProperty, 3);
                    grid.Children.Add(tbSubAttr);
                    //gridStyle
                    Grid gridStyle = new Grid();
                    gridStyle.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    gridStyle.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    TextBlock tblStyle = new TextBlock() { Text = "style" };
                    tblStyle.SetValue(Grid.ColumnProperty, 0);
                    gridStyle.Children.Add(tblStyle);
                    TextBox tbStyle = new TextBox() { Text = Utility.ParseStyletype(askill.style) };
                    tbStyle.SetValue(Grid.ColumnProperty, 1);
                    gridStyle.Children.Add(tbStyle);
                    gridStyle.SetValue(Grid.ColumnProperty, 3);
                    gridStyle.SetValue(Grid.RowProperty, 3);
                    grid.Children.Add(gridStyle);
                    //gridInfo
                    Grid gridInfo = new Grid();
                    gridInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    TextBlock tblPhase = new TextBlock() { Text = "phase" };
                    tblPhase.SetValue(Grid.ColumnProperty, 0);
                    gridInfo.Children.Add(tblPhase);
                    TextBox tbPhase = new TextBox() { Text = ((SkillPhase)askill.phase).ToString() };
                    tbPhase.SetValue(Grid.ColumnProperty, 1);
                    gridInfo.Children.Add(tbPhase);
                    TextBlock tblSoul = new TextBlock() { Text = "soul" };
                    tblSoul.SetValue(Grid.ColumnProperty, 2);
                    gridInfo.Children.Add(tblSoul);
                    TextBox tbSoul = new TextBox() { Text = askill.soul.ToString() };
                    tbSoul.SetValue(Grid.ColumnProperty, 3);
                    gridInfo.Children.Add(tbSoul);
                    TextBlock tblLimitNum = new TextBlock() { Text = "limit_num" };
                    tblLimitNum.SetValue(Grid.ColumnProperty, 4);
                    gridInfo.Children.Add(tblLimitNum);
                    TextBox tbLimitNum = new TextBox() { Text = askill.limit_num.ToString() };
                    tbLimitNum.SetValue(Grid.ColumnProperty, 5);
                    gridInfo.Children.Add(tbLimitNum);
                    gridInfo.SetValue(Grid.ColumnSpanProperty, 4);
                    gridInfo.SetValue(Grid.RowProperty, 4);
                    grid.Children.Add(gridInfo);
                    //gridNum
                    Grid gridNum = new Grid();
                    gridNum.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridNum.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridNum.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    gridNum.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    TextBox tbNum = new TextBox() { Text = askill.num.ToString() };
                    tbNum.SetValue(Grid.ColumnProperty, 0);
                    gridNum.Children.Add(tbNum);
                    TextBox tbNum01 = new TextBox() { Text = askill.num_01.ToString() };
                    tbNum01.SetValue(Grid.ColumnProperty, 1);
                    gridNum.Children.Add(tbNum01);
                    TextBox tbNum02 = new TextBox() { Text = askill.num_02.ToString() };
                    tbNum02.SetValue(Grid.ColumnProperty, 2);
                    gridNum.Children.Add(tbNum02);
                    TextBox tbNum03 = new TextBox() { Text = askill.num_03.ToString() };
                    tbNum03.SetValue(Grid.ColumnProperty, 3);
                    gridNum.Children.Add(tbNum03);
                    gridNum.SetValue(Grid.ColumnSpanProperty, 4);
                    gridNum.SetValue(Grid.RowProperty, 5);
                    grid.Children.Add(gridNum);
                    SkillInfo_LimitSkill_AS.Children.Add(grid);
                }

                SetSkillUnitRankInfo(tUSR.Result, Convert.ToInt32(skillId));

            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskSkillUnitRank.Start();
        }
        private void SetSkillUnitRankInfo(List<SkillUnitRank> surList, int skillId)
        {
            SkillUnitRankInfo.Children.Clear();
            SkillUnitRankInfo.RowDefinitions.Clear();
            int row = 0;
            foreach (SkillUnitRank sur in surList)
            {
                if (sur.HasSkill(skillId))
                {
                    SkillUnitRankInfo.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    var tbl = new TextBlock()
                    {
                        Text = sur.g_id.ToString()
                    };
                    tbl.SetValue(Grid.ColumnProperty, 0);
                    tbl.SetValue(Grid.RowProperty, row);
                    SkillUnitRankInfo.Children.Add(tbl);
                    var tb = new TextBox()
                    {
                        Tag = sur.id.ToString(),
                        Text = sur.name
                    };
                    tb.MouseDoubleClick += tb_MouseDoubleClick;
                    tb.SetValue(Grid.ColumnProperty, 1);
                    tb.SetValue(Grid.RowProperty, row);
                    SkillUnitRankInfo.Children.Add(tb);
                    int col = 2;
                    foreach (int skill in sur.Skills)
                    {
                        var rec = new Rectangle()
                        {
                            Fill = skill == skillId ? (SolidColorBrush)Application.Current.Resources["DefaultBrush"] : Brushes.Transparent,
                            Stroke = (SolidColorBrush)Application.Current.Resources["PressedBrush"],
                            StrokeThickness = 0.5
                        };
                        rec.SetValue(Grid.ColumnProperty, col);
                        rec.SetValue(Grid.RowProperty, row);
                        SkillUnitRankInfo.Children.Add(rec);
                        col++;
                    }
                    row++;
                }
            }
        }
        async private void tb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var unit = (Unit)await Utility.GetTabByName("Unit");
                string id = (sender as TextBox).Tag.ToString();
                unit.SelectUnitById(Convert.ToInt32(id));
                Utility.ChangeTab("Unit");
            }
        }
        private Dictionary<string, string> GetSkillTypeDict<T>() where T : struct , IConvertible
        {
            var typeDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (T type in Enum.GetValues(typeof(T)))
            {
                string id = (Convert.ToInt32(type)).ToString();
                typeDict.Add(string.Format("{0}_{1}", id, type.ToString()), id);
            }
            return typeDict;
        }
        private void SkillTypeRadio_Party_Checked(object sender, RoutedEventArgs e)
        {
            SkillInfo_PartySkill.Visibility = Visibility.Visible;
            SkillInfo_ActiveSkill.Visibility = Visibility.Collapsed;
            SkillInfo_PanelSkill.Visibility = Visibility.Collapsed;
            SkillInfo_LimitSkill.Visibility = Visibility.Collapsed;
            SkillUnitRankInfo.Children.Clear();
            Utility.BindData(SkillDataGrid, "select id,type,name from party_skill_master order by type,id");
            SkillSearch_type.ItemsSource = GetSkillTypeDict<PassiveSkillType>();
            SkillSearch_type.IsEnabled = true;
        }
        private void SkillTypeRadio_Active_Checked(object sender, RoutedEventArgs e)
        {
            SkillInfo_PartySkill.Visibility = Visibility.Collapsed;
            SkillInfo_ActiveSkill.Visibility = Visibility.Visible;
            SkillInfo_PanelSkill.Visibility = Visibility.Collapsed;
            SkillInfo_LimitSkill.Visibility = Visibility.Collapsed;
            SkillUnitRankInfo.Children.Clear();
            Utility.BindData(SkillDataGrid, "select id,type,name from active_skill_master order by type,id");
            SkillSearch_type.ItemsSource = GetSkillTypeDict<ActiveSkillType>();
            SkillSearch_type.IsEnabled = true;
        }
        private void SkillTypeRadio_Panel_Checked(object sender, RoutedEventArgs e)
        {
            SkillInfo_PartySkill.Visibility = Visibility.Collapsed;
            SkillInfo_ActiveSkill.Visibility = Visibility.Collapsed;
            SkillInfo_PanelSkill.Visibility = Visibility.Visible;
            SkillInfo_LimitSkill.Visibility = Visibility.Collapsed;
            SkillUnitRankInfo.Children.Clear();
            Utility.BindData(SkillDataGrid, "select id,type,name from panel_skill_master order by type,id");
            SkillSearch_type.ItemsSource = GetSkillTypeDict<PanelSkillType>();
            SkillSearch_type.IsEnabled = true;
        }
        private void SkillTypeRadio_Limit_Checked(object sender, RoutedEventArgs e)
        {
            SkillInfo_PartySkill.Visibility = Visibility.Collapsed;
            SkillInfo_ActiveSkill.Visibility = Visibility.Collapsed;
            SkillInfo_PanelSkill.Visibility = Visibility.Collapsed;
            SkillInfo_LimitSkill.Visibility = Visibility.Visible;
            SkillUnitRankInfo.Children.Clear();
            Utility.BindData(SkillDataGrid, "select id,name from limit_skill_master order by id");
            //SkillSearch_type.ItemsSource = GetSkillTypeDict<PanelSkillType>();
            //Limit skill has no type
            SkillSearch_type.IsEnabled = false;
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
            else if (SkillTypeRadio_Limit.IsChecked == true)
            {
                tableName = "limit_skill_master";
            }
            return tableName;
        }

        private void SkillSearchClear_Click(object sender, RoutedEventArgs e)
        {
            SkillSearch_name.Text = string.Empty;
            SkillSearch_type.Text = string.Empty;
            SkillSearch_attribute.SelectedIndex = 0;
            //SkillSearch_sub_attr.SelectedIndex = 0;
            if (SkillTypeRadio_Limit.IsChecked == true)
            {
                Utility.BindData(SkillDataGrid, "select id,name from " + GetSkillTableByRadioChecked() + " order by id");
            }
            else
            {
                Utility.BindData(SkillDataGrid, "select id,type,name from " + GetSkillTableByRadioChecked() + " order by type,id");
            }
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
            StringBuilder sb = new StringBuilder();
            if (SkillTypeRadio_Limit.IsChecked == true)
            {
                sb.AppendFormat("select id,name from {0} WHERE ", GetSkillTableByRadioChecked());
            }
            else
            {
                sb.AppendFormat("select id,type,name from {0} WHERE ", GetSkillTableByRadioChecked());
            }
            if (String.IsNullOrWhiteSpace(SkillSearch_name.Text) == false)
            {
                sb.AppendFormat("name LIKE '%{0}%' AND ", SkillSearch_name.Text.Trim());
            }
            if (String.IsNullOrWhiteSpace((string)SkillSearch_type.SelectedValue) == false && SkillTypeRadio_Limit.IsChecked == false)
            {
                sb.AppendFormat("type={0} AND ", SkillSearch_type.SelectedValue.ToString());
            }
            if (String.IsNullOrWhiteSpace((string)SkillSearch_attribute.SelectedValue) == false)
            {
                sb.AppendFormat("attribute={0} AND ", SkillSearch_attribute.SelectedValue.ToString());
            }
            //if (String.IsNullOrWhiteSpace((string)SkillSearch_sub_attr.SelectedValue) == false)
            //{
            //    sql += "sub_attr=" + SkillSearch_sub_attr.SelectedValue.ToString() + " AND ";
            //}
            if (SkillTypeRadio_Limit.IsChecked == true)
            {
                sb.AppendLine(" 1=1 order by id");
            }
            else
            {
                sb.AppendLine(" 1=1 order by type,id");
            }
            return sb.ToString();
        }
    }
}
