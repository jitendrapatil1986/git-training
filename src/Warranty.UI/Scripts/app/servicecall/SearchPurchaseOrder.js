require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'moment', 'urls', 'toastr', 'modelData', 'x-editable', 'jquery.maskedinput', 'bootbox', 'app/search', 'config/job-search', 'app/serviceCall/SearchVendor', 'maxlength'], function ($, ko, koxeditable, moment, urls, toastr, modelData, xeditable, maskedInput, bootbox, search, jobSearch) {
        window.ko = ko; //manually set the global ko property.

        require(['ko.validation', 'jquery.color'], function () {
            $(function () {

                $('body').on('focus', '.max-length', function () {
                    $(this).maxlength({
                        alwaysShow: true,
                        separator: ' out of ',
                        postText: ' characters entered',
                    });
                });

                var getEndpoints = function () {
                    return [jobSearch];
                };

                search.init({
                    searchBarId: '#searchBarPO',
                    endpoints: getEndpoints(),
                    isFromSearchPurchaseOrder: true
                });
                
                function SearchpurchaseOrderViewModel() {
                    var self = this;
                    if (modelData.initialSearchPurchaseOrder.vendorName !== null && modelData.initialSearchPurchaseOrder.vendorName !== "") {
                        self.vendorName = ko.observable(modelData.initialSearchPurchaseOrder.vendorName);
                        self.vendorNumber = ko.observable(modelData.initialSearchPurchaseOrder.vendorNumber);
                        self.hasSelectedVendor = ko.observable(true);
                    }
                    else {
                        self.vendorName = ko.observable();
                        self.vendorNumber = ko.observable();
                        self.hasSelectedVendor = ko.observable(false);
                    }

                    if (modelData.initialSearchPurchaseOrder.homeOwnerName !== null && modelData.initialSearchPurchaseOrder.homeOwnerName !== "") {
                        self.homeOwnerName = ko.observable(modelData.initialSearchPurchaseOrder.homeOwnerName);
                        self.jobNumber = ko.observable(modelData.initialSearchPurchaseOrder.jobNumber);
                        self.hasSelectedJob = ko.observable(true);
                    }
                    else {
                        self.homeOwnerName = ko.observable();
                        self.jobNumber = ko.observable();
                        self.hasSelectedJob = ko.observable(false);
                    }

                    self.clearVendor = function () {
                        self.vendorName("");
                        self.vendorNumber("");
                        $("#vendor-search").val("");
                        self.hasSelectedVendor(false)
                    };

                    self.clearJob = function () {
                        self.jobNumber("");
                        self.homeOwnerName("");
                        self.hasSelectedJob(false)
                    };

                    self.assignJobSearch = function (item) {
                        self.jobNumber(item.JobNumber);
                        self.homeOwnerName(item.HomeOwnerName);
                        self.hasSelectedJob(true);
                    };

                    $(document).on('vendor-number-selected', function () {
                        self.hasSelectedVendor(true);
                        var vendorNumber = $('#vendor-search').attr('data-vendor-number');
                        var vendorName = $('#vendor-search').attr('data-vendor-name');
                        self.vendorNumber(vendorNumber);
                        self.vendorName(vendorName);
                    });
                   
                };

                ko.validation.init({
                    errorElementClass: 'has-error',
                    errorMessageClass: 'help-block',
                    decorateElement: true,
                    grouping: { deep: true }
                });
                
                ko.validation.registerExtenders();
                var viewModel = new SearchpurchaseOrderViewModel();
                viewModel.errors = ko.validation.group(viewModel);
                ko.applyBindings(viewModel);

                jobSearch.target = function (item) {
                    viewModel.assignJobSearch(item);
                };
                
            });
        });
    });
});