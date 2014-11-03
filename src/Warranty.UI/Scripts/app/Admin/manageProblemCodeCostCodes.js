require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls', 'modelData'], function($, ko, urls, modelData) {

        function manageProblemCodeCostCodesViewModel() {
            self.cities = ko.observableArray(modelData.cities);
            self.city = ko.observable();
            self.codes = ko.observableArray([]);

            self.city.subscribe(function(cityCode) {
                $.ajax({
                    url: urls.ManageProblemCodeCostCodes.ProblemCodeCostCodes,
                    cache: false,
                    data: { cityCode: cityCode },
                    dataType: "json",
                })
                .done(function (response) {
                    self.codes(response);
                });
            });
        }


        var viewModel = new manageProblemCodeCostCodesViewModel();
        ko.applyBindings(viewModel);
    });
});