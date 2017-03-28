<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="app.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <script type="text/javascript" src="../Scripts/Models/siteconfigs.js"></script>
    <script type="text/javascript" src="../Scripts/ViewModels/default.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    Classroom Online Home Page
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
   
    <table width="100%">
        <tr>
            <td width="600">
                <div class="orglogo"><span id="schoolName"></span></div>
                <div id="mainmenu">
                    <ul>
                        <li class="adminOnly" style="display:none"><a href="config.aspx?{StandardTokens}" title="Configure Application">Configure Classroom Online</a></li>
                        <li class="adminOnly" style="display:none"><a href="../_layouts/15/start.aspx#/_layouts/15/people.aspx?MembershipGroupId={StudentGrpID}" title="Add Students">Add Students</a></li>
                        <li class="adminOnly" style="display:none"><a href="../_layouts/15/start.aspx#/_layouts/15/people.aspx?MembershipGroupId={FacultyGrpID}" title="Add Faculty">Add Faculty</a></li>
                        <li class="adminOnly" style="display:none"><a href="../Lists/SiteAssets" title="Site Assets">Site Assets</a></li>
                        <li><a href="roster.aspx?{StandardTokens}" title="View Roster">View the Class Roster</a></li>
                    </ul>
                </div>
            </td>
            <td class="applogo">
                <div><img src="../Images/AppLogo.png" /></div>
                <span>Classroom Online</span>
           </td>
        </tr>       
    </table>
</asp:Content>
