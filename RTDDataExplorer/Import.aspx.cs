using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;
using RTDDataProvider;

namespace RTDDataExplorer
{
    public partial class Import : System.Web.UI.Page
    {
        private static string MDBS_FILENAME = "MDBS.xml";
        private static string LDBS_FILENAME = "LDBS.xml";
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnUploadMDBS_Click(object sender, EventArgs e)
        {
            StringBuilder info = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            info.AppendLine("导入开始。");
            lblInfo.Text = info.ToString();
            string filename = upfileMDBS.FileName;
            if (string.Compare(filename, MDBS_FILENAME, StringComparison.OrdinalIgnoreCase) != 0)
            {
                info.AppendLine("导入失败。");
                info.AppendLine("文件选择错误，请确认是否为MDBS.xml。");
                lblInfo.Text = info.ToString();
                return;
            }
            bool isNewDB = true;
            if (radMDBS.SelectedValue == "0")
            {
                isNewDB = false;
            }
            using (StreamReader sr = new StreamReader(upfileMDBS.PostedFile.InputStream))
            {
                string xmlMDB = sr.ReadToEnd();
                try
                {
                    DataSet ds = FileParser.ParseXmlMDB(xmlMDB);
                    info.AppendLine("XML解析为DataSet成功。");
                    lblInfo.Text = info.ToString();
                    DB db = new DB(HostingEnvironment.MapPath("/RTD.db"));
                    info.AppendLine("MDB连接成功。");
                    db.ImportDataSet(ds, isNewDB);
                    info.AppendLine("DataSet导入到MDB成功。");
                    sw.Stop();
                    info.AppendLine(sw.Elapsed.ToString());
                    info.AppendLine("导入成功。");
                    lblInfo.Text = info.ToString();
                }
                catch (Exception ex)
                {
                    info.AppendLine("导入失败。");
                    info.AppendLine(ex.Message);
                    lblInfo.Text = info.ToString();
                }
            }
        }

        protected void btnUploadLDBS_Click(object sender, EventArgs e)
        {
            StringBuilder info = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            info.AppendLine("导入开始。");
            lblInfo.Text = info.ToString();
            string filename = upfileLDBS.FileName;
            if (string.Compare(filename, LDBS_FILENAME, StringComparison.OrdinalIgnoreCase) != 0)
            {
                info.AppendLine("导入失败。");
                info.AppendLine("文件选择错误，请确认是否为LDBS.xml。");
                lblInfo.Text = info.ToString();
                return;
            }
            bool isNewDB = false;
            if (radLDBS.SelectedValue == "1")
            {
                isNewDB = true;
            }
            using (StreamReader sr = new StreamReader(upfileLDBS.PostedFile.InputStream))
            {
                string xmlLDB = sr.ReadToEnd();
                try
                {
                    DataTable dt = FileParser.ParseXmlLDB(xmlLDB);
                    info.AppendLine("XML解析为DataSet成功。");
                    lblInfo.Text = info.ToString();

                    DB ldb = new DB(HostingEnvironment.MapPath("/RTD.db"));
                    info.AppendLine("LDB连接成功。");
                    ldb.ImportDataTable(dt, "level_data_id", isNewDB);
                    info.AppendLine("DataTable导入到LDB成功。");
                    sw.Stop();
                    info.AppendLine(sw.Elapsed.ToString());
                    info.AppendLine("导入成功。");
                    lblInfo.Text = info.ToString();
                }
                catch (Exception ex)
                {
                    info.AppendLine("导入失败。");
                    info.AppendLine(ex.Message);
                    lblInfo.Text = info.ToString();
                }
            }
        }
    }
}