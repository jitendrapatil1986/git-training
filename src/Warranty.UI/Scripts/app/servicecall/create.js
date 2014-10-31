require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls'], function ($, ko, urls) {        
        function createServiceCallViewModel() {
            var self = this;
            self.lineItems = ko.observableArray([]);
            self.relatedCalls = ko.observableArray([]);
            self.problemDescriptionToAdd = ko.observable('');
            self.problemDetailCodes = ko.observableArray([]);

            self.addLineItem = function () {
                var problemJdeCodeToAdd = $("#problemCode").val();
                var problemCodeToAdd = $("#problemCode").find('option:selected').text();
                var problemDescriptionToAdd = $("#problemDescription").val();
                var problemDetailToAdd = $("#problemDetail").val();

                var newProblemCode = $("#problemCode");
                if (newProblemCode.val() == "") {
                    $(newProblemCode).parent().addClass("has-error");
                    return;
                }

                var newProblemDetail = $("#problemDetail");
                if (newProblemDetail.val() == "") {
                    $(newProblemDetail).parent().addClass("has-error");
                    return;
                }

                var newProblemDescriptionToAdd = $("#problemDescription");
                if (newProblemDescriptionToAdd.val() == "") {
                    $(newProblemDescriptionToAdd).parent().addClass("has-error");
                    return;
                }
                
                self.lineItems.push(new lineItemViewModel({ problemCode: problemCodeToAdd, problemJdeCode: problemJdeCodeToAdd, problemDetailCode: problemDetailToAdd, problemDescription: problemDescriptionToAdd }));
                $("#problemDescription").val('');
                $("#problemCode").val('');
                $("#problemDetail").val('');
                $(newProblemCode).parent().removeClass("has-error");
                $(newProblemDetail).parent().removeClass("has-error");
                $(newProblemDescriptionToAdd).parent().removeClass("has-error");
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
                    .done(function(response) {
                        $.each(response, function(index, value) {
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

        var viewModel = new createServiceCallViewModel();
        ko.applyBindings(viewModel);

        viewModel.loadRelatedCalls();
    });
});