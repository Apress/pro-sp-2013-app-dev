<%@ Page 
    Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" 
    MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" 
    Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" 
    Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" 
    Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/knockout/knockout-2.2.1.js" ></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.userprofiles.debug.js"></script>

    <script type="text/javascript" src="../Scripts//App.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    User Information
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMain" runat="server"> 
    <h2>Current User Properties</h2>
    <table data-bind="with:currentUser">
        <tr><td>title</td><td data-bind="text: get_title()"></td></tr>
        <tr><td>Id</td><td data-bind="text: get_id()"></td></tr>
        <tr><td>loginName</td><td data-bind="text: get_loginName()"></td></tr>
        <tr><td>email</td><td data-bind="text: get_email()"></td></tr>
        <tr><td>isSiteAdmin</td><td data-bind="text: get_isSiteAdmin()"></td></tr>
    </table>
        <br />
    <h2>User Profile Properties</h2>
    <table data-bind="with:userProps">
        <tr><td>AccountName</td><td data-bind="text: AccountName"></td></tr>
        <tr><td>UserName</td><td data-bind="text: UserName"></td></tr>
        <tr><td>FirstName</td><td data-bind="text: FirstName"></td></tr>
        <tr><td>LastName</td><td data-bind="text: LastName"></td></tr>
        <tr><td>PreferredName</td><td data-bind="text: PreferredName"></td></tr>
        <tr><td>WorkEmail</td><td data-bind="text: WorkEmail"></td></tr>
        <tr><td>WorkPhone</td><td data-bind="text: WorkPhone"></td></tr>
        <tr><td>PictureURL</td><td><img src="#" data-bind="attr: {src: PictureURL}" /></td></tr>
    </table>
</asp:Content>
