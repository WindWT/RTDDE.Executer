<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RTDDataExplorer.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>RTD Data Explorer</title>
    <link rel="stylesheet" href="css/global.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <a href="Import.aspx">导入xml</a>
        </div>
        <div>
            <a href="View.aspx">查看数据</a>
        </div>
        <div>
            <span>数据库状态：</span><asp:Label ID="lblDBStatus" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
