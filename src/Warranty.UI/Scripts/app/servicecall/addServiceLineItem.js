require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls', 'toastr'], function ($, ko, urls, toastr) {
        $(function() {
            function createServiceCallLineItemViewModel() {
                var self = this;
                self.problemDescription = ko.observable("");

                self.addLineItem = function() {
                    self.serviceCallId = $("#addCallLineServiceCallId").val();
                    self.problemCode = $("#addCallLineProblemCode").find('option:selected').text();
                    self.problemCodeId = $("#addCallLineProblemCode").val();
                    self.problemDescription = $("#addCallLineProblemDescription").val();
                    var lineData = ko.toJSON(self);

                    $.ajax({
                        url: "/ServiceCall/AddLineItem", //TODO: Set without hard-code url.
                        type: "POST",
                        data: lineData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function(response) {
                            toastr.error("There was an issue adding the line item. Please try again!");
                        })
                        .done(function(response) {
                            toastr.success("Success! Item added.");
                            window.location.reload();
                        });

                    $("#addCallLineProblemDescription").val('');
                    $("#addCallLineProblemCode").val('');
                };
            }

            var viewModel = new createServiceCallLineItemViewModel();
            ko.applyBindings(viewModel);
        });
    });
});