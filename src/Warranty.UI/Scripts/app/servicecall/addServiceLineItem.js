require(['/Scripts/app/main.js', '/Scripts/lib/jquery.color-2.1.0.min.js'], function () {
    require(['jquery', 'ko', 'urls', 'toastr'], function ($, ko, urls, toastr) {
        $(function() {
            function highlight(elemId) {
                var elem = $(elemId);
                elem.css("backgroundColor", "#ffffff"); // hack for Safari
                //elem.animate({ backgroundColor: '#ffffaa' }, 1250);
                elem.animate({ backgroundColor: '#0099ff' }, 1250);
                setTimeout(function () {
                    $(elemId).animate({ backgroundColor: "#ffffff" }, 1250);
                }, 500);
            }

            $('a[href*=#]').click(function () {
                var elemId = '#' + $(this).attr('href').split('#')[1];
                highlight(elemId);
            });


            $('#testClick').click(function () {
                //alert('click');
                var elemId = '#testHighlight';
                highlight(elemId);
            });
            




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
                            window.location.reload();
                            highlight($("#allServiceCallLineItems").first());
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