<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calc.aspx.cs" Inherits="RTDDataExplorer.Calc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>角色属性</title>
    <style>
        body {
            font-size: 12px;
            font-family: 'Microsoft YaHei';
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="g_id"></asp:Label>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="name"></asp:Label>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="rare"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Level
                    </td>
                    <td>
                        <asp:TextBox ID="level" runat="server" Width="100">1</asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="calc" runat="server" OnClick="calc_Click" Text="计算" />
                    </td>
                </tr>
                <tr>
                    <td>HP</td>
                    <td colspan="2">
                        <asp:Label ID="HP" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>ATK</td>
                    <td colspan="2">
                        <asp:Label ID="ATK" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>HEAL</td>
                    <td colspan="2">
                        <asp:Label ID="HEAL" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>EXP</td>
                    <td colspan="2">
                        <asp:Label ID="EXP" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>pt</td>
                    <td colspan="2">
                        <asp:Label ID="pt" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>PASSIVE</td>
                    <td><asp:Label ID="passiveSkillName" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="passiveSkillText" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>ACTIVE</td>
                    <td><asp:Label ID="activeSkillName" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="activeSkillText" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>PANEL</td>
                    <td>
                        <asp:Label ID="panelSkillName" runat="server"></asp:Label></td>
                    <td>
                        <asp:Label ID="panelSkillText" runat="server"></asp:Label></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
