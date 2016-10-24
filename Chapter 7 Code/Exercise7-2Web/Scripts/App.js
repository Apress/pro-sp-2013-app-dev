
var appweburl = decodeURIComponent(getQueryStringParameter("SPAppWebUrl"));
var hostweburl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));

$().ready(function () {
    $.getScript(hostweburl + '/_layouts/15/sp.runtime.debug.js',
                function () {
                    $.getScript(hostweburl + '/_layouts/15/sp.debug.js',
                                function () {
                                    $.getScript(hostweburl + '/_layouts/15/sp.RequestExecutor.js',
                                                function () {
                                                    ko.applyBindings(new defaultViewModel());
                                                });
                                })
                })
});

function defaultViewModel() {
    var self = this;

    self.status = ko.observable();
    self.message = ko.observable();
    self.url = ko.observable("/_api/SP.AppContextSite(@target)/web/lists?@target='" + hostweburl + "'");
    self.format = ko.observable();

    self.result = null;

    self.onRunRequest = function () {
        var executor = new SP.RequestExecutor(appweburl);
        executor.executeAsync(
            {
                url: appweburl + self.url(),
                method: "GET",
                headers: {
                    "accept": self.format(),
                },
                success: Function.createDelegate(self, self.onComplete),
                error: Function.createDelegate(self, self.onComplete)
            }
        );
    };

    self.onComplete = function (data) {
        self.status(data.statusText);
        self.message(data.body);

        if (self.format() == 'application/atom-xml')
            self.result = $(data.body)[1];
        else
            self.result = JSON.parse(data.body).d;
    }
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