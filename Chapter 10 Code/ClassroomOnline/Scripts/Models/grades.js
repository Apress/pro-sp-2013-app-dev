
// Data Model Object for the gradebook
CO_Grades = function () {
    var self = this;

    self.getGrades = function (callback, studentName) {
        var restUrl = CO.Configs.SPAppWebUrl
                + "/_api/web/lists/getbytitle('Grades')/items"
                + "?$orderby=Created desc"
                + "&$select=ID,StudentName,Title,PointsPossible,PercentofCourseGrade,PointsAchieved";

        // If a student is given, read only their grades
        if (studentName) 
            restUrl += '&$filter=StudentName eq \'' + studentName + '\'';
        
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
            error: function () {
                alert('Unable to load grades.');
            }
        });
    };

    self.addGrade = function (studentName, assignmentTitle, pointsPossible, percentofCourseGrade, pointsAchieved, callback) {
        var body = {
            '__metadata': { 'type': 'SP.Data.GradesListItem' },
            'Title': assignmentTitle,
            'StudentName': studentName,
            'PointsPossible': pointsPossible,
            'PercentofCourseGrade': percentofCourseGrade,
            'PointsAchieved': pointsAchieved
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
                callback(data.d.Id, data.d.__metadata.etag);
            },
            error: function (err) {
                alert('Unable to add the grade.');
            }
        });
    }

    self.updateGrade = function (studentName, assignmentTitle, pointsPossible, percentofCourseGrade, pointsAchieved, id, etag, callback) {
        var body = {
            '__metadata': { 'type': 'SP.Data.GradesListItem' },
            'Title': assignmentTitle,
            'StudentName': studentName,
            'PointsPossible': pointsPossible,
            'PercentofCourseGrade': percentofCourseGrade,
            'PointsAchieved': pointsAchieved
        };
        body = JSON.stringify(body);

        jQuery.ajax({
            url: CO.Configs.SPAppWebUrl + "/_api/web/lists/getbytitle('Grades')/items/getbyid('" + id.toString() + "')",
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
                callback(id, resp.getResponseHeader("ETAG"));
            },
            error: function (err) {
                alert('Unable to update the grade.');
            }
        });
    }

    self.deleteGrade = function (id, etag, callback) {
        jQuery.ajax({
            url: CO.Configs.SPAppWebUrl
                 + "/_api/web/lists/getbytitle('Grades')/items/getbyid('" + id.toString() + "')",
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