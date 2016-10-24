
// Data Model Object for the gradebook
CO_Grades = function () {
    var self = this;

    self.getGrades = function (callback, studentName) {
        var restUrl = CO.Configs.SPAppWebUrl
                + "/_api/web/lists/getbytitle('Grades')/items"
                + "?$orderby=EnteredDate desc"
                + "&$select=GradeID,StudentName,AssignmentTitle,PointsPossible,PercentOfCourseGrade,PointsAchieved,ClassroomURL"
                + '&$filter=ClassroomURL eq \'' + CO.Configs.SPHostUrl + '\'';

        // If a student is given, read only their grades
        if (studentName) 
            restUrl += ' and StudentName eq \'' + studentName + '\'';
        
        // query the grade list for the grades to display/edit
        jQuery.ajax({
            url: restUrl,
            type: "GET",
            headers: {
                "accept": 'application/json;odata=verbose',
            },
            success: function (data) {
                callback(data.d.results);
            },
            error: function (a,b,c) {
                alert('Unable to load grades.');
            }
        });
    };

    self.addGrade = function (studentName, assignmentTitle, pointsPossible, percentofCourseGrade, pointsAchieved, callback) {
        var d = new Date();
        d = (d.getMonth() + 1).toString()
            + "/" + d.getDate().toString()
            + "/" + d.getFullYear().toString()
            + " " + d.getHours().toString()
            + ":" + d.getMinutes().toString()
            + ":" + d.getSeconds().toString();
        
        var body = {
            '__metadata': { 'type': 'SP.Data.GradesListItem' },
            'ClassroomURL': CO.Configs.SPHostUrl,
            'AssignmentTitle': assignmentTitle,
            'StudentName': studentName,
            'PointsPossible': pointsPossible,
            'PercentOfCourseGrade': percentofCourseGrade,
            'PointsAchieved': pointsAchieved,
            'EnteredBy': CO.Configs.currentUser.name,
            'EnteredDate': d
        };
        body = JSON.stringify(body);

        jQuery.ajax({
            url: CO.Configs.SPAppWebUrl + "/_api/web/lists/getbytitle('Grades')/items",
            type: "POST",
            data: body,
            headers: {
                "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                "accept": "application/json;odata=verbose",
                "content-type": "application/json;odata=verbose",
                "content-length": body.length
            },
            success: function (data) {
                callback(data.d.__metadata.uri, '*');
            },
            error: function (err) {
                alert('Unable to add the grade.');
            }
        });
    }

    self.updateGrade = function (studentName, assignmentTitle, pointsPossible, percentofCourseGrade, pointsAchieved, id, etag, callback) {
        var d = new Date();
        d = (d.getMonth() + 1).toString()
            + "/" + d.getDate().toString()
            + "/" + d.getFullYear().toString()
            + " " + d.getHours().toString()
            + ":" + d.getMinutes().toString()
            + ":" + d.getSeconds().toString();

        var body = {
            '__metadata': { 'type': 'SP.Data.GradesListItem' },
            'ClassroomURL': CO.Configs.SPHostUrl,
            'AssignmentTitle': assignmentTitle,
            'StudentName': studentName,
            'PointsPossible': pointsPossible,
            'PercentOfCourseGrade': percentofCourseGrade,
            'PointsAchieved': pointsAchieved,
            'EnteredBy': CO.Configs.currentUser.name,
            'EnteredDate': d
        };
        body = JSON.stringify(body);

        jQuery.ajax({
            url: id, //CO.Configs.SPAppWebUrl + "/_api/web/lists/getbytitle('Grades')/items/getbyid('" + id.toString() + "')",
            type: "POST",
            data: body,
            headers: {
                "X-Http-Method": "PATCH",
                "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                "IF-MATCH" : etag,
                "accept": "application/json;odata=verbose",
                "content-type": "application/json;odata=verbose",
                "content-length": body.length
            },
            success: function (data,status,resp) {
                callback(id, '*');
            },
            error: function (err) {
                alert('Unable to update the grade.');
            }
        });
    }

    self.deleteGrade = function (id, etag, callback) {
        jQuery.ajax({
            url: id,
            type: "POST",
            headers: {
                "X-Http-Method": "DELETE",
                "X-RequestDigest" : $("#__REQUESTDIGEST").val(),
                "IF-MATCH" : etag
            },
            success: function () {
                callback();
            },
            error: function (err) {
                alert('Unable to delete the grade.');
            }
        });
    }
}