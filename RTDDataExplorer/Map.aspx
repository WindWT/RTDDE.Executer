<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="RTDDataExplorer.Map" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>地图查看</title>
    <style>
        body {
            font-size: 12px;
            font-family: 'Microsoft YaHei';
        }

        table, td {
            border-collapse: collapse;
            border: solid;
            border-width: 1px;
            border-color: black;
            white-space: nowrap;
        }

            td.map {
                width: 22px;
                height: 22px;
                margin:0;
                padding:0;
                text-align: center;
                box-sizing: content-box;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="map" runat="server"></asp:Label>
            <asp:GridView ID="questGrid" runat="server" AllowPaging="false" AllowSorting="false" AllowCustomPaging="false"></asp:GridView>
            <asp:GridView ID="questMonsterGrid" runat="server" AllowPaging="false" AllowSorting="false" AllowCustomPaging="false"></asp:GridView>
        </div>
    </form>
</body>
</html>
