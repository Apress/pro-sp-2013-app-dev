var collListItems;

$(document).ready(function () {
    getConfigValues();
});

function getConfigValues() {
    var context = SP.ClientContext.get_current();
    var configList = context.get_web().get_lists().getByTitle('Configuration Values');
    var camlQuery = new SP.CamlQuery();
    collListItems = configList.getItems(camlQuery);
    context.load(collListItems);
    context.executeQueryAsync(onGetConfigValuesSuccess, onGetConfigValuesFail);
}

function onGetConfigValuesSuccess() {
    var OrgLogoUrl;
    var OrgName;
    var listItemEnumerator = collListItems.getEnumerator();

    while (listItemEnumerator.moveNext()) {
        var oListItem = listItemEnumerator.get_current();
        var current = oListItem.get_item('Title');
        switch (current) {
            case 'OrganizationName':
                OrgName = oListItem.get_item('Value');
                break;
            case 'OrganizationLogoUrl':
                OrgLogoUrl = oListItem.get_item('Value');
                break;
        };
    }

    if (OrgName && OrgName.length > 0) {
        $('#DeltaPlaceHolderPageTitleInTitleArea').html(OrgName);
        $('.ms-siteicon-img').attr('title', OrgName);
    }

    if (OrgLogoUrl && OrgLogoUrl.length > 0)
        $('.ms-siteicon-img').attr('src', OrgLogoUrl);
    else
        $('.ms-siteicon-img').attr('src', '../Images/AppLogo.png');
}

function onGetConfigValuesFail(sender, args) {
    alert('Failed to get the Configuration Values. Error:' + args.get_message());
}
