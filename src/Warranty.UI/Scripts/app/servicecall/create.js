require(['/Scripts/app/main.js'], function () {
    
    require(['jquery', 'ko', 'urls'], function ($, ko, urls) {
        window.ko = ko;

        require(['ko.validation'], function () {
            function createServiceCallViewModel() {
                var self = this;
                self.lineItems = ko.observableArray([]).extend({
                    validation: {
                        validator: function (val) {
                            return val.length > 0;
                        },
                        message: 'Must have at least one line item.'
                    }
                });
                self.isAdding = ko.observable(false);
                self.relatedCalls = ko.observableArray([]);
                self.problemDescriptionToAdd = ko.observable().extend({ required: {onlyIf : function() { return self.isAdding(); } } });
                self.problemDetailCodeToAdd = ko.observable().extend({ required: { onlyIf: function () { return self.isAdding(); } } });
                self.problemJdeCodeToAdd = ko.observable('').extend({ required: { onlyIf: function () { return self.isAdding(); } } });
                self.problemDetailCodes = ko.observableArray([]);
                

                self.addLineItem = function () {
                    self.isAdding(true);
                    if (self.errors().length != 0) {
                        self.errors.showAllMessages();
                        return;
                    }
                    var problemCodeToAdd = $("#problemCode").find('option:selected').text();

                    self.lineItems.push(new lineItemViewModel({
                        problemCode: problemCodeToAdd,
                        problemJdeCode: self.problemJdeCodeToAdd(),
                        problemDetailCode: self.problemDetailCodeToAdd(),
                        problemDescription: self.problemDescriptionToAdd()
                    }));
                    self.isAdding(false);
                    self.problemDetailCodes([]);
                    self.problemJdeCodeToAdd('');
                    self.problemDetailCodeToAdd('');
                    self.problemDescriptionToAdd('');
                };

                self.removeLineItem = function (lineItem) {
                    self.lineItems.remove(lineItem);
                };

                $('#problemCode').change(function () {
                    self.loadRelatedCalls();
                });

                self.loadRelatedCalls = function loadRelatedCalls() {
                    self.relatedCalls.removeAll();

                    $.ajax({
                        url: urls.RelatedCall.RelatedCalls,
                        cache: false,
                        data: { jobId: $('#JobId').val(), problemCode: $('#problemCode option:selected').text() },
                        dataType: "json",
                    })
                        .done(function (response) {
                            $.each(response, function (index, value) {
                                self.relatedCalls.push(new relatedCallViewModel({ serviceCallId: value.ServiceCallId, callNumber: value.CallNumber, problemDescription: value.ProblemDescription, createdDate: value.CreatedDate }));
                            });

                            self.loadProblemDetails();
                        });
                };

                self.loadProblemDetails = function loadProblemDetails() {
                    $.ajax({
                        url: urls.ProblemDetail.ProblemDetails + '?problemJdeCode=' + $('#problemCode option:selected').val(),
                        type: "GET",
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    }).done(function (response) {
                        self.problemDetailCodes(response);
                    });
                };

                $("#createForm").submit(function () {
                    if (self.submitErrors().length != 0) {
                        self.submitErrors.showAllMessages();
                        return false;
                    }
                    return true;
                });
            }

            function lineItemViewModel(options) {
                var self = this;
                self.problemCode = options.problemCode;
                self.problemJdeCode = options.problemJdeCode;
                self.problemDescription = options.problemDescription;
                self.problemDetailCode = options.problemDetailCode;
            }

            function relatedCallViewModel(options) {
                var self = this;
                self.serviceCallId = options.serviceCallId;
                self.callNumber = options.callNumber;
                self.problemDescription = options.problemDescription;
                self.createdDate = options.createdDate;

                self.callSummaryUrl = ko.computed(function () {
                    return urls.ServiceCall.CallSummary + '/' + self.serviceCallId;
                });
            }

            ko.validation.init({
                errorElementClass: 'has-error',
                errorMessageClass: 'help-block',
                decorateElement: true
            });

            var viewModel = new createServiceCallViewModel();
            viewModel.errors = ko.validation.group([viewModel.problemDescriptionToAdd, viewModel.problemDetailCodeToAdd, viewModel.problemJdeCodeToAdd]);
            viewModel.submitErrors = ko.validation.group([viewModel.lineItems]);
            ko.applyBindings(viewModel);


            

        });

        
    });
});