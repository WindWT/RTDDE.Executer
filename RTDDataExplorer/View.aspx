<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="RTDDataExplorer.View" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>数据查看</title>
    <link rel="stylesheet" href="css/global.css" />
    <script src="Scripts/jquery-2.0.3.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox TextMode="multiline" Width="100%" Height="100px" ReadOnly="false" ID="lblSQL" runat="server"></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />
            <asp:DropDownList ID="ddlQuickSearch" runat="server" OnSelectedIndexChanged="ddlQuickSearch_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Text="快速查询" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="MDB" Value="MDBTableName"></asp:ListItem>
                <asp:ListItem Text="角色故事" Value="UnitStory"></asp:ListItem>
                <asp:ListItem Text="新限时任务" Value="NewQuest"></asp:ListItem>
                <asp:ListItem Text="任务分类" Value="QuestCategory"></asp:ListItem>
                <asp:ListItem Text="主线任务" Value="MainQuest"></asp:ListItem>
                <asp:ListItem Text="日常任务" Value="DailyQuest"></asp:ListItem>
            </asp:DropDownList>
            <asp:CheckBox ID="isShowMap" runat="server" Text="显示地图" Checked="false" OnCheckedChanged="isShowMap_CheckedChanged" AutoPostBack="true" />
            <asp:CheckBox ID="isShowCalc" runat="server" Text="显示角色属性" Checked="false" OnCheckedChanged="isShowCalc_CheckedChanged" AutoPostBack="true" />
            <asp:GridView ID="grid" AllowPaging="true" PageSize="25" AllowSorting="false" runat="server" AutoGenerateColumns="true" EnableSortingAndPagingCallbacks="true">
                <Columns>
                    <asp:HyperLinkField Text="查看地图" DataNavigateUrlFields="id"
                        DataNavigateUrlFormatString="Map.aspx?id={0}" Target="_blank"  Visible="false"></asp:HyperLinkField>
                    <asp:HyperLinkField Text="属性计算" DataNavigateUrlFields="g_id"
                        DataNavigateUrlFormatString="Calc.aspx?g_id={0}" Target="_blank"  Visible="false"></asp:HyperLinkField>
                </Columns>
            </asp:GridView>
            <asp:TextBox TextMode="multiline" Width="100%" Height="25px" ReadOnly="true" ID="lblInfo" runat="server"></asp:TextBox>
        </div>
    </form>
</body>
</html>
