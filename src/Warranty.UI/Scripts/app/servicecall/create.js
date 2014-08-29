require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko'], function ($, ko) {
        function createServiceCallViewModel() {
            var self = this;
            self.lineItems = ko.observableArray([]).extend(require: );
            self.addLineItem = function () {
                self.lineItems.push(new lineItemViewModel({ problemCode: $("#problemCode").find('option:selected').text(), problemCodeId: $("#problemCode").val(), problemDescription: $("#problemDescription").val() }));
                $("#problemDescription").val('');
            };

            self.removeLineItem = function (lineItem) {
                self.lineItems.remove(lineItem);
            };

            self.headerCommentLineItems = ko.observableArray([]);
            self.addHeaderCommentLineItem = function () {
                self.headerCommentLineItems.push(new headerCommentLineItemViewModel({ comment: $("#headerComment").val() }));
                $("#headerComment").val('');
            };

            self.removeHeaderCommentLineItem = function (headerCommentLineItem) {
                self.headerCommentLineItems.remove(headerCommentLineItem);
            };
        }

        function lineItemViewModel(options) {
            var self = this;
            self.problemCode = options.problemCode;
            self.problemCodeId = options.problemCodeId;
            self.problemDescription = options.problemDescription;
        }

        function headerCommentLineItemViewModel(options) {
            var self = this;
            self.comment = options.comment;
            //self.createdBy = options.createdBy;
            //self.createdDate = options.createdDate;
        }

        var viewModel = new createServiceCallViewModel();
        ko.applyBindings(viewModel);

    });
});