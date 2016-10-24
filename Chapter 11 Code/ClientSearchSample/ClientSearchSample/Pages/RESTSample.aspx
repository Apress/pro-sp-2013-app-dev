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
    var appWebURL = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

    $(function () { $("#cmdSearch").click(onSearch); });

    function onSearch() {
        var queryUrl = appWebURL + "/_api/search/query?querytext='" + $("#txtSearchText").val() + "'";

        // No tokens or digests required since we are in the app web.
        $.ajax(
            {
                url: queryUrl,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                success: onQuerySuccess,
                error: onQueryFail
            }
        );
    }

    function onQuerySuccess(data) {
        $("#results").empty();

        if (data.d.query.PrimaryQueryResult != null)
            $.each(data.d.query.PrimaryQueryResult.RelevantResults.Table.Rows.results, function () {
                $("#results").append('<tr>');
                $("#results").append('<td>' + getValue(this, 'Title') + '</td>');
                $("#results").append('<td>' + getValue(this, 'Author') + '</td>');
                $("#results").append('<td>' + getValue(this, 'Write') + '</td>');
                $("#results").append('<td>' + getValue(this, 'Path') + '</td>');
                $("#results").append('</tr>');
            });
    }

    function getValue(row, fldName) {
        var ret = null;
        $.each(row.Cells.results, function () {
            if (this.Key == fldName) {
                ret = this.Value;
            }
        });
        return ret;
    }

    function onQueryFail(sender, args) {
        $("#results").append('Query failed. Error:' + args.get_message());
    }

    function getQueryStringParameter(urlParameterKey) {
        var params = document.URL.split('?')[1].split('&');
        var strParams = '';
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split('=');
            if (singleParam[0] == urlParameterKey)
                return decodeURIComponent(singleParam[1]);
        }
    }
</script>
</asp:Content>
