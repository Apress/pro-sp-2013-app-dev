<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Exercise7_3Web.Pages.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <asp:HiddenField ID="SPAppToken" runat="server" />

    <div>    
        <table>
            <tr>
                <td>REST Path:</td>
                <td>
                    <asp:TextBox ID="txtREST" runat="server" Width="700">/_api/web/lists</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>Format:</td>
                <td>
                    <asp:DropDownList ID="ddlFormat" runat="server">
                        <asp:ListItem Value="application/json;odata=verbose">application/json;odata=verbose</asp:ListItem>
                        <asp:ListItem Value="application/atom-xml">application/atom-xml</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <asp:Button ID="cmdRunRequest" runat="server" Text="Execute the REST Request" OnClick="cmdRunRequest_Click" />
    </div>
    
    <h2><asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></h2>
    <asp:Label ID="lblOutput" runat="server" Text=""></asp:Label>    

    </form>
</body>
</html>
