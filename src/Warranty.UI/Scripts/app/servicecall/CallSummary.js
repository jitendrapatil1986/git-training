require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls', 'toastr', 'modelData', 'dropdownData', 'enumerations/ServiceCallStatus','/Scripts/lib/jquery.color-2.1.0.min.js','x-editable'], function ($, ko, urls, toastr, modelData, dropdownData, serviceCallStatusData,xeditable) {
        $(function () {
            $.fn.editable.defaults.mode = 'inline';
            $("#Employee_List").editable({
                type: 'select',
            });

            $(".datepicker-input").datepicker();

            $('.btn-action-with-popup').click(function (e) {
                $(this).addClass("active");
                $('.popup-action-with-message').hide();
                var right = ($(window).width() - ($(this).offset().left + $(this).outerWidth()));
                var actionwithPopup = $(this).data('action-with-popup');
                $("#" + actionwithPopup).css({
                    'position': 'absolute',
                    'right': right,
                    'top': $(this).offset().top + $(this).height() + 15
                }).show();
            });
            
            $('.btn-cancel-popup').click(function (e) {
                var popupWindow = $(this).parent();
                var parentButton = $("#btn_" + popupWindow.attr('id'));
                parentButton.removeClass("active");
                popupWindow.hide();
            });

            $('.btn-execute-action').click(function (e) {
                var popupWindow = $(this).parent();
                var actionUrl = $(this).data('action-url');
                var textArea = $(this).prev('textarea');
                var message = textArea.val();
                textArea.val('');
                var serviceCallId = $(this).data('service-call-id');
                var parentButton = $("#btn_" + popupWindow.attr('id'));
                $.ajax({
                    type: "POST",
                    url: actionUrl,
                    data: { id: serviceCallId, message: message },

                    success: function (data) {
                        changeButtonText(parentButton);
                        parentButton.removeClass("active");
                        popupWindow.hide();
                    }
                });
                
                function changeButtonText(button) {
                    var currentText = button.text();
                    var nextText = button.data('next-text');
                    button.data('next-text', currentText);
                    button.text(nextText);
                }
            });

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

            function clearNoteFields() {
                $("#addCallNoteDescription").val('');
                $("#addCallNoteLineReferenceDropDown").val('');
                self.selectedLineToAttachToNote('');
                self.noteDescriptionToAdd('');
            }
            
            function AllLineItemsViewModel(options) {
                var self = this;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.completed = options.completed;

                //track line item properties.
                self.problemCodeId = ko.observable(options.problemCodeId);
                self.problemCode = ko.observable(options.problemCode);
                self.problemDescription = ko.observable(options.problemDescription);
                self.currentProblemCode = ko.observable();
                self.currentProblemDescription = ko.observable();
                
                //track editing problem code, desc, and line altogether.
                self.problemCodeEditing = ko.observable();
                self.problemDescriptionEditing = ko.observable("");
                self.lineEditing = ko.observable("");
                
                //edit line item.
                self.editLine = function() {
                    this.problemCodeEditing(true);
                    this.problemDescriptionEditing(true);
                    this.lineEditing(true);
                    this.currentProblemCode(this.problemCode());
                    this.currentProblemDescription(this.problemDescription());
                };
                
                //save line item changes.
                self.saveLineItemChanges = function () {
                    updateServiceCallLineItem(this);
                };
                
                //cancel line item changes.
                self.cancelLineItemChanges = function () {
                    this.problemCodeEditing(false);
                    this.problemDescriptionEditing(false);
                    this.lineEditing(false);
                    this.problemCode(this.currentProblemCode());
                    this.problemDescription(this.currentProblemDescription());
                };
                
                //complete line item.
                self.completeLine = function () {
                    this.lineEditing(false);
                    completeServiceCallLineItem(this);
                };
                
                self.lineNumber = ko.observable(options.lineNumber);
                
                self.lineNumberWithProblemCode = ko.computed(function() {
                    return self.lineNumber() + " - " + self.problemCode();
                });

                self.serviceCallLineItemStatus = ko.observable(options.serviceCallLineItemStatus);
                self.serviceCallLineItemStatusDisplayName = ko.observable(options.serviceCallLineItemStatus ? options.serviceCallLineItemStatus.displayName : '');
                
                self.lineItemStatusCSS = ko.computed(function () {
                    return self.serviceCallLineItemStatusDisplayName() ? 'label label-' + self.serviceCallLineItemStatusDisplayName().toLowerCase() + '-service-line-item' : '';
                });
            }
            
            function CallNotesViewModel(options) {
                var self = this;
                self.serviceCallNoteId = options.serviceCallNoteId;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.note = options.note;
                self.createdBy = options.createdBy;
                self.createdDate = options.createdDate;
                self.serviceCallCommentTypeId = options.serviceCallCommentTypeId;
            }

            function updateServiceCallLineItem(line) {
                var updateProblemCode = $("#updateCallLineProblemCode");
                if (updateProblemCode.val() == "") {
                    $(updateProblemCode).parent().addClass("has-error");
                    return;
                }

                var updateProblemDescription = $("#updateCallLineProblemDescription");
                if (updateProblemDescription.val() == "") {
                    $(updateProblemDescription).parent().addClass("has-error");
                    return;
                }

                var lineData = ko.toJSON(line);

                $.ajax({
                    url: urls.ManageServiceCall.EditLineItem,
                    type: "POST",
                    data: lineData,
                    dataType: "json",
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                })
                    .fail(function (response) {
                        toastr.error("There was an issue updating the line item. Please try again!");
                    })
                    .done(function (response) {
                        toastr.success("Success! Item updated.");
                        self.problemCode = line.problemCode;
                        
                        //change to non-edit mode once success has occurred.
                        line.problemCodeEditing(false);
                        line.problemDescriptionEditing(false);
                        line.lineEditing(false);
                    });
                
                $(updateProblemCode).parent().removeClass("has-error");
                $(updateProblemDescription).parent().removeClass("has-error");
            }
            
            function completeServiceCallLineItem(line) {
                var lineData = ko.toJSON(line);

                $.ajax({
                    url: urls.ManageServiceCall.CompleteLineItem,
                    type: "POST",
                    data: lineData,
                    dataType: "json",
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                })
                    .fail(function (response) {
                        toastr.error("There was an issue completing the line item. Please try again!");
                    })
                    .done(function (response) {
                        toastr.success("Success! Item completed.");
                        self.serviceCallLineItemStatus = response.ServiceCallLineItemStatus;
                    });
            }

            function serviceCallSummaryItemViewModel() {
                var self = this;
                
                self.allLineItems = ko.observableArray([]);
                self.theLookups = dropdownData.availableLookups;  //dropdown list does not need to be observable. Only the actual elements w/i the array do.
                self.problemDescriptionToAdd = ko.observable('');
                self.problemCodeToAdd = ko.observable();
                
                self.allCallNotes = ko.observableArray([]);
                self.selectedLineToAttachToNote = ko.observable();
                self.selectedLineToFilterNotes = ko.observable();
                self.noteDescriptionToAdd = ko.observable('');
                self.filteredCallNotes = ko.computed(function() {
                    var lineIdToFilterNotes = self.selectedLineToFilterNotes();
                    if (!lineIdToFilterNotes || lineIdToFilterNotes == "") {
                        return self.allCallNotes();
                    } else {
                        return ko.utils.arrayFilter(self.allCallNotes(), function (i) {
                            return i.serviceCallLineItemId == lineIdToFilterNotes;
                        });
                    }
                });

                self.areAllLineItemsClosed = function () {
                    var anyNonCompletedLineItem = ko.utils.arrayFirst(self.allLineItems(), function (i) {
                        return (i.serviceCallLineItemStatus().displayName.toLowerCase() != serviceCallStatusData.Closed.DisplayName.toLowerCase());
                    });

                    if (anyNonCompletedLineItem)
                        return false;
                    else
                        return true;
                };

                self.addLineItem = function() {
                    self.serviceCallId = $("#callSummaryServiceCallId").val();
                    self.problemCode = $("#addCallLineProblemCode").find('option:selected').text();
                    self.problemCodeId = $("#addCallLineProblemCode").val();
                    self.problemDescription = $("#addCallLineProblemDescription").val();
                    
                    var newProblemCode = $("#addCallLineProblemCode");
                    if (newProblemCode.val() == "") {
                        $(newProblemCode).parent().addClass("has-error");
                        return;
                    }
                    
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
                        url: urls.ManageServiceCall.AddLineItem,
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
                                serviceCallLineItemId: response.ServiceCallLineItemId,
                                lineNumber: response.LineNumber,
                                problemCode: self.problemCode,
                                problemCodeId: self.problemCodeId,
                                problemDescription: self.problemDescription,
                                serviceCallLineItemStatus: response.ServiceCallLineItemStatus,
                                completed: false
                            }));

                            toastr.success("Success! Item added.");
                            highlight($("#allServiceCallLineItems").first());
                            
                            $("#addCallLineProblemDescription").val('');
                            $("#addCallLineProblemCode").val('');
                            self.problemDescription = '';
                        });
                    
                    $(newProblemCode).parent().removeClass("has-error");
                    $(newProblemDescription).parent().removeClass("has-error");
                };

                self.addCallNote = function () {
                    self.serviceCallId = $("#callSummaryServiceCallId").val();
                    self.serviceCallLineItemId = $("#addCallNoteLineReferenceDropDown").find('option:selected').val();
                    self.note = $("#addCallNoteDescription").val();

                    var newNoteDescription = $("#addCallNoteDescription");
                    if (newNoteDescription.val() == "") {
                        $(newNoteDescription).parent().addClass("has-error");
                        return;
                    }
                    
                    var newCallNote = new CallNotesViewModel({
                        serviceCallId: self.serviceCallId,
                        serviceCallLineItemId: self.serviceCallLineItemId,
                        note: self.note,
                        serviceCallCommentTypeId: self.serviceCallCommentTypeId
                    });
                    
                    var lineNoteData = ko.toJSON(newCallNote);

                    $.ajax({
                        url: urls.ManageServiceCall.AddNote,
                        type: "POST",
                        data: lineNoteData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function (response) {
                            toastr.error("There was an issue adding the call note. Please try again!");
                        })
                        .done(function (response) {
                            self.allCallNotes.unshift(new CallNotesViewModel({
                                serviceCallNoteId: response.ServiceCallNoteId,
                                serviceCallId: self.serviceCallId,
                                serviceCallLineItemId: self.serviceCallLineItemId,
                                note: self.note,
                                serviceCallCommentTypeId: self.serviceCallCommentTypeId,
                                createdBy: response.CreatedBy,
                                createdDate: response.CreatedDate
                            }));

                            var currentLineItem = ko.utils.arrayFirst(self.allLineItems(), function(i) {
                                return i.serviceCallLineItemId == response.ServiceCallLineItemId;
                            });

                            if (currentLineItem) {
                                currentLineItem.serviceCallLineItemStatusDisplayName(response.ServiceCallLineItemStatus.DisplayName);
                            }

                            toastr.success("Success! Note added.");
                            highlight($("#allServiceCallNotes").first());
                            clearNoteFields();
                        });
                };

                self.cancelCallNote = function () {
                    clearNoteFields();
                };
                
                self.resetCallNoteFilter = function () {
                    $("#filterCallNoteLineReferenceDropDown").val('');
                    self.selectedLineToFilterNotes('');
                };

                self.callSummaryServiceCallStatus = ko.observable($("#callSummaryServiceCallStatus").html());

                self.callSummaryStatusClosed = ko.computed(function () {
                    if (self.callSummaryServiceCallStatus().toLowerCase() == serviceCallStatusData.Closed.DisplayName.toLowerCase())
                        return true;
                    else
                        return false;
                });
                
                self.callSummaryStatusSigned = ko.computed(function () {
                    if (self.callSummaryServiceCallStatus().toLowerCase() == serviceCallStatusData.HomeownerSigned.DisplayName.toLowerCase())
                        return true;
                    else
                        return false;
                });

                self.homeownerVerificationSignature = ko.observable('');
                self.homeownerVerificationSignatureDate = ko.observable();
                
                self.verifiedHomeownerSignature = ko.observable($("#verifiedHomeownerSignature").val());
                self.verifiedHomeownerSignatureDate = ko.observable($("#verifiedHomeownerSignatureDate").val());
                
                self.verifiedSignature = ko.computed(function () {
                    var result = 'Verification Signed by ' + self.verifiedHomeownerSignature();
                    
                    if (self.verifiedHomeownerSignatureDate()) {
                        result += ' on ' + moment(self.verifiedHomeownerSignatureDate()).format('L');
                    }

                    return result;
                });
                
                self.saveVerifiedHomeownerSignature = function () {
                    self.serviceCallId = $("#callSummaryServiceCallId").val();
                    self.homeownerVerificationSignature = $("#homeownerVerificationSignature").val();
                    self.homeownerVerificationSignatureDate = $("#homeownerVerificationSignatureDate").val();

                    var newhomeownerVerificationSignature = $("#homeownerVerificationSignature");
                    if (newhomeownerVerificationSignature.val() == "") {
                        $(newhomeownerVerificationSignature).parent().addClass("has-error");
                        return;
                    }

                    var newhomeownerVerificationSignatureDate = $("#homeownerVerificationSignatureDate");
                    if (newhomeownerVerificationSignatureDate.val() == "") {
                        $(newhomeownerVerificationSignatureDate).parent().addClass("has-error");
                        return;
                    }

                    var verifySignatureData = ko.toJSON({
                        serviceCallId: self.serviceCallId, homeownerVerificationSignature: self.homeownerVerificationSignature,
                        homeownerVerificationSignatureDate: self.homeownerVerificationSignatureDate
                    });

                    $.ajax({
                        url: urls.ManageServiceCall.VerifyHomeownerSignatureServiceCall,
                        type: "POST",
                        data: verifySignatureData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function (response) {
                            alert(JSON.stringify(response));
                            toastr.error("There was an issue saving the homeowner verification signature. Please try again!");
                        })
                        .done(function (response) {
                            self.callSummaryServiceCallStatus(response.ServiceCallStatus.DisplayName);
                            self.verifiedHomeownerSignature(response.HomeownerVerificationSignature);
                            self.verifiedHomeownerSignatureDate(response.HomeownerVerificationSignatureDate);
                            toastr.success("Success! Data saved.");

                            $("#homeownerVerificationSignature").val('');
                            $("#homeownerVerificationSignatureDate").val('');
                            self.homeownerVerificationSignature = '';
                            self.homeownerVerificationSignatureDate = '';
                        });

                    $(homeownerVerificationSignature).parent().removeClass("has-error");
                    $(homeownerVerificationSignatureDate).parent().removeClass("has-error");
                };
            }

            var viewModel = new serviceCallSummaryItemViewModel();
            ko.applyBindings(viewModel);

            var persistedAllLineItemsViewModel = modelData.initialServiceLines;
                
            _(persistedAllLineItemsViewModel).each(function (item) {
                viewModel.allLineItems.push(new AllLineItemsViewModel(item));
            });

            var persistedAllCallNotesViewModel = modelData.initialServiceNotes;

            _(persistedAllCallNotesViewModel).each(function(note) {
                viewModel.allCallNotes.push(new CallNotesViewModel(note));
            });
        });
    });
});