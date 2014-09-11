require(['/Scripts/app/main.js'], function () {
    require(['modelData', 'jquery', 'ko', 'urls', 'toastr', 'bootbox'], function (modelData, $, ko, urls, toastr, bootbox) {
        function manageLookupsViewModel() {
            var self = this;
            self.availableLookups = ko.observableArray(modelData.availableLookups);
            self.selectedLookup = ko.observable({ name: '', displayName: '' });
            self.lookupItems = ko.observableArray([]);

            self.selectionChanged = function () {
                self.lookupItems.removeAll();

                $.ajax({
                    url: urls.ManageLookups.LookupSubtableDetails,
                    cache: false,
                    data: { query: this.selectedLookup().name },
                    dataType: "json",
                })
                .done(function (response) {
                    $.each(response, function (index, value) {
                        self.lookupItems.push(new lookupItemViewModel({ id: value.Id, displayName: value.DisplayName }));
                    });
                });
            };

            self.addItem = function () {
                var newDisplayName = $('#displayName');
                if (newDisplayName.val() == "") {
                    $(newDisplayName).parent().addClass("has-error");
                    return;
                }
                var lookupItem = new lookupItemViewModel({ displayName: newDisplayName.val(), lookupType: self.selectedLookup().name });
                var lineData = ko.toJSON(lookupItem);

                $.ajax({
                    url: urls.ManageLookups.CreateLookup,
                    type: "POST",
                    data: lineData,
                    dataType: "json",
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                })
                .fail(function (response) {
                    toastr.error("There was an error adding a new item. Please try again.");
                })
                .done(function (response) {
                    if (response == 0) {
                        toastr.error("Item Display Name is Required");
                    } else {
                        self.lookupItems.splice(0, 0, new lookupItemViewModel({ id: response, displayName: lookupItem.displayName, isNew: true }));
                        toastr.success("Success! Item added.");
                    }
                });

                $("#displayName").val('');
            };

            self.removeItem = function (item) {
                bootbox.confirm("Are you sure you want to delete \"" + item.displayName + "\"?", function(result) {
                    if (result) {
                        item.lookupType = self.selectedLookup().name;
                        var lineData = ko.toJSON(item);

                        $.ajax({
                            url: urls.ManageLookups.DeleteLookup,
                            type: "POST",
                            data: lineData,
                            dataType: "json",
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        })
                        .fail(function (response) {
                            toastr.error("There was an error deleting the item. Please try again");
                        })
                        .done(function (response) {
                            toastr.success("Success! Item deleted.");
                        });
                        self.lookupItems.remove(item);
                    }
                });
            };
        }

        function lookupItemViewModel(options) {
            var self = this;
            self.id = options.id;
            self.displayName = options.displayName;
            self.lookupType = options.lookupType;
            self.isNew = options.isNew;
        }

        var viewModel = new manageLookupsViewModel();
        ko.applyBindings(viewModel);
    });
});