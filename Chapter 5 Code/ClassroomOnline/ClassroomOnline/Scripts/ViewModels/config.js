
$(function () {
    CO.Configs.ensureConfigs(
        function () {
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

    self.updateConfigs = function () {
        CO.Configs.OrganizationName = self.OrganizationName();
        CO.Configs.OrganizationLogoUrl = self.OrganizationLogoUrl();
        CO.Configs.CourseName = self.CourseName();
        CO.Configs.CourseNumber = self.CourseNumber();
        CO.Configs.Schedule = self.Schedule();
        CO.Configs.Location = self.Location();

        CO.Configs.saveConfigs(
            function () {
                location.href = "Default.aspx?" + CO.Configs.StandardTokens;
            });
    }
}
