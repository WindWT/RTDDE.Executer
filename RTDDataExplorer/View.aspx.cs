using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace RTDDataExplorer
{
    public partial class View : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblSQL.Text))
            {
                lblSQL.Text = "SELECT * FROM USER_RANK_MASTER";
            }
            dataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataBind();
        }
        protected void isShowMap_CheckedChanged(object sender, EventArgs e)
        {
            dataBind();
        }
        protected void isShowCalc_CheckedChanged(object sender, EventArgs e)
        {
            dataBind();
        }
        private void dataBind()
        {
            string sql = lblSQL.Text;
            grid.Columns[0].Visible = isShowMap.Checked;
            grid.Columns[1].Visible = isShowCalc.Checked;
            try
            {
                DB db = new DB(false);
                DataTable dt = db.GetData(sql);
                grid.DataSource = dt;
                grid.DataBind();
                lblInfo.Text = String.Empty;
            }
            catch (Exception ex)
            {
                lblInfo.Text = ex.Message;
            }
        }

        protected void ddlQuickSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ddlQuickSearch.SelectedValue))
            {
                switch (ddlQuickSearch.SelectedValue)
                {
                    case "UnitStory":
                        {
                            lblSQL.Text = "SELECT g_id,name,story FROM UNIT_MASTER order by g_id";
                            break;
                        }
                    case "NewQuest":
                        {
                            lblSQL.Text = @"SELECT id,
       name,category,
       ( CASE
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                ELSE 0 
       END ) AS [end],
       ( CASE
                WHEN open_type_1 = 6 THEN open_param_1 
                WHEN open_type_2 = 6 THEN open_param_2 
                WHEN open_type_3 = 6 THEN open_param_3 
                WHEN open_type_4 = 6 THEN open_param_4 
                WHEN open_type_5 = 6 THEN open_param_5 
                WHEN open_type_6 = 6 THEN open_param_6 
                ELSE 0 
       END ) AS isDisabled1,
       ( CASE
                WHEN open_type_1 = 8 THEN open_param_1 
                WHEN open_type_2 = 8 THEN open_param_2 
                WHEN open_type_3 = 8 THEN open_param_3 
                WHEN open_type_4 = 8 THEN open_param_4 
                WHEN open_type_5 = 8 THEN open_param_5 
                WHEN open_type_6 = 8 THEN open_param_6 
                ELSE 0 
       END ) AS isDisabled2,
       ( CASE
                WHEN open_type_1 = 7 THEN open_param_1 
                WHEN open_type_2 = 7 THEN open_param_2 
                WHEN open_type_3 = 7 THEN open_param_3 
                WHEN open_type_4 = 7 THEN open_param_4 
                WHEN open_type_5 = 7 THEN open_param_5 
                WHEN open_type_6 = 7 THEN open_param_6 
                ELSE 0 
       END ) AS isNeedQuestClear
  FROM quest_master
 ORDER BY start DESC,id DESC;";
                            break;
                        }
                    case "QuestCategory":
                        {
                            lblSQL.Text = "SELECT * FROM QUEST_CATEGORY_MASTER ORDER BY id";
                            break;
                        }
                    case "MainQuest":
                        {
                            lblSQL.Text = @"SELECT * FROM QUEST_MASTER
WHERE category<1000
ORDER BY id DESC";
                            break;
                        }
                    case "MDBTableName":
                        {
                            lblSQL.Text = @"SELECT name FROM sqlite_master WHERE type='table'";
                            break;
                        }
                    case "DailyQuest":
                        {
                            string today = DateTime.Today.AddHours(1).ToString("yyyyMMddHH");
                            lblSQL.Text = @"SELECT id,name,category,
       ( CASE
                WHEN open_type_1 = 1 THEN open_param_1 
                WHEN open_type_2 = 1 THEN open_param_2 
                WHEN open_type_3 = 1 THEN open_param_3 
                WHEN open_type_4 = 1 THEN open_param_4 
                WHEN open_type_5 = 1 THEN open_param_5 
                WHEN open_type_6 = 1 THEN open_param_6 
                ELSE -1
       END ) AS DayOfWeek,
       ( CASE
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                ELSE 0 
       END ) AS [end],
       ( CASE
                WHEN open_type_1 = 6 THEN open_param_1 
                WHEN open_type_2 = 6 THEN open_param_2 
                WHEN open_type_3 = 6 THEN open_param_3 
                WHEN open_type_4 = 6 THEN open_param_4 
                WHEN open_type_5 = 6 THEN open_param_5 
                WHEN open_type_6 = 6 THEN open_param_6 
                ELSE 0 
       END ) AS isDisabled1,
       ( CASE
                WHEN open_type_1 = 8 THEN open_param_1 
                WHEN open_type_2 = 8 THEN open_param_2 
                WHEN open_type_3 = 8 THEN open_param_3 
                WHEN open_type_4 = 8 THEN open_param_4 
                WHEN open_type_5 = 8 THEN open_param_5 
                WHEN open_type_6 = 8 THEN open_param_6 
                ELSE 0 
       END ) AS isDisabled2,
       ( CASE
                WHEN open_type_1 = 7 THEN open_param_1 
                WHEN open_type_2 = 7 THEN open_param_2 
                WHEN open_type_3 = 7 THEN open_param_3 
                WHEN open_type_4 = 7 THEN open_param_4 
                WHEN open_type_5 = 7 THEN open_param_5 
                WHEN open_type_6 = 7 THEN open_param_6 
                ELSE 0 
       END ) AS isNeedQuestClear
FROM QUEST_MASTER
WHERE DayOfWeek>=0
AND isDisabled1=0
AND isDisabled2=0
AND ([end]>"+today+@" OR [end]=0)
ORDER BY DayOfWeek,id DESC";
                            break;
                        }
                }
                dataBind();
            }
        }
    }
}