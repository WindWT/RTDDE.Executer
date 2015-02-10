using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace RTDDE.Executer.Func
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
/data/data/jp.co.acquire.RTD/files/Restore/LDBS0_Msg.bytes
=====>Config-Import MAP
/data/data/jp.co.acquire.RTD/shared_prefs/GAME.xml
/data/data/jp.co.acquire.RTD/shared_prefs/com.prime31.UnityPlayerNativeActivity.xml
=====>??????";
            UsageTextiOS.Text =
@"[RoDora]/Library/Caches/
=====>Config-Import MDBS MsgPack
[RoDora]/Library/Caches/Restore/LDBS0_Msg.bytes
=====>Config-Import MAP
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
