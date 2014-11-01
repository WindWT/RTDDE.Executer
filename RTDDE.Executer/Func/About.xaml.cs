using RTDDE.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace RTDDE.Executer
{
    /// <summary>
    /// Common.xaml 的交互逻辑
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            UsageTextAndroid.Text =
@"/data/data/jp.co.acquire.RTD/files/
=====>Config-Import MDBS MsgPack
/data/data/jp.co.acquire.RTD/shared_prefs/GAME.xml
=====>Config-Import GAME(MAP Data)
/data/data/jp.co.acquire.RTD/shared_prefs/com.prime31.UnityPlayerNativeActivity.xml
=====>??????";
            UsageTextiOS.Text =
@"[RoDora]/Library/Caches/
=====>Config-Import MDBS MsgPack
[RoDora]/Library/Caches/GAME
=====>Config-Import GAME(MAP Data)
[RoDora]/Library/Preferences/jp.co.acquire.RTD.plist
=====>??????";
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
