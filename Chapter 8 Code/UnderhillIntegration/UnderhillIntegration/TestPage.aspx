<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs" Inherits="UnderhillIntegration.TestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
         <asp:Button ID="cmdGo" runat="server" Text="Go" OnClick="cmdGo_Click" />

        <h2>
        <asp:Label ID="lblStatus" runat="server" Text="Status will go here..."></asp:Label>
        </h2>

        <asp:Label ID="lblResponse" runat="server" Text="Response will go here..."></asp:Label>
        
    </form>
</body>
</html>
