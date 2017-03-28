<%@ Page language="C#" MasterPageFile="app.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.2.1.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>

    <script type="text/javascript" src="../Scripts/Models/siteconfigs.js"></script>
    <script type="text/javascript" src="../Scripts/ViewModels/hwlist.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    Homework File List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <div>
        <h2>...Working...</h2>
        <table border="1" cellspacing="0" cellpadding="10" style="display:none">
            <thead style='background-color:gray;color:white'>
                <tr>
                    <td>Date Submitted</td>
                    <td>Student</td>
                    <td>Title</td>
                    <td>Size</td>
                </tr>
            </thead>
            <tbody data-bind="foreach: assignments">
                <tr>
                    <td data-bind="text: dateSubmitted"></td>
                    <td data-bind="text: studentName"></td>
                    <td>
                        <a href="#" data-bind="attr: { href: url, title: title }, text: title ? title : url"></a>
                    </td>
                    <td data-bind="text: size"></td>
                </tr>          
            </tbody>
        </table>
    </div>
</asp:Content>
