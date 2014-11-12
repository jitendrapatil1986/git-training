require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'x-editable', 'ko', 'ko.x-editable', 'urls', 'toastr', 'modelData'], function ($, xeditable, ko, koxeditable, urls, toastr, modelData) {

        function manageProblemCodeCostCodesViewModel() {
            self.cities = ko.observableArray(modelData.cities);
            self.city = ko.observable();
            self.codes = ko.observableArray([]);
            self.loading = ko.observable(false);

            self.groupedCodes = ko.computed(function () {
                var rows = [], current = [];
                rows.push(current);
                for (var i = 0; i < self.codes().length; i += 1) {
                    current.push(self.codes()[i]);
                    if (((i + 1) % 3) === 0) {
                        current = [];
                        rows.push(current);
                    }
                }
                return rows;
            }, this);

            self.city.subscribe(function (cityCode) {
                self.loading(true);
                self.codes([]);
                $.ajax({
                    url: urls.ManageProblemCodeCostCodes.ProblemCodeCostCodes,
                    cache: false,
                    data: { cityCode: cityCode },
                    dataType: "json",
                })
                .done(function (response) {
                    self.loading(false);
                    $.each(response, function(index, value) {
                        self.codes.push(new codeViewModel(value, cityCode));
                    });
                });
            });
        }

        function codeViewModel(jsonData, cityCode) {
            var self = this;
            self.cityCodeProblemCodeCostCodeId = ko.observable();
            self.cityCode = ko.observable(cityCode);
            self.problemJdeCode = ko.observable(jsonData.ProblemJdeCode);
            self.problemCode = ko.observable(jsonData.ProblemCode);
            self.costCode = ko.observable(jsonData.CostCode);

            self.costCode.subscribe(function(costCode) {
                $.ajax({
                    url: urls.ManageProblemCodeCostCodes.UpdateProblemCodeCostCode,
                    type: "POST",
                    data: ko.toJSON({ cityCode: self.cityCode(), problemJdeCode: self.problemJdeCode(), costCode: self.costCode() }),
                    dataType: "json",
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                }).fail(function() {
                    toastr.error("There was an error updating the cost code");
                }).success(function() {
                    toastr.success("Successfully updated cost code");
                });
            });
        }

        var viewModel = new manageProblemCodeCostCodesViewModel();
        ko.applyBindings(viewModel);
    });
});