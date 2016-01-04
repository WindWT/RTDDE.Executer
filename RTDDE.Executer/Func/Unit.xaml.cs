using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;
using RTDDE.Executer.Util;

namespace RTDDE.Executer.Func
{
    public partial class Unit : UserControl, IRedirectable
    {
        public Unit()
        {
            InitializeComponent();
        }
        private const string UnitSql = "SELECT id,g_id,name,attribute,sub_a1 FROM UNIT_MASTER order by g_id";
        private void UnitTab_Initialized(object sender, EventArgs e)
        {
            Utility.DisableBindData = true;
            UnitSearch_category.ItemsSource = new Dictionary<string, string>()
            {
                {"------",""},
                {"★","1"},
                {"★★","2"},
                {"★★★","3"},
                {"★★★★","4"},
                {"★★★★★","5"},
                {"★×6","6"}
            };
            UnitSearch_style.ItemsSource = new Dictionary<string, string>()
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
            UnitSearch_attribute.ItemsSource = attrDict;
            UnitSearch_sub_a1.ItemsSource = attrDict;
            var kindDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (AssignID kind in Enum.GetValues(typeof(AssignID))) {
                if (kind.ToString().StartsWith("MS0") == false) {
                    string id = Utility.ParseAssignID(kind).ToString();
                    kindDict.Add(string.Format("{0}_{1}", id, kind.ToString()), id);
                }
            }
            UnitSearch_kind.ItemsSource = kindDict;
            Utility.DisableBindData = false;
            Utility.BindData(UnitDataGrid, UnitSql);
        }

        private void UnitDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string unitid = ((DataRowView)UnitDataGrid.SelectedItem).Row["id"].ToString();
            UnitInfo_id.Text = unitid;

            if (Settings.Config.General.IsDefaultLvMax) {
                UnitInfo_lv.Text = "99";
            }
            else {
                UnitInfo_lv.Text = "1";
            }

            UnitInfo_BindData(unitid);
        }
        private void UnitInfo_lv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UnitInfo_lv.Text)) {
                UnitInfo_lv.Text = "";
            }
            Regex r = new Regex("[^0-9]");
            if (r.Match(UnitInfo_lv.Text).Success) {
                if (Settings.Config.General.IsDefaultLvMax) {
                    UnitInfo_lv.Text = "99";     //This will trigger itself again
                    return;
                }
                else {
                    UnitInfo_lv.Text = "1";
                    return;
                }
            }
            if (string.IsNullOrWhiteSpace(UnitInfo_id.Text)) {
                return;
            }
            UnitInfo_BindData(UnitInfo_id.Text);
        }

        public void UnitInfo_BindData(string unitid)
        {
            if (string.IsNullOrWhiteSpace(UnitInfo_lv.Text)) {
                return;
            }
            double thislevel = Convert.ToDouble(Convert.ToInt32(UnitInfo_lv.Text));

            Task<UnitInfo> task = new Task<UnitInfo>(() =>
            {
                string sql = @"
Select *,
IFNULL((SELECT g_id FROM UNIT_MASTER as ui WHERE uo.rev_unit_id=ui.id),0) as rev_unit_g_id,
IFNULL((SELECT NAME FROM UNIT_MASTER as ui WHERE uo.rev_unit_id=ui.id),'') as rev_unit_name,
IFNULL((SELECT g_id FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_fire=ui.id),0) as ultimate_rev_unit_g_id_fire,
IFNULL((SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_fire=ui.id),'') as ultimate_rev_unit_name_fire,
IFNULL((SELECT g_id FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_water=ui.id),0) as ultimate_rev_unit_g_id_water,
IFNULL((SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_water=ui.id),'') as ultimate_rev_unit_name_water,
IFNULL((SELECT g_id FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_shine=ui.id),0) as ultimate_rev_unit_g_id_shine,
IFNULL((SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_shine=ui.id),'') as ultimate_rev_unit_name_shine,
IFNULL((SELECT g_id FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_dark=ui.id),0) as ultimate_rev_unit_g_id_dark,
IFNULL((SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_dark=ui.id),'') as ultimate_rev_unit_name_dark
from unit_master as uo
WHERE uo.id={0}";
                return DAL.ToSingle<UnitInfo>(String.Format(sql, unitid));
            });
            Task<Skills> taskSkills = new Task<Skills>(() =>
            {
                Task.WaitAll(task);
                if (task.Result == null) {
                    return null;
                }
                UnitInfo ui = task.Result;

                double maxlevel = ui.lv_max;
                if (Settings.Config.General.IsEnableLevelLimiter && (thislevel > maxlevel)) {
                    thislevel = maxlevel;
                }

                return new Skills(ui.p_skill_id, ui.a_skill_id, ui.panel_skill_id, ui.limit_skill_id, (int)thislevel);
            });
            Task<AccessoryMaster> taskAccessory = new Task<AccessoryMaster>(() =>
            {
                string sql = @"SELECT * FROM accessory_master WHERE id={0}";
                return DAL.ToSingle<AccessoryMaster>(String.Format(sql, unitid));
            });
            taskSkills.ContinueWith(t =>
            {
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception);
                    return;
                }
                if (task.Result == null) {
                    return;
                }
                UnitInfo ui = task.Result;
                UnitInfo_g_id.Text = ui.g_id.ToString();
                UnitInfo_name.Text = ui.name;
                string rare = string.Empty;
                for (int i = 0; i < ui.category; i++) {
                    rare += "★";
                }
                UnitInfo_category.Text = rare;
                UnitInfo_style.Text = Utility.ParseStyletype(ui.style);
                UnitInfo_attribute.Text = Utility.ParseAttributeToString(ui.attribute);
                UnitInfo_sub_a1.Text = Utility.ParseAttributeToString(ui.sub_a1);
                UnitInfo_kind.Text = Utility.ParseUnitKind(ui.kind).ToString();
                UnitInfo_bonus_limit_base.Text = ui.bonus_limit_base.ToString();

                //Rev
                UnitRevStackPanel.Children.Clear();
                UnitRevStackPanel.Children.Add(CreateRevGrid(ui.rev_unit_id, ui.need_pt, ui.max_attribute_exp,
                    ui.rev_unit_name, "Rev"));
                UnitRevStackPanel.Children.Add(CreateRevGrid(ui.ultimate_rev_unit_id_fire, ui.need_pt,
                    ui.max_attribute_exp, ui.ultimate_rev_unit_name_fire, "U.Fire"));
                UnitRevStackPanel.Children.Add(CreateRevGrid(ui.ultimate_rev_unit_id_water, ui.need_pt,
                    ui.max_attribute_exp, ui.ultimate_rev_unit_name_water, "U.Water"));
                UnitRevStackPanel.Children.Add(CreateRevGrid(ui.ultimate_rev_unit_id_shine, ui.need_pt,
                    ui.max_attribute_exp, ui.ultimate_rev_unit_name_shine, "U.Shine"));
                UnitRevStackPanel.Children.Add(CreateRevGrid(ui.ultimate_rev_unit_id_dark, ui.need_pt,
                    ui.max_attribute_exp, ui.ultimate_rev_unit_name_dark, "U.Dark"));

                double maxlevel = ui.lv_max;
                UnitInfo_lv_max.Text = maxlevel.ToString("0");
                UnitInfo_lv.Text = thislevel.ToString("0");

                //calc hp atk heal
                double y = UnitParam_CategoryPer[ui.category - 1];
                int num = (ui.style - 1) * 3;
                double y2 = UnitParam_StylePer[num];
                double y3 = UnitParam_StylePer[num + 1];
                double y4 = UnitParam_StylePer[num + 2];

                double thishp = (int)(Math.Pow(Math.Pow(thislevel, y), y2) * Convert.ToDouble(ui.life));
                double thisattack = (int)(Math.Pow(Math.Pow(thislevel, y), y3) * Convert.ToDouble(ui.attack));
                double thisheal = (int)(Math.Pow(Math.Pow(thislevel, y), y4) * Convert.ToDouble(ui.heal));
                UnitInfo_HP.Text = thishp.ToString("0");
                UnitInfo_ATK.Text = thisattack.ToString("0");
                UnitInfo_HEAL.Text = thisheal.ToString("0");

                //calc exp pt
                int num2 = ui.category - 1;
                float expThisLevel = (float)(Math.Pow(thislevel - 1, Convert.ToDouble(UnitParam_NextExp_Per[num2])) * (double)(Convert.ToSingle(ui.grow)));
                float expNextLevel = (float)(Math.Pow(thislevel, Convert.ToDouble(UnitParam_NextExp_Per[num2])) * (double)(Convert.ToSingle(ui.grow)));
                UnitInfo_EXP.Text = expThisLevel.ToString("0");
                UnitInfo_EXP_Next.Text = Math.Abs(thislevel - maxlevel) < 0.1 ? "MAX" : expNextLevel.ToString("0");
                UnitInfo_cost.Text = (ui.cost * thislevel).ToString("0");
                UnitInfo_sale.Text = (ui.sale * thislevel).ToString("0");
                UnitInfo_mix.Text = (ui.mix * thislevel).ToString("0");

                int set_pt = ui.set_pt;
                UnitInfo_pt.Text = ((int)((float)(thislevel - 1) * Math.Pow((float)ui.set_pt, 0.5f) + (float)set_pt)).ToString("0");
                //story
                var storyDoc = Utility.ParseTextToDocument(ui.story, Settings.Config.General.IsForceWrapInStory ? 43 : 0);
                storyDoc.TextAlignment = TextAlignment.Center;
                UnitInfo_story.Padding = new Thickness(37, 0, 37, 0);
                UnitInfo_story.Document = storyDoc;
                //cutin
                UnitInfo_ct_text.Text = ui.ct_text;
                UnitInfo_sct_text.Text = ui.sct_text;
                UnitInfo_sct6_text.Text = ui.sct6_text;
                UnitInfo_a_skill_text.Text = ui.a_skill_text;
                //skill
                if (ui.p_skill_id == 0 && ui.a_skill_id == 0 && ui.panel_skill_id == 0 && ui.limit_skill_id == 0) {
                    UnitSkillExpander.Visibility = Visibility.Collapsed;
                }
                else {
                    UnitSkillExpander.Visibility = Visibility.Visible;
                    partySkill_BindData(t.Result.partySkill);
                    activeSkill_BindData(t.Result.activeSkill);
                    panelSkill_BindData(t.Result.panelSkill);
                    limitSkill_BindData(t.Result.limitSkill, t.Result.limitActiveSkill);
                }
                //Accessory
                Task.WaitAll(taskAccessory);
                if (taskAccessory.Result == null) {
                    UnitAccessoryExpander.Visibility = Visibility.Collapsed;
                }
                else {
                    UnitAccessoryExpander.Visibility = Visibility.Visible;
                    AccessoryMaster acce = taskAccessory.Result;
                    accessory_id.Text = acce.id.ToString();
                    accessory_name.Text = acce.name;
                    accessory_type.Text = Utility.ParseAccessoryType(acce.type);
                    accessory_detail.Document = Utility.ParseTextToDocument(acce.detail);
                    accessory_num_01.Text = acce.num_01.ToString();
                    accessory_num_02.Text = acce.num_02.ToString();
                    accessory_num_03.Text = acce.num_03.ToString();
                    accessory_num_04.Text = acce.num_04.ToString();
                    accessory_conv_money.Text = acce.conv_money.ToString();
                    accessory_style.Text = Utility.ParseStyletype(acce.style);
                    accessory_attribute.Text = Utility.ParseAttributeToString(acce.attribute);
                    accessory_su_a1.Text = acce.su_a1.ToString();  //not sub attribute
                }
                //Advanced
                UnitInfo_ui_id.Text = ui.ui_id.ToString();
                UnitInfo_flag_no.Text = ui.flag_no.ToString();
                UnitInfo_model.Text = ui.model;
                UnitInfo_sub_c1.Text = ui.sub_c1.ToString();
                UnitInfo_sub_c2.Text = ui.sub_c2.ToString();
                UnitInfo_sub_c3.Text = ui.sub_c3.ToString();
                UnitInfo_sub_c4.Text = ui.sub_c4.ToString();
                UnitInfo_shadow.Text = ui.shadow.ToString();
                UnitInfo_ap_rec_val.Text = ui.ap_rec_val.ToString();
                UnitInfo_yorisiro.Text = ui.yorisiro.ToString();
                UnitInfo_present.Text = ui.present.ToString();
                UnitInfo_material_type.Text = ui.material_type.ToString();

            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskSkills.Start();
            taskAccessory.Start();
        }

        private Grid CreateRevGrid(int id, int pt, int revExp, string name, string type) {
            Grid grid = new Grid();
            if (id == 0) {
                return grid;
            }
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            TextBlock tbRev = new TextBlock() { Text = type };
            tbRev.SetValue(Grid.ColumnProperty, 0);
            grid.Children.Add(tbRev);
            TextBox tbPt = new TextBox() { Text = pt.ToString() };
            tbPt.SetValue(Grid.ColumnProperty, 1);
            grid.Children.Add(tbPt);
            TextBox tbgId = new TextBox() { Text = revExp.ToString() };
            tbgId.SetValue(Grid.ColumnProperty, 2);
            grid.Children.Add(tbgId);
            TextBox tbId = new TextBox() { Text = id.ToString() };
            tbId.SetValue(Grid.ColumnProperty, 3);
            grid.Children.Add(tbId);
            TextBox tbName = new TextBox() { Text = name };
            tbName.SetValue(Grid.ColumnProperty, 4);
            grid.Children.Add(tbName);
            Button button = new Button() {
                Content = "→",
                Style = FindResource("InlineButton") as Style
            };
            button.Click += (s, arg) => {
                Utility.GoToItemById<Unit>(id);
            };
            button.SetValue(Grid.ColumnProperty, 5);
            grid.Children.Add(button);
            return grid;
        }

        private void partySkill_BindData(PartySkillMaster skill)
        {
            if (skill == null || skill.id == 0) {
                UnitInfo_PartySkill.Visibility = Visibility.Collapsed;
                return;
            }
            UnitInfo_PartySkill.Visibility = Visibility.Visible;

            partySkill_id.Text = skill.id.ToString();
            partySkill_name.Text = skill.name;
            partySkill_text.Document = Utility.ParseTextToDocument(skill.text);
            partySkill_attribute.Text = Utility.ParseAttributeToString(skill.attribute);
            partySkill_sub_attr.Text = Utility.ParseAttributeToString(skill.sub_attr);
            partySkill_style.Text = Utility.ParseStyletype(skill.style);
            partySkill_type_id.Text = skill.type.ToString();
            partySkill_type.Text = Utility.ParseSkillType((PassiveSkillType)skill.type);
            partySkill_num.Text = skill.num.ToString();
            partySkill_num_01.Text = skill.num_01.ToString();
            partySkill_num_02.Text = skill.num_02.ToString();
            partySkill_num_03.Text = skill.num_03.ToString();
            partySkill_num_04.Text = skill.num_04.ToString();
            partySkill_num_05.Text = skill.num_05.ToString();
        }
        private void activeSkill_BindData(ActiveSkillMaster skill)
        {
            if (skill == null || skill.id == 0) {
                UnitInfo_ActiveSkill.Visibility = Visibility.Collapsed;
                return;
            }
            UnitInfo_ActiveSkill.Visibility = Visibility.Visible;

            activeSkill_id.Text = skill.id.ToString();
            activeSkill_name.Text = skill.name;
            activeSkill_text.Document = Utility.ParseTextToDocument(skill.text);
            activeSkill_attribute.Text = Utility.ParseAttributeToString(skill.attribute);
            activeSkill_sub_attr.Text = Utility.ParseAttributeToString(skill.sub_attr);
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
        }
        private void panelSkill_BindData(PanelSkillMaster skill)
        {
            if (skill == null || skill.id == 0) {
                UnitInfo_PanelSkill.Visibility = Visibility.Collapsed;
                return;
            }
            UnitInfo_PanelSkill.Visibility = Visibility.Visible;

            panelSkill_id.Text = skill.id.ToString();
            panelSkill_name.Text = skill.name;
            panelSkill_text.Document = Utility.ParseTextToDocument(skill.text);
            panelSkill_attribute.Text = Utility.ParseAttributeToString(skill.attribute);
            panelSkill_style.Text = Utility.ParseStyletype(skill.style);
            panelSkill_type.Text = Utility.ParseSkillType((PanelSkillType)skill.type);
            panelSkill_type_id.Text = skill.type.ToString();
            panelSkill_num.Text = skill.num.ToString();
            panelSkill_num_01.Text = skill.num_01.ToString();
            panelSkill_num_02.Text = skill.num_02.ToString();
            panelSkill_num_03.Text = skill.num_03.ToString();
            panelSkill_phase.Text = ((SkillPhase)skill.phase).ToString();
            panelSkill_duplication.Text = skill.duplication == 1 ? true.ToString() : skill.duplication == 2 ? false.ToString() : String.Empty;
        }
        private void limitSkill_BindData(LimitSkillMaster skill, ActiveSkillMaster[] laSkill)
        {
            if (skill == null || skill.id == 0) {
                UnitInfo_LimitSkill.Visibility = Visibility.Collapsed;
                return;
            }
            UnitInfo_LimitSkill.Visibility = Visibility.Visible;

            limitSkill_id.Text = skill.id.ToString();
            limitSkill_name.Text = skill.name;
            limitSkill_general_text.Document = Utility.ParseTextToDocument(skill.general_text);
            limitSkill_coefficient.Text = skill.coefficient.ToString();

            UnitInfo_LimitSkill_AS.Children.Clear();
            for (int i = 0; i < 3; i++) {
                ActiveSkillMaster askill = laSkill[i];
                Grid grid = new Grid();
                if (askill.id == 0) {
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
                TextBox tbAttr = new TextBox() { Text = Utility.ParseAttributeToString(askill.attribute) };
                tbAttr.SetValue(Grid.ColumnProperty, 1);
                tbAttr.SetValue(Grid.RowProperty, 3);
                grid.Children.Add(tbAttr);
                TextBox tbSubAttr = new TextBox() { Text = Utility.ParseAttributeToString(askill.sub_attr) };
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
                UnitInfo_LimitSkill_AS.Children.Add(grid);
            }
        }

        private void UnitSearchClear_Click(object sender, RoutedEventArgs e)
        {
            UnitSearch_id.Text = string.Empty;
            UnitSearch_g_id.Text = string.Empty;
            UnitSearch_name.Text = string.Empty;
            UnitSearch_story.Text = string.Empty;
            UnitSearch_category.SelectedIndex = 0;
            UnitSearch_style.SelectedIndex = 0;
            UnitSearch_kind.SelectedIndex = 0;
            UnitSearch_attribute.SelectedIndex = 0;
            UnitSearch_sub_a1.SelectedIndex = 0;
            Utility.BindData(UnitDataGrid, UnitSql);
        }
        private void UnitSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utility.BindData(UnitDataGrid, UnitSearch_BuildSQL());
        }
        private void UnitSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.BindData(UnitDataGrid, UnitSearch_BuildSQL());
        }
        private string UnitSearch_BuildSQL()
        {
            string sql = @"SELECT id,g_id,name,attribute,sub_a1 FROM UNIT_MASTER WHERE ";
            if (String.IsNullOrWhiteSpace(UnitSearch_id.Text) == false) {
                sql += "id=" + UnitSearch_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(UnitSearch_g_id.Text) == false) {
                sql += "g_id=" + UnitSearch_g_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(UnitSearch_name.Text) == false) {
                sql += "name LIKE '%" + UnitSearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace(UnitSearch_story.Text) == false) {
                sql += "story LIKE '%" + UnitSearch_story.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_category.SelectedValue) == false) {
                sql += "category=" + UnitSearch_category.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_kind.SelectedValue) == false) {
                sql += "kind=" + UnitSearch_kind.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_style.SelectedValue) == false) {
                sql += "style=" + UnitSearch_style.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_attribute.SelectedValue) == false) {
                sql += "attribute=" + UnitSearch_attribute.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_sub_a1.SelectedValue) == false) {
                sql += "sub_a1=" + UnitSearch_sub_a1.SelectedValue.ToString() + " AND ";
            }
            sql += " 1=1 ORDER BY g_id";
            return sql;
        }
        public DataGrid GetTargetDataGrid(int firstId, int lastId = -1, string type = null)
        {
            return UnitDataGrid;
        }

        #region Magic Numbers
        private static readonly double[] UnitParam_CategoryPer = new double[]{
	        0.38999998569488525,
	        0.40000000596046448,
	        0.40999999642372131,
	        0.41999998688697815,
	        0.43000000715255737,
	        0.40999999642372131
        };
        public static readonly decimal[] UnitParam_NextExp_Per = new decimal[]{
            2.38m,
            2.42m,
            2.48m,
            2.58m,
            2.66m,
            2.60m
        };
        private static readonly double[] UnitParam_StylePer = new double[]{
	        1.1000000238418579,
	        1.1000000238418579,
	        0.699999988079071,
	        0.800000011920929,
	        1.25,
	        0.85000002384185791,
	        0.949999988079071,
	        1.0,
	        0.949999988079071,
	        1.0,
	        0.60000002384185791,
	        1.2999999523162842
        };
        #endregion

    }
}
