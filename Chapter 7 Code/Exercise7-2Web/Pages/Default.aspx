<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Exercise7_2Web.Pages.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/knockout/knockout-2.2.1.js" ></script>

    <script type="text/javascript" src="../Scripts/App.js"></script>
</head>
<body>
    <form id="form1" runat="server">
      <div>
            <input type="text" data-bind="value: url" size="100" />
            <br />
            <br />
            <select data-bind="value: format">
                <option value="application/json;odata=verbose">application/json;odata=verbose</option>
                <option value="application/atom-xml">application/atom-xml</option>
            </select>
            <br />
            <br />        
            <input data-bind="click: onRunRequest" type="button" value="Execute the REST Request" />
            <br />
            <br />
            <h1 data-bind="text: status"></h1>
            <p data-bind="text: message" />
        </div>
    </form>
</body>
</html>
