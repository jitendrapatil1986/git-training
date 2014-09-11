require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls', 'toastr', 'modelData', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, ko, urls, toastr, modelData) {
        $(function () {
            $(".approve-button").click(function (e) {
                e.preventDefault();
                var serviceCallId = $(this).data("service-call-id");
                var url = urls.ServiceCall.Approve;
                executeApproval(url, serviceCallId, $(this), 'Open');
            });

            $(".deny-button").click(function (e) {
                e.preventDefault();
                var serviceCallId = $(this).data("service-call-id");
                var url = urls.ServiceCall.Deny;
                executeApproval(url, serviceCallId, $(this), 'Closed');
            });

            function executeApproval(url, serviceCallId, button, status) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { id: serviceCallId },
                    success: function (result) {
                        button.parent().html('<span>Status: <span class="label label-' + status.toLowerCase() + '-service-call">' + status + '</span></span>');
                    }
                });
            }
            
            function highlight(elemId) {
                var elem = $(elemId);
                elem.css("backgroundColor", "#ffffff"); // hack for Safari
                elem.animate({ backgroundColor: '#d4dde3' }, 500);
                setTimeout(function () {
                    $(elemId).animate({ backgroundColor: "#ffffff" }, 750);
                }, 500);
            }

            function AllLineItemsViewModel(options) {
                var self = this;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.problemCode = options.problemCode;
                self.problemCodeId = options.problemCodeId;
                self.problemDescription = options.problemDescription;
                self.completed = options.completed;
            }

            function createServiceCallLineItemViewModel() {
                var self = this;
                self.problemDescription = ko.observable("");
                self.allLineItems = ko.observableArray([]);

                self.addLineItem = function() {
                    self.serviceCallId = $("#addCallLineServiceCallId").val();
                    self.problemCode = $("#addCallLineProblemCode").find('option:selected').text();
                    self.problemCodeId = $("#addCallLineProblemCode").val();
                    self.problemDescription = $("#addCallLineProblemDescription").val();
                    
                    var newProblemDescription = $("#addCallLineProblemDescription");
                    if (newProblemDescription.val() == "") {
                        $(newProblemDescription).parent().addClass("has-error");
                        return;
                    }
                    
                    var newLineItem = new AllLineItemsViewModel({
                        serviceCallId: self.serviceCallId, problemCodeId: self.problemCodeId,
                        problemCode: self.problemCode, problemDescription: self.problemDescription
                    });
                    
                    var lineData = ko.toJSON(newLineItem);

                    $.ajax({
                        url: "/ServiceCall/AddLineItem", //TODO: Set without hard-code url.
                        type: "POST",
                        data: lineData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function (response) {
                            toastr.error("There was an issue adding the line item. Please try again!");
                        })
                        .done(function (response) {
                            self.allLineItems.unshift(new AllLineItemsViewModel({
                                serviceCallId: self.serviceCallId,
                                serviceCallLineItemId: response.newServiceLineId,
                                problemCode: self.problemCode,
                                problemCodeId: self.problemCodeId,
                                problemDescription: self.problemDescription,
                                completed: false
                            }));

                            toastr.success("Success! Item added.");
                            highlight($("#allServiceCallLineItems").first());
                            
                            $("#addCallLineProblemDescription").val('');
                            $("#addCallLineProblemCode").val('');
                            self.problemDescription = '';
                        });
                };
            }

            var viewModel = new createServiceCallLineItemViewModel();
            ko.applyBindings(viewModel);

            var persistedAllLineItemsViewModel = modelData.initialServiceLines;
                
            _(persistedAllLineItemsViewModel).each(function(item) {
                viewModel.allLineItems.push(new AllLineItemsViewModel(item));
            });
        });
    });
});