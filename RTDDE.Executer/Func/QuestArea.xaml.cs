using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RTDDE.Provider;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    public partial class QuestArea : UserControl
    {
        public QuestArea()
        {
            InitializeComponent();
            QuestAreaExpander_Area.Visibility = Visibility.Collapsed;
            QuestAreaExpander_Quest.Visibility = Visibility.Collapsed;
            QuestAreaExpander_Reward.Visibility = Visibility.Collapsed;
        }
        public class QuestReward
        {
            public int point;
            public int present_type;
            public string present_param_name;
            public int present_param_1;
        }
        async private void QuestAreaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestAreaDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string qcInfo_id = ((DataRowView)QuestAreaDataGrid.SelectedItem).Row["id"].ToString();
            QuestAreaInfo_id.Text = qcInfo_id;
            Task<QuestAreaMaster> taskQuestAreaMaster = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_area_master WHERE id={0}";
                return DAL.ToSingle<QuestAreaMaster>(String.Format(sql, qcInfo_id));
            });
            Task<List<QuestMaster>> taskQuest = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_master WHERE parent_area_id={0} order by display_order DESC";
                return DAL.ToList<QuestMaster>(String.Format(sql, qcInfo_id));
            });
            Task<List<QuestReward>> taskReward = Task.Run(() =>
            {
                const string sql = @"select point,present_type,present_param_1,
(case when present_type=4 
then (select name from unit_master where unit_master.id=quest_challenge_reward_master.present_param_0) 
else present_param_0 end) as present_param_name 
from quest_challenge_reward_master WHERE parent_area_id={0}
order by point";
                return DAL.ToList<QuestReward>(String.Format(sql, qcInfo_id));
            });

            QuestAreaMaster qam = await taskQuestAreaMaster;
            QuestAreaInfo_name.Text = qam.name;
            QuestAreaInfo_display_order.Text = qam.display_order.ToString();
            QuestAreaInfo_lock_type.Text = qam.lock_type.ToString();
            QuestAreaInfo_text.Text = Utility.ParseText(qam.text);

            //parent field
            Task<QuestFieldMaster> questParentFieldTask = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_field_master WHERE id={0}";
                return DAL.ToSingle<QuestFieldMaster>(String.Format(sql, qam.parent_field_id));
            });
            QuestFieldMaster qfmParent = await questParentFieldTask;
            if (qfmParent == null) {
                QuestAreaInfo_parent.Visibility = Visibility.Collapsed;
            }
            else {
                QuestAreaInfo_parent.Visibility = Visibility.Visible;
                QuestAreaInfo_parent_field_id.Text = qfmParent.id.ToString();
                QuestAreaInfo_parent_field_name.Text = qfmParent.name;
            }
            //move field
            Task<QuestFieldMaster> questMoveFieldTask = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_field_master WHERE id={0}";
                return DAL.ToSingle<QuestFieldMaster>(String.Format(sql, qam.move_field_id));
            });
            QuestFieldMaster qfmMove = await questMoveFieldTask;
            if (qfmMove == null) {
                QuestAreaInfo_move.Visibility = Visibility.Collapsed;
            }
            else {
                QuestAreaInfo_move.Visibility = Visibility.Visible;
                QuestAreaInfo_move_field_id.Text = qfmMove.id.ToString();
                QuestAreaInfo_move_field_name.Text = qfmMove.name;
            }
            //connect area
            Task<QuestAreaMaster> questConnectAreaTask = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_area_master WHERE id={0}";
                return DAL.ToSingle<QuestAreaMaster>(String.Format(sql, qam.connect_area_id));
            });
            QuestAreaMaster qamConnect = await questConnectAreaTask;
            if (qamConnect == null) {
                QuestAreaInfo_connect.Visibility = Visibility.Collapsed;
            }
            else {
                QuestAreaInfo_connect.Visibility = Visibility.Visible;
                QuestAreaInfo_connect_area_id.Text = qamConnect.id.ToString();
                QuestAreaInfo_connect_area_name.Text = qamConnect.name;
            }
            //lock quest
            Task<QuestMaster> questLockQuestTask = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_master WHERE id={0}";
                return DAL.ToSingle<QuestMaster>(String.Format(sql, qam.lock_value));
            });
            QuestAreaInfo_lock.Visibility = Visibility.Collapsed;
            if (qam.lock_type == 1) {
                QuestMaster qmLock = await questLockQuestTask;
                if (qmLock == null) {
                    QuestAreaInfo_lock.Visibility = Visibility.Collapsed;
                }
                else {
                    QuestAreaInfo_lock.Visibility = Visibility.Visible;
                    QuestAreaInfo_lock_value.Text = qmLock.id.ToString();
                    QuestAreaInfo_lock_name.Text = qmLock.name;
                    QuestAreaInfo_lock_dialog_msg.Document = Utility.ParseTextToDocument(qam.lock_dialog_msg);
                }
            }
            //expander
            if (QuestAreaInfo_parent.Visibility == Visibility.Collapsed &&
                QuestAreaInfo_move.Visibility == Visibility.Collapsed &&
                QuestAreaInfo_connect.Visibility == Visibility.Collapsed &&
                QuestAreaInfo_lock.Visibility == Visibility.Collapsed) {
                QuestAreaExpander_Area.Visibility = Visibility.Collapsed;
            }
            else {
                QuestAreaExpander_Area.Visibility = Visibility.Visible;
            }

            //quest in area
            List<QuestMaster> questMasters = await taskQuest;
            if (questMasters == null || questMasters.Count == 0) {
                QuestAreaExpander_Quest.Visibility = Visibility.Collapsed;
                QuestAreaToQuestButton.IsEnabled = false;
            }
            else {
                QuestAreaInfo_Quest.Children.Clear();
                QuestAreaExpander_Quest.Visibility = Visibility.Visible;
                QuestAreaToQuestButton.IsEnabled = true;
                foreach (QuestMaster qm in questMasters) {
                    int id = qm.id;
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    TextBlock textBlock = new TextBlock() { Text = id.ToString() };
                    textBlock.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(textBlock);
                    TextBox textBox = new TextBox() { Text = qm.name };
                    textBox.SetValue(Grid.ColumnProperty, 1);
                    grid.Children.Add(textBox);
                    Button button = new Button()
                    {
                        Content = "→",
                        Style = FindResource("InlineButton") as Style
                    };
                    button.Click += async (s, arg) =>
                    {
                        Quest quest = await Utility.GetTab<Quest>();
                        quest.GoToItemById(id);
                    };
                    button.SetValue(Grid.ColumnProperty, 2);
                    grid.Children.Add(button);
                    QuestAreaInfo_Quest.Children.Add(grid);
                }
            }

            //challenge reward for this area
            List<QuestReward> rewardList = await taskReward;
            if (rewardList == null || rewardList.Count == 0) {
                QuestAreaExpander_Reward.Visibility = Visibility.Collapsed;
            }
            else {
                QuestAreaInfo_Reward.Children.Clear();
                QuestAreaExpander_Reward.Visibility = Visibility.Visible;
                foreach (QuestReward reward in rewardList) {
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    TextBlock textBlock = new TextBlock() { Text = reward.point.ToString() };
                    textBlock.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(textBlock);
                    TextBox typeTextBox = new TextBox() { Text = Utility.ParsePresentType(reward.present_type) };
                    typeTextBox.SetValue(Grid.ColumnProperty, 1);
                    grid.Children.Add(typeTextBox);
                    TextBox nameTextBox = new TextBox() { Text = reward.present_param_name };
                    nameTextBox.SetValue(Grid.ColumnProperty, 2);
                    grid.Children.Add(nameTextBox);
                    TextBox paramTextBox = new TextBox() { Text = reward.present_param_1.ToString() };
                    paramTextBox.SetValue(Grid.ColumnProperty, 3);
                    grid.Children.Add(paramTextBox);
                    QuestAreaInfo_Reward.Children.Add(grid);
                }
            }

            //Advanced
            QuestAreaInfo_flag_no.Text = qam.flag_no.ToString();
            QuestAreaInfo_name_short.Text = qam.name_short;
            QuestAreaInfo_banner_bg_texture.Text = qam.banner_bg_texture;
            QuestAreaInfo_banner_texture.Text = qam.banner_texture;
            QuestAreaInfo_icon_texture.Text = qam.icon_texture;
            QuestAreaInfo_banner_texture_url.Text = "http://www.acquirespdl.jp/RTD/DLC/BANNER/" + qam.banner_texture;

            ////banner image
            //if (string.IsNullOrEmpty(qam.banner_texture) == false) {
            //    BitmapImage bitmapImage = new BitmapImage();
            //    bitmapImage.BeginInit();
            //    bitmapImage.UriSource = new Uri("http://www.acquirespdl.jp/RTD/DLC/BANNER/" + qam.banner_texture, UriKind.Absolute);
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapImage.EndInit();
            //    QuestAreaInfo_banner_texture_image.Source = bitmapImage;
            //}
            //else {
            //    QuestAreaInfo_banner_texture_image.Source = null;
            //}
        }

        private void QuestAreaTypeRadio_Main_Checked(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT id,name,text FROM quest_area_master WHERE parent_field_id>0 order by id DESC";
            Utility.BindData(QuestAreaDataGrid, sql);
        }
        private void QuestAreaTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT id,name,text FROM quest_area_master WHERE parent_field_id=0 order by id DESC";
            Utility.BindData(QuestAreaDataGrid, sql);
        }

        private void QuestAreaSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utility.BindData(QuestAreaDataGrid, QuestAreaSearch_BuildSQL());
        }
        private string QuestAreaSearch_BuildSQL()
        {
            StringBuilder sb = new StringBuilder("SELECT id,name,text FROM quest_area_master WHERE ");
            if (String.IsNullOrWhiteSpace(QuestAreaSearch_id.Text) == false) {
                sb.AppendFormat("id={0} AND ", QuestAreaSearch_id.Text.Trim());
            }
            if (String.IsNullOrWhiteSpace(QuestAreaSearch_name.Text) == false) {
                sb.AppendFormat("name LIKE '%{0}%' AND ", QuestAreaSearch_name.Text.Trim());
            }
            sb.AppendLine(" 1=1 order by id DESC");
            return sb.ToString();
        }

        private void QuestAreaSearchClear_Click(object sender, RoutedEventArgs e)
        {
            QuestAreaSearch_id.Text = string.Empty;
            QuestAreaSearch_name.Text = string.Empty;
            QuestAreaTypeRadio_Main.IsChecked = true;
        }

        async private void QuestAreaToQuestButton_OnClick(object sender, RoutedEventArgs e)
        {
            string id = QuestAreaInfo_id.Text;
            if (string.IsNullOrEmpty(id)) {
                return;
            }
            Task<QuestMaster> taskFirstQuest = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_master WHERE parent_area_id={0} order by display_order ASC";
                return DAL.ToSingle<QuestMaster>(String.Format(sql, id));
            });
            Task<QuestMaster> taskLastQuest = Task.Run(() =>
            {
                const string sql = "SELECT * FROM quest_master WHERE parent_area_id={0} order by display_order DESC";
                return DAL.ToSingle<QuestMaster>(String.Format(sql, id));
            });
            QuestMaster firstQuestMaster = await taskFirstQuest;
            QuestMaster lastQuestMaster = await taskFirstQuest;
            if (firstQuestMaster != null && lastQuestMaster != null) {
                Quest quest = await Utility.GetTab<Quest>();
                quest.GoToItemById(firstQuestMaster.id, lastQuestMaster.id);
            }
            else {
                Utility.ShowException("NO QUEST IN AREA");
            }
        }

        async private void QuestAreaInfoToLockQuestButton_OnClick(object sender, RoutedEventArgs e)
        {
            Quest quest = (Quest)await Utility.GetTab<Quest>();
            quest.GoToItemById(Convert.ToInt32(QuestAreaInfo_lock_value.Text)); ;
        }
    }
}
