
$(function () {
    ko.applyBindings(new testPermissionsViewModel());
});

function testPermissionsViewModel() {
    var self = this;
    self.result = null;

    self.readList = function () {
        var context = new SP.ClientContext.get_current();
        var hostWebContext =
            new SP.AppContextSite(
                context, decodeURIComponent(getQueryStringParameter("SPHostUrl")));

        self.result =
            hostWebContext.get_web().get_lists().getByTitle("Documents")
                .getItems(new SP.CamlQuery.createAllItemsQuery());
        context.load(self.result);

        context.executeQueryAsync(
            Function.createDelegate(self, self.onSuccess),
            Function.createDelegate(self, self.onFail)
            );
    }

    self.writeList = function () {
        var context = new SP.ClientContext.get_current();
        var hostWebContext =
            new SP.AppContextSite(
                context, decodeURIComponent(getQueryStringParameter("SPHostUrl")));

        self.result =
            hostWebContext.get_web().get_lists().getByTitle("Documents")
                .getItemById(1);
        self.result.set_item("Title", "My Updated Book Title");
        self.result.update();

        context.executeQueryAsync(
            Function.createDelegate(self, self.onSuccess),
            Function.createDelegate(self, self.onFail)
            );
    }

    self.onSuccess = function () {
        if (self.result instanceof SP.ListItemCollection) {
            if (!self.result.get_item(0))
                alert("Success, but nothing returned!");
            else
                alert("Success! First Title = " + self.result.get_item(0).get_item("Title"));
        }
        else
            alert("Success!");
    }

    self.onFail = function (sender, args) {
        alert("Failed: " + args.get_message());
    }
}

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