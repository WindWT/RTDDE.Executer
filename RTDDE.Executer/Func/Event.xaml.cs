using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RTDDE.Executer.Util;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;
using RTDDE.Provider.Util;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Event.xaml 的交互逻辑
    /// </summary>
    public partial class Event : UserControl, IRedirectable
    {
        public Event()
            : this(false)
        {
        }
        public Event(bool disableAutoLoad)
        {
            InitializeComponent();
            if (disableAutoLoad == false) {
                Utility.BindData(EventDataGrid, "SELECT id,name FROM MAP_EVENT_Master order by id DESC");
            }
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
            Task<List<QuestOpenType>> taskOpenType = Task.Run(() =>
            {
                List<QuestOpenType> opentypeList = new List<QuestOpenType>();
                if (mapEvent == null) {
                    return null;
                }
                opentypeList.Add(Utility.GetQuestOpenType(mapEvent.open_type_01, (int)mapEvent.open_param_01, 0));
                opentypeList.Add(Utility.GetQuestOpenType(mapEvent.open_type_02, (int)mapEvent.open_param_02, 0));
                opentypeList.Add(Utility.GetQuestOpenType(mapEvent.open_type_03, (int)mapEvent.open_param_03, 0));
                opentypeList.Add(Utility.GetQuestOpenType(mapEvent.open_type_04, (int)mapEvent.open_param_04, 0));
                opentypeList.Add(Utility.GetQuestOpenType(100, (int)mapEvent.open_timezone, (int)mapEvent.close_timezone));
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
            AreaMap.LoadArea((int)qfmParent.id);
            AreaMap.LoadEventMark(mapEvent);
            //quests
            Task<List<QuestMaster>> mapEventQuestTask = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_master WHERE parent_map_event_id={0}";
                return DAL.ToList<QuestMaster>(String.Format(sql, mapEvent.id));
            });
            List<QuestMaster> quests = await mapEventQuestTask;
            EventInfo_quests.Children.Clear();
            foreach (QuestMaster quest in quests) {
                int id = quest.id;
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                TextBox typeTextBox = new TextBox() { Text = quest.id.ToString() };
                typeTextBox.SetValue(Grid.ColumnProperty, 0);
                grid.Children.Add(typeTextBox);
                TextBox paramTextBox = new TextBox() { Text = quest.name };
                paramTextBox.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(paramTextBox);
                Button button = new Button()
                {
                    Content = "→",
                    Style = FindResource("InlineButton") as Style
                };
                button.Click += (s, arg) =>
                {
                    Utility.GoToItemById<Quest>(id);
                };
                button.SetValue(Grid.ColumnProperty, 2);
                grid.Children.Add(button);
                EventInfo_quests.Children.Add(grid);
            }
            //opentype
            List<QuestOpenType> opentypes = await taskOpenType;
            EventInfo_opentype_content.Children.Clear();
            if (opentypes.Count == 0) {
                EventInfo_opentype.Visibility = Visibility.Collapsed;
            }
            else {
                EventInfo_opentype.Visibility = Visibility.Visible;
                foreach (var type in opentypes) {
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
            EventInfo_banner_bg_texture_url.Text = "http://www.acquirespdl.jp/RTD/DLC/BANNER/" + mapEvent.dialog_banner;
        }

        public DataGrid GetTargetDataGrid(int firstId, int lastId = -1, string type = null)
        {
            Utility.BindData(EventDataGrid, "SELECT id,name FROM MAP_EVENT_Master order by id DESC");
            return EventDataGrid;
        }

        private void EventSearchClear_OnClick(object sender, RoutedEventArgs e) {
            EventSearch_id.Text = String.Empty;
            EventSearch_name.Text = String.Empty;
            Utility.BindData(EventDataGrid, "SELECT id,name FROM MAP_EVENT_Master order by id DESC");
        }

        private void EventSearch_TextChanged(object sender, TextChangedEventArgs e) {
            string sql = @"SELECT id,name FROM MAP_EVENT_Master WHERE ";
            if (String.IsNullOrWhiteSpace(EventSearch_id.Text) == false) {
                sql += "id=" + EventSearch_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(EventSearch_name.Text) == false) {
                sql += "name LIKE '%" + EventSearch_name.Text.Trim() + "%' AND ";
            }
            sql += " 1=1 ORDER BY id DESC";
            Utility.BindData(EventDataGrid, sql);
        }
    }
}
