
$(function () {
    CO.Configs.ensureConfigs(onConfigLoaded);
})

function onConfigLoaded() {
    $("#schoolLogo").attr("src", CO.Configs.OrganizationLogoUrl);
    $("#schoolName").text(CO.Configs.OrganizationName);

    if (CO.Configs.isCurrentUserSiteAdmin) {
        $(".adminOnly").show();
        updateLinkTokens("FacultyGrpID", CO.Configs.facultyGroupID);
        updateLinkTokens("StudentGrpID", CO.Configs.studentGroupID);
   }

    // replace the standard token placeholder in any links that require tokens
    updateLinkTokens("StandardTokens", CO.Configs.StandardTokens);
}

// Utility Function
function updateLinkTokens(token, value) {
    $("a[href *= '{" + token + "}']")
        .each(
            function (ind, a) {
                $(a).attr("href",
                          $(a).attr("href").replace("{" + token + "}",
                                                    value));
            });
}