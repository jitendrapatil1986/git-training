require(['/Scripts/app/main.js'], function () {
    require(['modelData', 'jquery', 'ko', 'urls', 'toastr'], function (modelData, $, ko, urls, toastr) {
        function manageLookupsViewModel() {
            var self = this;
            self.availableLookups = ko.observableArray(modelData.availableLookups);
            self.selectedLookup = ko.observable('');

            self.lookupType = ko.computed(function() {
                return this.selectedLookup;
            });
            self.lookupItems = ko.observableArray([]);

            self.selectionChanged = function () {
                self.lookupItems.removeAll();
                
                $.ajax({
                    url: urls.ManageLookups.LookupSubtableDetails,
                    cache: false,
                    data: {query: this.selectedLookup()},
                    dataType: "json",
                })
                .done(function (response) {
                    $.each(response, function(index, value) {
                        self.lookupItems.push(new lookupLineItemViewMode({ id: value.Id, displayName: value.DisplayName }));
                    });
                });
            };
            
            self.addItem = function () {
                self.lookupType = self.selectedLookup();
                self.displayName = $("#displayName").val();
                var lineData = ko.toJSON(self);

                $.ajax({
                    url: urls.ManageLookups.CreateLookup,
                    type: "POST",
                    data: lineData,
                    dataType: "json",
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                })
                .fail(function(response) {
                    toastr.error("There was an issue adding a new line item. Please try again.");
                })
                .done(function(response) {
                    self.lookupItems.removeAll();
                            
                    $.ajax({
                        url: urls.ManageLookups.LookupSubtableDetails,
                        cache: false,
                        data: { query: self.selectedLookup() },
                        dataType: "json",
                    })
                    .done(function (nestedResponse) {
                        $.each(nestedResponse, function (index, value) {
                            self.lookupItems.push(new lookupLineItemViewMode({ id: value.Id, displayName: value.DisplayName }));
                        });
                    });
                        
                    toastr.success("Success!");
                });

                $("#displayName").val('');
            };
            
            self.removeItem = function (item) {
                var strConfirm = confirm("Continue removing item?");
                if (strConfirm) {
                    item.lookupType = self.selectedLookup();
                    var lineData = ko.toJSON(item);
                    
                    $.ajax({
                        url: urls.ManageLookups.DeleteLookup,
                        type: "POST",
                        data: lineData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function(response) {
                            toastr.error("There was an issue removing the line item. Please try again");
                        })
                        .done(function (response) {
                            toastr.success("Success!");
                        });
                    self.lookupItems.remove(item);
                }
            };
        }

        function lookupLineItemViewMode(options) {
            var self = this;
            self.id = options.id;
            self.displayName = options.displayName;
            self.lookupType = options.lookupType;
        }

        var viewModel = new manageLookupsViewModel();
        ko.applyBindings(viewModel);
    });
});