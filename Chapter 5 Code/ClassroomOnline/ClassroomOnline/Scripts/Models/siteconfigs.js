
var CO = CO || {};

$(function() {
    CO.Configs = new CO_Configs(); 
})

// Data Model Object for the app's configuration
CO_Configs = function () {
    var self = this;
    self._loadComplete = 0;
    self._callbacks = []; // holds a list of callbacks during page load.
    self._pendingItems;   // holds a collection of client objects for the configuration list.

    self.ensureConfigs = function (callback) {
        if (self._loadComplete)
            callback();
        else
            self._callbacks.push(callback);
    }

    self._loadConfigs = function () {
        var clientContext = SP.ClientContext.get_current();

        var list = clientContext.get_web().get_lists().getByTitle('Configuration Values');
        self._pendingItems = list.getItems(new SP.CamlQuery.createAllItemsQuery());
        clientContext.load(self._pendingItems);

        self._currentWeb = clientContext.get_web();
        clientContext.load(self._currentWeb);

        self.siteGroups = self._currentWeb.get_siteGroups();
        clientContext.load(self.siteGroups, 'Include(Title,Id)');

        self._currentUser = self._currentWeb.get_currentUser();
        clientContext.load(self._currentUser);

        clientContext.executeQueryAsync(
            Function.createDelegate(self, self._onConfigLoadSucceeded),
            Function.createDelegate(self, self._onConfigLoadFailed)
            );
    }

    self._onConfigLoadSucceeded = function (sender, args) {
        // Load configuration values from the list
        var listEnumerator = self._pendingItems.getEnumerator();
        while (listEnumerator.moveNext()) {
            var item = listEnumerator.get_current();
            eval("self." + item.get_item("Title") + " = '" + item.get_item("Value") + "'");
        }

        // replace the site logo with the school logo.
        $(".ms-siteicon-img").attr("src", self.OrganizationLogoUrl).show();

        // Get tokens from the URL
        self.SPAppWebUrl = decodeURIComponent(getQueryStringParameter("SPAppWebUrl"));
        self.SPLanguage = decodeURIComponent(getQueryStringParameter("SPLanguage"));
        self.SPClientTag = decodeURIComponent(getQueryStringParameter("SPClientTag"));
        self.SPProductNumber = decodeURIComponent(getQueryStringParameter("SPProductNumber"));
        self.SPHostUrl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));
        self.scriptBase = self.SPHostUrl + "/_layouts/15/";
        self.StandardTokens = document.URL.split("?")[1];
        
        self._loadComplete = 1;
        while (self._callbacks.length > 0)
            self._callbacks.pop()();
    }
      
    self._onConfigLoadFailed = function (sender, args) {
        alert('Unable to load app configuration: ' + args.get_message() + '\n' + args.get_stackTrace());
    }

    self.saveConfigs = function(callback) {
        if (callback) self._callbacks.push(callback);

        var clientContext = SP.ClientContext.get_current();
        var list = clientContext.get_web().get_lists().getByTitle('Configuration Values');

        var item;
        item = list.getItemById(1);
        item.set_item('Title', 'OrganizationName');
        item.set_item('Value', self.OrganizationName);
        item.update();

        item = list.getItemById(2);
        item.set_item('Title', 'OrganizationLogoUrl');
        item.set_item('Value', self.OrganizationLogoUrl);
        item.update();

        item = list.getItemById(3);
        item.set_item('Title', 'CourseName');
        item.set_item('Value', self.CourseName);
        item.update();

        item = list.getItemById(4);
        item.set_item('Title', 'CourseNumber');
        item.set_item('Value', self.CourseNumber);
        item.update();

        item = list.getItemById(5);
        item.set_item('Title', 'Schedule');
        item.set_item('Value', self.Schedule);
        item.update();

        item = list.getItemById(6);
        item.set_item('Title', 'Location');
        item.set_item('Value', self.Location);
        item.update();

        clientContext.executeQueryAsync(
            Function.createDelegate(self, self._onConfigSaveSucceeded),
            Function.createDelegate(self, self._onConfigSaveFailed)
            );
    }

    self._onConfigSaveSucceeded = function(sender, args) {
        // replace the site logo with the school logo.
        $(".ms-siteicon-img").attr("src", self.OrganizationLogoUrl).show();

        while (self._callbacks.length > 0)
            self._callbacks.pop()();
    }

    self._onConfigSaveFailed = function(sender, args) {
        alert('Save failed. ' + args.get_message() + '\n' + args.get_stackTrace());
    }

    // Start loading the configuration.
    self._loadConfigs();
}

// Utility routine
function getQueryStringParameter(paramToRetrieve) {
    var params =
        document.URL.split("?")[1].split("&");
    var strParams = "";
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split("=");
        if (singleParam[0] == paramToRetrieve)
            return singleParam[1];
    }
}