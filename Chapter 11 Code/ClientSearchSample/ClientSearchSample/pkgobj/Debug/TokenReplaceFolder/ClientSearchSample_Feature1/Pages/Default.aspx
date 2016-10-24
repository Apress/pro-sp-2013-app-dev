<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.search.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Page Title
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div>
    Search:
    <input id="txtSearchText" type="text" size="40" />
    <input id="cmdSearch" type="button" value="Start Search" />
</div> 

<p>
    <table border="1" cellpadding="5" cellspacing="0">
        <thead>
            <tr>
                <th>Title</th>
                <th>Author</th>
                <th>Last Update</th>
                <th>URL</th>
            </tr>
        </thead>
        <tbody id="results">
        </tbody>
    </table>
</p>
        
<script type="text/javascript">
    var results;
    var context = SP.ClientContext.get_current();

    $(function () { $("#cmdSearch").click(onSearch); });

    function onSearch() {
        var query = new Microsoft.SharePoint.Client.Search.Query.KeywordQuery(context);
        query.set_queryText($("#txtSearchText").val());

        var exec = new Microsoft.SharePoint.Client.Search.Query.SearchExecutor(context);
        results = exec.executeQuery(query);

        context.executeQueryAsync(onQuerySuccess, onQueryFail)
    }

    function onQuerySuccess() {
        $("#results").empty();

        $.each(results.m_value.ResultTables[0].ResultRows, function () {
            $("#results").append('<tr>');
            $("#results").append('<td>' + this.Title + '</td>');
            $("#results").append('<td>' + this.Author + '</td>');
            $("#results").append('<td>' + this.Write + '</td>');
            $("#results").append('<td>' + this.Path + '</td>');
            $("#results").append('</tr>');
        });
    }

    function onQueryFail(sender, args) {
        $("#results").append('Query failed. Error:' + args.get_message());
    }
</script>
</asp:Content>
