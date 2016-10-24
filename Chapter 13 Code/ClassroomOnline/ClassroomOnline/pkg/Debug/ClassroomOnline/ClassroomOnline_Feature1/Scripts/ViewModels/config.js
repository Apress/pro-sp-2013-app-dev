
$(function () {
    CO.Configs.ensureConfigs(
        function () {
            if (!CO.Configs.isCurrentUserSiteAdmin) {
                alert("Access Denied!");
                window.history.back();
            }
            else
                ko.applyBindings(new ConfigViewModel());
        });
})

function ConfigViewModel() {
    var self = this;
    self.OrganizationName = ko.observable(CO.Configs.OrganizationName);
    self.OrganizationLogoUrl = ko.observable(CO.Configs.OrganizationLogoUrl);
    self.CourseName = ko.observable(CO.Configs.CourseName);
    self.CourseNumber = ko.observable(CO.Configs.CourseNumber);
    self.Schedule = ko.observable(CO.Configs.Schedule);
    self.Location = ko.observable(CO.Configs.Location);

    self.siteGroups = [];
    var listEnumerator = CO.Configs.siteGroups.getEnumerator();
    while (listEnumerator.moveNext()) {
        var grp = listEnumerator.get_current();
        var grpItem = { groupID: grp.get_id(), groupName: grp.get_title() }
        self.siteGroups.push(grpItem);

        if (grp.get_id() == CO.Configs.facultyGroupID)
            self.facultyGroup = ko.observable(grpItem);
        if (grp.get_id() == CO.Configs.studentGroupID)
            self.studentGroup = ko.observable(grpItem);
    }

    self.updateConfigs = function () {
        CO.Configs.OrganizationName = self.OrganizationName();
        CO.Configs.OrganizationLogoUrl = self.OrganizationLogoUrl();
        CO.Configs.CourseName = self.CourseName();
        CO.Configs.CourseNumber = self.CourseNumber();
        CO.Configs.Schedule = self.Schedule();
        CO.Configs.Location = self.Location();
        CO.Configs.facultyGroupID = self.facultyGroup().groupID;
        CO.Configs.studentGroupID = self.studentGroup().groupID;

        CO.Configs.saveConfigs(
            function () {
                location.href = "Default.aspx?" + CO.Configs.StandardTokens;
            });
    }
}
