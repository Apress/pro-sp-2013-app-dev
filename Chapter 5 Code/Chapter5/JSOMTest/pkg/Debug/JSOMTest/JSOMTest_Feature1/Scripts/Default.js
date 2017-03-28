
$(function () {
    ko.applyBindings(new defaultViewModel());
});

function defaultViewModel() {
    var self = this;
    self.message = ko.observable();

    self.startLoad = function () {
        self.context = SP.ClientContext.get_current();

        self.web = self.context.get_web();
        context.load(self.web); // adds a "load" request for the web object to the next batch

        self.user = self.web.get_currentUser();
        context.load(self.user); // adds a "load" request for the user object to the next batch

        // Send the request batch to the server
        self.context.executeQueryAsync(
            Function.createDelegate(self, self.onGetUserNameSuccess),
            Function.createDelegate(self, self.onGetUserNameFail)
            );
    }

    self.onGetUserNameSuccess = function () {
        self.message('Hello ' + self.user.get_title() + ' from ' + self.web.get_title());
    }

    self.onGetUserNameFail = function (sender, args) {
        self.message('Failed to get user name. Error:' + args.get_message());
    }

    self.startLoad();
}
