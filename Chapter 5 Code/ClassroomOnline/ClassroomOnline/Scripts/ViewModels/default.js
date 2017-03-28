
$(function () {
    CO.Configs.ensureConfigs(onConfigLoaded);
})

function onConfigLoaded() {
    $("#schoolLogo").attr("src", CO.Configs.OrganizationLogoUrl);
    $("#schoolName").text(CO.Configs.OrganizationName);
    
    // replace the standard token placeholder in any links that require tokens
    updateLinkTokens("StandardTokens", CO.Configs.StandardTokens);
}

// This function looks for a token surrounded by braces {} and
// replaces it with the string value provided.
function updateLinkTokens(token, value) {
    $("a[href *= '{" + token + "}']")
        .each(
            function (ind, a) {
                $(a).attr("href",
                          $(a).attr("href").replace("{" + token + "}",
                                                    value));
            });
}