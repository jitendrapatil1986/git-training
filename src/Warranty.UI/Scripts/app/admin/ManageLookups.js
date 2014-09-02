require(['/Scripts/app/main.js'], function () {
    require(['modelData', 'jquery', 'ko', 'urls'], function (modelData, $, ko, urls) {
        function manageLookupsViewModel() {
            var self = this;
            self.availableLookups = ko.observableArray(modelData.availableLookups);
            self.selectedLookup = ko.observable('');

            self.lookupType = ko.computed(function() {
                return this.selectedLookup;
            });
            self.lookupItems = ko.observableArray([]);

            self.removeItem = function (item) {
                self.lookupItems.remove(item);
            };

            self.selectionChanged = function () {
                //self.add$.get(urls.)
            };
        }

        var viewModel = new manageLookupsViewModel();
        ko.applyBindings(viewModel);
    });
});