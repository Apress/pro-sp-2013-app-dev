<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="app.master" Language="C#" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.2.1.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>

    <script type="text/javascript" src="../Scripts/Models/siteconfigs.js"></script>
    <script type="text/javascript" src="../Scripts/Models/grades.js"></script>
    <script type="text/javascript" src="../Scripts/ViewModels/gradebook.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    Course Gradebook
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderMain" runat="server"> 

    <table border="1" cellspacing="0" cellpadding="5">
        <thead style='background-color:gray;color:white'>
            <th>Student</th>
            <th>Assignment</th>
            <th>Points Possible</th>
            <th>Course Weight (%)</th>
            <th>Points Acheived</th>
            <th class='buttonColumn'></th>
        </thead>
        <tbody data-bind="foreach: gradebookEntries()">
            <tr>
                <td data-bind="text: studentName"></td>
                <td data-bind="text: assignmentTitle"></td>
                <td data-bind="text: pointsPossible"></td>
                <td data-bind="text: percentOfGrade"></td>
                <td data-bind="text: pointsAchieved"></td>
                <td class='buttonColumn'>
                    <a href="#" data-bind="click: $root.onEdit">Edit</a>
                    <a href="#" data-bind="click: $root.onDelete">Delete</a>
                </td>
            </tr>    
            <tr style="display:none">

                <td><input data-bind="value: studentName" /></td>
                <td><input data-bind="value: assignmentTitle" /></td>
                <td><input data-bind="value: pointsPossible" /></td>
                <td><input data-bind="value: percentOfGrade" /></td>
                <td><input data-bind="value: pointsAchieved" /></td>
                <td class='buttonColumn'>
                    <a href="#" data-bind="click: $root.onSave">Save</a>
                    <a href="#" data-bind="click: $root.onCancel">Cancel</a>
                </td>
            </tr>
       </tbody>
        <tfoot id="addLine">
            <tr data-bind="with: addItem">
                <td><input data-bind="value: studentName" /></td>
                <td><input data-bind="value: assignmentTitle" /></td>
                <td><input data-bind="value: pointsPossible" /></td>
                <td><input data-bind="value: percentOfGrade" /></td>
                <td><input data-bind="value: pointsAchieved" /></td>
                <td class='buttonColumn'>
                    <a href="#" data-bind="click: $root.onAdd">Add</a>
                </td>
            </tr>
        </tfoot>
    </table>

</asp:Content>









