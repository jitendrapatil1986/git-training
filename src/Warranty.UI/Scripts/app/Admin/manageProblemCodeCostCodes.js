require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls', 'modelData'], function($, ko, urls, modelData) {

        function manageProblemCodeCostCodesViewModel() {
            self.cities = ko.observableArray(modelData.cities);
            self.city = ko.observable();
            self.codes = ko.observableArray([]);
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

            self.city.subscribe(function(cityCode) {
                $.ajax({
                    url: urls.ManageProblemCodeCostCodes.ProblemCodeCostCodes,
                    cache: false,
                    data: { cityCode: cityCode },
                    dataType: "json",
                })
                .done(function (response) {
                    self.codes([]);
                    $.each(response, function(index, value) {
                        self.codes.push(new codeViewModel(value));
                    });
                });
            });
        }

        function codeViewModel(jsonData) {
            var self = this;
            self.problemJdeCode = ko.observable(jsonData.ProblemJdeCode);
            self.problemCode = ko.observable(jsonData.ProblemCode);
            self.costCode = ko.observable(jsonData.CostCode);
        }


        var viewModel = new manageProblemCodeCostCodesViewModel();
        ko.applyBindings(viewModel);
    });
});