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
    /// Guide.xaml 的交互逻辑
    /// </summary>
    public partial class Guide : UserControl
    {
        public Guide()
        {
            InitializeComponent();
        }
        public void Refresh()
        {
            GuideTab_Initialized(null, null);
        }
        private void GuideTab_Initialized(object sender, EventArgs e)
        {
            Utility.BindData(GuideDataGrid, "SELECT id,icon,i0,i1,i2,i3 FROM UNIT_TALK_MASTER order by id");
        }
        private void GuideDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GuideDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string sp_guide_id = ((DataRowView)GuideDataGrid.SelectedItem).Row["id"].ToString();
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = @"SELECT * FROM unit_talk_master WHERE id={0}";
                DB db = new DB();
                return db.GetData(String.Format(sql, sp_guide_id));
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
                DataRow guideData = t.Result.Rows[0];
                GuideTalk.Children.Clear();
                for (int i = 0; i < 128; i++)
                {
                    string guide = guideData[i + 6].ToString();  //remove id&5 icon
                    if (!string.IsNullOrWhiteSpace(guide))
                    {
                        Grid grid = new Grid();
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        var tbName = new TextBlock()
                        {
                            Text = Utility.ParseMessageName(i),
                            Background = (SolidColorBrush)Application.Current.Resources["DefaultBrush"]
                        };
                        var tbValue = new TextBox()
                        {
                            Text = guide.Replace("*", "\n"),
                            IsReadOnly = true
                        };
                        grid.Children.Add(tbName);
                        grid.Children.Add(tbValue);
                        tbName.SetValue(Grid.RowProperty, 0);
                        tbValue.SetValue(Grid.RowProperty, 1);
                        GuideTalk.Children.Add(grid);
                    }
                }
            }, MainWindow.uiTaskScheduler);
            task.Start();
        }
    }
}
