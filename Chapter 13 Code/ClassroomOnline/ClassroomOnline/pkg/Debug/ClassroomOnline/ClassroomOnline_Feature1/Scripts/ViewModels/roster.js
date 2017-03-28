
$(function () {
    CO.Configs.ensureConfigs(
        function () {
            ko.applyBindings(new RosterViewModel());
        })
})
  
function RosterViewModel() {
    var self = this;

    // Note: none of these properties are observables because this page is read-only.
    self.faculty = CO.Configs.faculty;
    self.students = CO.Configs.students;
    self.isSiteAdmin = CO.Configs.isCurrentUserSiteAdmin;
    self.isFaculty = CO.Configs.isCurrentUserFaculty;
    self.isStudent = CO.Configs.isCurrentUserStudent;

    self.courseName = CO.Configs.CourseName; 
    self.courseNumber = CO.Configs.CourseNumber;

    self.faculty = [{ name: "Professor Moriarty", email: "jmoriarty@underhill.edu" }, { name: "Raj Bashir", email: "rbashir56@gmail.com" }];
    self.students =
        [  { name: "Penny Templeton", email: "pt@templeton.org" }
         , { name: "Howard Licten", email: "howard@hotmail.com" }
         , { name: "Leonard Nice", email: "lnice@underhill.edu" }
         , { name: "Sheldon Rathbone", email: "sheldore@gmail.com" }
        ]
}
