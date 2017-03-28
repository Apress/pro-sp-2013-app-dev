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
    <script type="text/javascript" src="../Scripts/ViewModels/config.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    Settings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <div id="configvaluescontainer">
        <table>
            <tbody>
                <tr>
                    <td>School Name</td>
                    <td><input data-bind="value: OrganizationName" size="30"/></td>
                </tr>
                <tr>
                    <td>URL of School Logo</td>
                    <td><input data-bind="value: OrganizationLogoUrl" size="30"/></td>
                </tr>
                <tr>
                    <td>Course Name</td>
                    <td><input data-bind="value: CourseName" size="30"/></td>
                </tr>
                <tr>
                    <td>Course Number</td>
                    <td><input data-bind="value: CourseNumber" size="15"/></td>
                </tr>
                <tr>
                    <td>Schedule</td>
                    <td><input data-bind="value: Schedule" size="15"/></td>
                </tr>
                <tr>
                    <td>Location</td>
                    <td><input data-bind="value: Location" size="15"/></td>
                </tr>
                <tr>
                    <td>Faculty Group</td>
                    <td><select data-bind="options: siteGroups, value: facultyGroup, optionsText: 'groupName'"></select></td>
                </tr>
                <tr>
                    <td>Student Group</td>
                    <td><select data-bind="options: siteGroups, value: studentGroup, optionsText: 'groupName'"></select></td>
                </tr>
            </tbody>
        </table>
        <a href="#" data-bind="click: updateConfigs">Update Configurations</a>
    </div>
</asp:Content>
