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
    public partial class Skill : UserControl
    {
        public Skill()
        {
            InitializeComponent();
        }
        private void SkillDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void SkillTypeRadio_Party_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void SkillTypeRadio_Active_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void SkillTypeRadio_Panel_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void SkillDataGrid_BindData(string sql)
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(sql);
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.LogException(t.Exception.InnerException.Message);
                    //ExceptionMessage.Instance.Message = t.Exception.InnerException.Message;
                    return;
                }
                SkillDataGrid.ItemsSource = t.Result.DefaultView;
            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
