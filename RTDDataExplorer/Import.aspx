<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Import.aspx.cs" Inherits="RTDDataExplorer.Import" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>导入数据</title>
    <link rel="stylesheet" href="css/global.css" />
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr>
                <td>
                    <span>导入主数据（MDBS.xml）</span>
                    <br />
                    <asp:FileUpload ID="upfileMDBS" runat="server" />
                    <asp:RadioButtonList ID="radMDBS" runat="server">
                        <asp:ListItem Text="新增或覆盖" Value="1" Selected="True" />
                        <asp:ListItem Text="追加且更新" Value="0" Selected="False" />
                    </asp:RadioButtonList>
                    <asp:Button ID="btnUploadMDBS" OnClick="btnUploadMDBS_Click" Text="导入主数据" runat="server" />
                </td>
                <td>
                    <span>导入地图数据（LDBS.xml）</span>
                    <br />
                    <asp:FileUpload ID="upfileLDBS" runat="server" />
                    <asp:RadioButtonList ID="radLDBS" runat="server">
                        <asp:ListItem Text="新增或覆盖" Value="1" Selected="False" />
                        <asp:ListItem Text="追加且更新" Value="0" Selected="True" />
                    </asp:RadioButtonList>
                    <asp:Button ID="btnUploadLDBS" OnClick="btnUploadLDBS_Click" Text="导入地图数据" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:textbox TextMode="multiline" Width="100%" Height="200px" ID="lblInfo" runat="server"></asp:textbox>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
