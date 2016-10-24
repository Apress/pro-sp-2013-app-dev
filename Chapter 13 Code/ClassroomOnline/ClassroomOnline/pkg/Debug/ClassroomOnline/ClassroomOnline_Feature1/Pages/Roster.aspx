<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="app.master" Language="C#" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.2.1.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>

    <script type="text/javascript" src="../Scripts/Models/siteconfigs.js"></script>
    <script type="text/javascript" src="../Scripts/ViewModels/roster.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    Class Roster for <span data-bind="text: courseNumber"></span> - <span data-bind="text: courseName"></span>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
   <p data-bind="visible: isFaculty" style="display:none">You are a member of the faculty for this course.</p>
   <p data-bind="visible: isStudent" style="display:none">You are a registered student in this course.</p>
   <p data-bind="visible: isSiteAdmin" style="display:none">You are an administrator for this site.</p>

    <h2>Faculty Members</h2>
    <table border="1" cellspacing="0" cellpadding="10">
        <thead style='background-color:gray;color:white'>
            <tr>
                <th>Name</th>
                <th>Login</th>
            </tr>
        </thead>
        <tbody data-bind="foreach: faculty">
            <tr>
                <td data-bind="text: name"></td>
                <td data-bind="text: email"></td>
            </tr>    
        </tbody>
    </table>

    <h2>Students</h2>
    <table border="1" cellspacing="0" cellpadding="10">
        <thead style='background-color:gray;color:white'>
            <tr>
                <th>Name</th>
                <th>Login</th>
            </tr>
        </thead>
        <tbody data-bind="foreach: students">
            <tr>
                <td data-bind="text: name"></td>
                <td data-bind="text: email"></td>
            </tr>    
        </tbody>
    </table>
</asp:Content>
