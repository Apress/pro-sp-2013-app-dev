
$(function () {
    ko.applyBindings(new userProfileProps());
});

function userProfileProps() {
    var self = this;
    self._props = null;
    self.userProps = ko.observable();
    self._currentUser = null;
    self.currentUser = ko.observable();

    self.load = function () {
        var context = new SP.ClientContext.get_current();
        self._currentUser = context.get_web().get_currentUser();
        context.load(self._currentUser);

        var pm = new SP.UserProfiles.PeopleManager(context);
        self._props = pm.getMyProperties();
        context.load(self._props);

        context.executeQueryAsync(
            Function.createDelegate(self, self.onSuccess),
            Function.createDelegate(self, self.onFail)
            );
    }

    self.onSuccess = function () {
        self.currentUser(self._currentUser);
        self.userProps(self._props.get_userProfileProperties());
    }

    self.onFail = function (sender, args) {
        alert("Unable to access user profile: " + args.get_message());
    }

    self.load();
}
