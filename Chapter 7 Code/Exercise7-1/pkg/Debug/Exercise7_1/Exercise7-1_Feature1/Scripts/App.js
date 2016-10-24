
var appweburl = decodeURIComponent(getQueryStringParameter("SPAppWebUrl"));
var hostweburl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));

$(function () { ko.applyBindings(new defaultViewModel()); });

function defaultViewModel() {
    var self = this;

    self.status = ko.observable();
    self.message = ko.observable();
    self.url = ko.observable("/_api/web");
    self.format = ko.observable();

    self.result = null;

    self.onRunRequest = function () {
        jQuery.ajax({
            url: appweburl + self.url(),
            type: "GET",
            headers: {
                "accept": self.format(),
            },
            success: Function.createDelegate(self, self.onComplete),
            error: Function.createDelegate(self, self.onComplete)
        });
    };

    self.onComplete = function (data, status) {
        self.status(status);

        if (self.format() == 'application/atom-xml') {
            self.result = data;
            self.message((new XMLSerializer()).serializeToString(data));
        } else {
            self.result = data.d;
            self.message(JSON.stringify(data));
        }
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