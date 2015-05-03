using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Event.xaml 的交互逻辑
    /// </summary>
    public partial class Event : UserControl
    {
        public Event()
        {
            InitializeComponent();
        }
        private void EventTab_Initialized(object sender, EventArgs e)
        {
            Utility.BindData(EventDataGrid, "SELECT id,name FROM MAP_EVENT_Master order by id");
        }

        async private void EventDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string Eventid = ((DataRowView)EventDataGrid.SelectedItem).Row["id"].ToString();
            EventInfo_id.Text = Eventid;
            Task<MapEventMaster> task = Task.Run(() =>
            {
                string sql = @"Select * from MAP_EVENT_Master WHERE id={0}";
                return DAL.ToSingle<MapEventMaster>(String.Format(sql, Eventid));
            });

            MapEventMaster mapEvent = await task;
            Task<List<OpenType>> taskOpenType = Task.Run(() =>
            {
                List<OpenType> opentypeList = new List<OpenType>();
                if (mapEvent == null) {
                    return null;
                }
                opentypeList.Add(Utility.ParseOpentype(mapEvent.open_type_01, (int)mapEvent.open_param_01, 0));
                opentypeList.Add(Utility.ParseOpentype(mapEvent.open_type_02, (int)mapEvent.open_param_02, 0));
                opentypeList.Add(Utility.ParseOpentype(mapEvent.open_type_03, (int)mapEvent.open_param_03, 0));
                opentypeList.Add(Utility.ParseOpentype(mapEvent.open_type_04, (int)mapEvent.open_param_04, 0));
                opentypeList.Add(Utility.ParseOpentype(100, (int)mapEvent.open_timezone, (int)mapEvent.close_timezone));
                opentypeList.RemoveAll(o => string.IsNullOrEmpty(o.Type));
                return opentypeList;
            });

            EventInfo_name.Text = mapEvent.name;
            EventInfo_text.Text = mapEvent.text;
            EventInfo_dialog_text.Text = mapEvent.dialog_text;

            EventInfo_event_type.Text = mapEvent.event_type.ToString();
            EventInfo_max_count.Text = mapEvent.max_count.ToString();

            //parent field
            Task<QuestFieldMaster> mapEventParentFieldTask = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_field_master WHERE id={0}";
                return DAL.ToSingle<QuestFieldMaster>(String.Format(sql, mapEvent.parent_field_id));
            });
            QuestFieldMaster qfmParent = await mapEventParentFieldTask;
            if (qfmParent == null) {
                EventInfo_parent.Visibility = Visibility.Collapsed;
            }
            else {
                EventInfo_parent.Visibility = Visibility.Visible;
                EventInfo_parent_field_id.Text = qfmParent.id.ToString();
                EventInfo_parent_field_name.Text = qfmParent.name;
            }
            //quests
            Task<List<QuestMaster>> mapEventQuestTask = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_master WHERE parent_map_event_id={0}";
                return DAL.ToList<QuestMaster>(String.Format(sql, mapEvent.id));
            });
            List<QuestMaster> quests = await mapEventQuestTask;
            EventInfo_quests.Children.Clear();
            foreach (QuestMaster quest in quests) {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                TextBox typeTextBox = new TextBox() { Text = quest.id.ToString() };
                typeTextBox.SetValue(Grid.ColumnProperty, 0);
                grid.Children.Add(typeTextBox);
                TextBox paramTextBox = new TextBox() { Text = quest.name };
                paramTextBox.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(paramTextBox);
                EventInfo_quests.Children.Add(grid);
            }
            //opentype
            List<OpenType> opentypes = await taskOpenType;
            EventInfo_opentype_content.Children.Clear();
            if (opentypes.Count == 0) {
                EventInfo_opentype.Visibility = Visibility.Collapsed;
            }
            else {
                EventInfo_opentype.Visibility = Visibility.Visible;
                foreach (OpenType type in opentypes) {
                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    TextBox typeTextBox = new TextBox() { Text = type.Type };
                    typeTextBox.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(typeTextBox);
                    TextBox paramTextBox = new TextBox() { Text = type.Param };
                    paramTextBox.SetValue(Grid.ColumnProperty, 1);
                    grid.Children.Add(paramTextBox);
                    TextBox groupTextBox = new TextBox() { Text = type.Group.ToString() };
                    groupTextBox.SetValue(Grid.ColumnProperty, 2);
                    grid.Children.Add(groupTextBox);
                    EventInfo_opentype_content.Children.Add(grid);
                }
            }
            EventInfo_post_type.Text = mapEvent.post_type.ToString();
            EventInfo_post_message_id.Text = mapEvent.post_message_id.ToString();
            EventInfo_post_param_01.Text = mapEvent.post_param_01.ToString();
            EventInfo_post_param_02.Text = mapEvent.post_param_02.ToString();
            //advanced
            EventInfo_banner_bg_texture.Text = mapEvent.banner_bg_texture;
            EventInfo_icon_texture.Text = mapEvent.icon_texture;
            EventInfo_dialog_banner.Text = mapEvent.dialog_banner;
            EventInfo_icon.Text = mapEvent.icon;
            EventInfo_icon_pos_x.Text = mapEvent.icon_pos_x.ToString();
            EventInfo_icon_pos_y.Text = mapEvent.icon_pos_y.ToString();
            EventInfo_icon_type.Text = mapEvent.icon_type.ToString();
            EventInfo_banner_bg_texture_url.Text = "http://www.acquirespdl.jp/RTD/DLC/BANNER/" + mapEvent.banner_bg_texture;
        }
    }
}
