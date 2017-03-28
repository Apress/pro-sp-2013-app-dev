
$(function () {
    CO.Configs.ensureConfigs(
        function () {
            ko.applyBindings(new gradebookViewModel(!CO.Configs.isCurrentUserSiteAdmin));
        });
})

function gradebookViewModel(viewAsStudent) {
    var self = this;

    self.model = new CO_Grades();
    self.eventItem = null;
    self.eventTarget = null;

    self.viewAsStudent = viewAsStudent;
    self.gradebookEntries = ko.observableArray();
    self.addItem = ko.observable(new gradebookEntry('', '', '', '', '',0,''));

    self.onLoad = function () {
        // render a read-only version of the form for students
        if (self.viewAsStudent) {
            $('.buttonColumn').hide();
            $('#addLine').hide();

            self.model.getGrades(
                Function.createDelegate(self, self.onLoadComplete),
                CO.Configs.currentUser.name);
        }
        else // render all grades for the faculty
            self.model.getGrades(
                Function.createDelegate(self, self.onLoadComplete),
                null);
    };
    self.onLoadComplete = function (gradeList) {
        for (var i = 0; i < gradeList.length; i++)
            self.gradebookEntries.push(
                new gradebookEntry(gradeList[i].StudentName, gradeList[i].Title, gradeList[i].PointsPossible,
                                   gradeList[i].PercentofCourseGrade, gradeList[i].PointsAchieved,
                                   gradeList[i].Id, gradeList[i].__metadata.etag));
    }

    self.onAdd = function (sender, e) {
        if (self.addItem().isValid()) {
            self.model.addGrade(sender.studentName(), sender.assignmentTitle(), sender.pointsPossible(),
                                sender.percentOfGrade(), sender.pointsAchieved(),
                                Function.createDelegate(self, self.onAddComplete));
        }
    };
    self.onAddComplete = function (id, etag) {
        // Copy the new ID and ETag so that future updates can occur.
        self.addItem().id = id;
        self.addItem().etag = etag;

        var item = self.addItem();
        self.addItem(new gradebookEntry('', '', '', '', '', 0, ''));
        self.gradebookEntries.push(item);
    }

    self.onEdit = function (sender, e) {
        sender.backupValues();
        $(e.target).parent().parent().hide().next().show();
    };

   self.onSave = function (sender, e) {
        if (sender.isValid()) {
            self.eventItem = sender;
            self.eventTarget = e.target;

            self.model.updateGrade(sender.studentName(), sender.assignmentTitle(), sender.pointsPossible(),
                                   sender.percentOfGrade(), sender.pointsAchieved(),
                                   sender.id, sender.etag,
                                   Function.createDelegate(self, self.onSaveComplete));
        }
    };
    self.onSaveComplete = function (id, etag) {
        // Copy the ETag so that future updates can occur. The ID didn't change.
        self.eventItem.etag = etag;

        $(self.eventTarget).parent().parent().hide().prev().show();
    }

    self.onCancel = function (sender, e) {
        sender.restoreValues();
        $(e.target).parent().parent().hide().prev().show();
    };

    self.onDelete = function (sender, e) {
        //self.eventItem = sender;
        //self.eventTarget = e.target;

        //self.model.deleteGrade(sender.id, sender.etag,
        //                       Function.createDelegate(self, self.onDeleteComplete));
    };
    //self.onDeleteComplete = function (data) {
    //    self.gradebookEntries.remove(self.eventItem);
    //}

 
    self.onLoad();
}

function gradebookEntry(studentName, assignmentTitle, pointsPossible, percentOfGrade, pointsAchieved, id, etag) {
    var self = this;
    self.studentName = ko.observable(studentName);
    self.assignmentTitle = ko.observable(assignmentTitle);
    self.pointsPossible = ko.observable(pointsPossible);
    self.percentOfGrade = ko.observable(percentOfGrade);
    self.pointsAchieved = ko.observable(pointsAchieved);
    self.id = id;
    self.etag = etag;

    self.isValid = function () {
        if (self.studentName() == '')
            { alert("Please enter the student's name."); return 0; }
        if (self.assignmentTitle() == '')
            { alert("Please enter the assignment's title."); return 0; }
        if ((self.pointsPossible() == '') || (self.pointsPossible() != parseInt(self.pointsPossible()))) 
            { alert("Please enter a valid number of points possible."); return 0; }
        if ((self.percentOfGrade() == '') || (self.percentOfGrade() != parseFloat(self.percentOfGrade()))) 
            { alert("Please enter a valid percentage."); return 0; }
        if ((self.pointsAchieved() == '') || (self.pointsAchieved() != parseInt(self.pointsAchieved()))) 
            { alert("Please enter a valid number of points achieved."); return 0; }
        return 1;
    }

    self.backupValues = function () {
        self._studentName = self.studentName();
        self._assignmentTitle = self.assignmentTitle();
        self._pointsPossible = self.pointsPossible();
        self._percentOfGrade = self.percentOfGrade();
        self._pointsAchieved = self.pointsAchieved();
    }

    self.restoreValues = function () {
        self.studentName(self._studentName);
        self.assignmentTitle(self._assignmentTitle);
        self.pointsPossible(self._pointsPossible);
        self.percentOfGrade(self._percentOfGrade);
        self.pointsAchieved(self._pointsAchieved);
    }
}
