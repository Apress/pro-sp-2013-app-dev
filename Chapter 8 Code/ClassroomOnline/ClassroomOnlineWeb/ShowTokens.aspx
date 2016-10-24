<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowTokens.aspx.cs" Inherits="ClassroomOnlineWeb.Pages.ShowTokens" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Remote Web Tokens</title>
</head>
<body>
    <h2>Remote Web Tokens and Values</h2>
    <form id="Form1" runat="server">
        <asp:GridView ID="grd" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="True" Width="100%">
            <AlternatingRowStyle BackColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
    </form>
</body>
</html>