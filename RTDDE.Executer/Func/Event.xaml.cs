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

        private void EventDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string Eventid = ((DataRowView)EventDataGrid.SelectedItem).Row["id"].ToString();
            EventInfo_id.Text = Eventid;
            EventInfo_lv.Text = "1";
            EventInfo_BindData(Eventid);
        }
        private void EventInfo_lv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EventInfo_lv.Text)) {
                EventInfo_lv.Text = "";
            }
            Regex r = new Regex("[^0-9]");
            if (r.Match(EventInfo_lv.Text).Success) {
                EventInfo_lv.Text = "1";
                return;
            }
            EventInfo_BindData(EventInfo_id.Text);
        }

        public void EventInfo_BindData(string Eventid)
        {
            if (string.IsNullOrWhiteSpace(EventInfo_lv.Text)) {
                return;
            }
            int thislevel = Convert.ToInt32(EventInfo_lv.Text);

            Task<MapEventMaster> task = new Task<MapEventMaster>(() =>
            {
                string sql = @"Select * from MAP_EVENT_Master WHERE id={0}";
                return DAL.ToSingle<MapEventMaster>(String.Format(sql, Eventid));
            });
            
            task.Start();
        }
    }
}
