<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>
<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" language="C#" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
	<script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js"></script>
	<script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
	<script type="text/javascript" src="/_layouts/15/sp.js"></script>

	<!-- Add your CSS styles to the following file -->
	<link rel="Stylesheet" type="text/css" href="../Content/App.css" />

	<!-- Add your JavaScript to the following file -->
	<script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
	Classroom Online
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderMain" runat="server">

	<div id=”menu”>
		<ul>
		  <li><a href='../Lists/Configuration Values'>Site Configuration</a></li>
		  <li><a href='../Lists/Site Assets'>Site Assets</a></li>
		</ul>
	</div>

</asp:Content>
