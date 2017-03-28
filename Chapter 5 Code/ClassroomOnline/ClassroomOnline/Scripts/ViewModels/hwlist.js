
$(function () {
    CO.Configs.ensureConfigs(
        function () {
            ko.applyBindings(new HomeworkListViewModel());
        });
})

function HomeworkListViewModel() {
    var self = this;
    self.assignments = ko.observableArray();

    self._loadList = function () {
        var clientContext = SP.ClientContext.get_current();
        
        // This line accesses the homework library in the local app web.
        var list = clientContext.get_web().get_lists().getByTitle('Homework');

        // These lines are used to access the homework library in the host web instead of the app web
        //     var hostWebContext = new SP.AppContextSite(clientContext, CO.Configs.SPHostUrl);
        //     var list = hostWebContext.get_web().get_lists().getByTitle('Homework');

        var query = new SP.CamlQuery();
        query.set_viewXml("<View>"
                        + "  <Query>"
                        + "  <OrderBy>"
                        + "    <FieldRef Name='Modified' Ascending='False' />"
                        + "    <FieldRef Name='Title' Ascending='True' />"
                        + "  </OrderBy>"
                        + "  <Where>"
                        + "    <Geq>"
                        + "       <FieldRef Name='Modified' />"
                        + "       <Value Type='DateTime'>"
                        + "         <Today OffsetDays='-7' />"
                        + "       </Value>"
                        + "    </Geq>"
                        + "  </Where>"
                        + "  </Query>"
                        + "  <ViewFields>"
                        + "    <FieldRef Name='Title' />"
                        + "    <FieldRef Name='Modified' />"
                        + "    <FieldRef Name='Editor' />"
                        + "    <FieldRef Name='FileRef' />"
                        + "    <FieldRef Name='FileLeafRef' />"
                        + "    <FieldRef Name='File_x0020_Size' />"
                        + "  </ViewFields>"
                        + "  <RowLimit>100</RowLimit>"
                        + "</View>");

        self._pendingItems = list.getItems(query);
        clientContext.load(self._pendingItems);

        clientContext.executeQueryAsync(
            Function.createDelegate(self, self._onLoadListSucceeded),
            Function.createDelegate(self, self._onLoadListFailed)
            );
    }

    self._onLoadListSucceeded = function (sender, args) {
        var listEnumerator = self._pendingItems.getEnumerator();
        while (listEnumerator.moveNext()) {
            var item = listEnumerator.get_current().get_fieldValues();
            
            self.assignments.push(
                {
                    title: item.Title,
                    url: item.FileRef,
                    dateSubmitted: item.Modified.toString(),
                    studentName: item.Editor.get_lookupValue(),
                    size: item.File_x0020_Size
                });
        }

        $('h2').hide();
        $('table').show();
    }

    self._onLoadListFailed = function (sender, args) {
        alert('Unable to load file list: ' + args.get_message() + '\n' + args.get_stackTrace());
    }

    self._loadList();
}
