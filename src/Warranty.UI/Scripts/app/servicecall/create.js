require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls'], function ($, ko, urls) {        
        function createServiceCallViewModel() {
            var self = this;
            self.lineItems = ko.observableArray([]);
            self.relatedCalls = ko.observableArray([]);
            self.problemCodeToAdd = ko.observable();
            self.problemDescriptionToAdd = ko.observable('');

            self.addLineItem = function () {
                self.problemCodeToAdd = $("#problemCode").val();
                self.problemDescriptionToAdd = $("#problemDescription").val();

                var newProblemCode = $("#problemCode");
                if (newProblemCode.val() == "") {
                    $(newProblemCode).parent().addClass("has-error");
                    return;
                }

                var newProblemDescriptionToAdd = $("#problemDescription");
                if (newProblemDescriptionToAdd.val() == "") {
                    $(newProblemDescriptionToAdd).parent().addClass("has-error");
                    return;
                }
                
                self.lineItems.push(new lineItemViewModel({ problemCode: $("#problemCode").find('option:selected').text(), problemCodeId: $("#problemCode").val(), problemDescription: $("#problemDescription").val() }));
                $("#problemDescription").val('');
                $("#problemCode").val('');
                self.problemDescriptionToAdd = '';
                self.problemCodeToAdd = '';
                $(newProblemCode).parent().removeClass("has-error");
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
                    });
            };
        }
        
        function lineItemViewModel(options) {
            var self = this;
            self.problemCode = options.problemCode;
            self.problemCodeId = options.problemCodeId;
            self.problemDescription = options.problemDescription;
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