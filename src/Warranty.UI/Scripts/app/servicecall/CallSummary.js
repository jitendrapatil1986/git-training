require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'bootbox', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/PhoneNumberType', 'enumeration/ActivityType', 'enumeration/HomeownerContactType', 'enumeration/HomeownerVerificationType', 'jquery.maskedinput', 'enumeration/ServiceCallStatus', 'enumeration/ServiceCallLineItemStatus', 'app/additionalContacts', 'app/formUploader', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, ko, koxeditable, bootbox, urls, toastr, modelData, dropdownData, xeditable, phoneNumberTypeEnum, activityTypeEnum, homeownerContactTypeEnum, homeownerVerificationTypeEnum, maskedInput, serviceCallStatusData, serviceCallLineItemStatusData, additionalContacts) {
        window.ko = ko; //manually set the global ko property.
        require(['ko.validation'], function () {

         $(function () {

            $.fn.editable.defaults.mode = 'inline';
            $.fn.editable.defaults.emptytext = 'Add';
            $.fn.editableform.buttons =
                '<button type="submit" class="btn btn-primary editable-submit btn-sm"><i class="glyphicon glyphicon-ok"></i></button>';

            $("#Employee_List").editable({
                type: 'select',
            });

            $("#Home_Phone").editable({
                params: { phoneNumberTypeValue: phoneNumberTypeEnum.Home.Value }
            });

            $("#Mobile_Phone").editable({
                params: { phoneNumberTypeValue: phoneNumberTypeEnum.Mobile.Value }
            });

            $("#Email").editable({
            });

            $(".attached-file-display-name").editable();

            $(".phone-number-with-extension").on('shown', function () {
                $(this).data('editable').input.$input.mask("(999) 999-9999? x99999", { placeholder: " " });
            });

            $("#homeownerVerificationSignatureDate").datepicker({
                format: 'm/d/yyyy'
            });

            $("#homeownerVerificationSignatureDate").on('changeDate', function (e) {
                viewModel.homeownerVerificationSignatureDate(moment(e.date).format("L"));
            });

            $('.btn-action-with-popup').click(function (e) {
                $('.btn-action-with-popup').removeClass('active');
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
                var serviceCallId = $(this).data('service-call-id');
                var parentButton = $("#btn_" + popupWindow.attr('id'));
                $.ajax({
                    type: "POST",
                    url: actionUrl,
                    data: { id: serviceCallId, message: message },
                    success: function (result) {
                        updateUI(result.actionName, result.actionMessage);
                        changeButtonText(parentButton);
                        parentButton.removeClass("active");
                        textArea.val('');
                        popupWindow.hide();
                    }
                });
            });

            $('#btn_execute_reopen').click(function (e) {
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
                        reOpenServiceCall();
                        parentButton.removeClass("active");
                        popupWindow.hide();
                    }
                });
            });

            $('#btn_complete').click(function (e) {
                var serviceCallId = $(this).data('service-call-id');
                var url = urls.ServiceCall.Complete;
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { id: serviceCallId },
                    success: function (data) {
                        completeServiceCall();
                    }
                });
            });

            function reOpenServiceCall() {
                viewModel.callSummaryServiceCallStatus(serviceCallStatusData.Open.DisplayName);
                toastr.success("Success! Service Call has been succesfully reopened.");
            }

            function completeServiceCall() {
                viewModel.callSummaryServiceCallStatus(serviceCallStatusData.Complete.DisplayName);
                viewModel.canBeReopened(true);
                toastr.success("Success! Service Call has been succesfully completed.");
            }
            
            function updateUI(actionName, actionMessage) {

                if (actionName == activityTypeEnum.Escalation.DisplayName) {
                    var isEscalated = viewModel.isEscalated();
                    viewModel.isEscalated(!isEscalated);
                    if (!viewModel.isEscalated) {
                        viewModel.escalationReason('');
                        viewModel.escalationDate('');
                    } else {
                        viewModel.escalationReason(actionMessage);
                        viewModel.escalationDate(moment());
                    }
                }
                else if (actionName == activityTypeEnum.SpecialProject.DisplayName) {
                    var isSpecialProject = viewModel.isSpecialProject();
                    viewModel.isSpecialProject(!isSpecialProject);
                    if (!viewModel.isSpecialProject) {
                        viewModel.specialProjectReason('');
                        viewModel.specialProjectDate('');
                    } else {
                        viewModel.specialProjectReason(actionMessage);
                        viewModel.specialProjectDate(moment());
                    }
                }
            }

            function changeButtonText(button) {
                var currentText = button.text();
                var nextText = button.data('next-text');
                button.data('next-text', currentText);
                button.text(nextText);
            }

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
                executeApproval(url, serviceCallId, $(this), 'Completed');
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
                viewModel.noteDescriptionToAdd('');
                viewModel.noteDescriptionToAdd.isModified(false);
            }

            function AllLineItemsViewModel(options) {
                var self = this;
                self.lineEditing = ko.observable(false);
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.completed = options.completed;
                self.numberOfAttachments = options.numberOfAttachments;
                self.numberOfNotes = options.numberOfNotes;

                //track line item properties.
                self.problemJdeCode = ko.observable(options.problemJdeCode).extend({
                    required: {
                        onlyIf: function () { return self.lineEditing() === true; }
                    }
                });
                
                self.problemCode = ko.observable(options.problemCode);
                self.problemDescription = ko.observable(options.problemDescription).extend({
                    required: {
                        onlyIf: function () { return self.lineEditing() === true; }
                    }
                });

                self.currentProblemCode = ko.observable();
                self.currentProblemJdeCode = ko.observable();
                self.currentProblemDescription = ko.observable();

                //track editing problem code, desc, and line altogether.
                self.problemCodeEditing = ko.observable();
                self.problemDescriptionEditing = ko.observable("");
                

                //edit line item.
                self.editLine = function () {
                    this.problemCodeEditing(true);
                    this.problemDescriptionEditing(true);
                    this.lineEditing(true);
                    this.currentProblemCode(this.problemCode());
                    this.currentProblemJdeCode(this.problemJdeCode());
                    this.currentProblemDescription(this.problemDescription());
                };

                //save line item changes.
                self.saveLineItemChanges = function () {
                    var errors = ko.validation.group(self);

                    if (errors().length != 0) {
                        viewModel.errors.showAllMessages(false);
                        self.errors.showAllMessages();
                        return;
                    }
                    
                    updateServiceCallLineItem(this);
                };

                //cancel line item changes.
                self.cancelLineItemChanges = function () {
                    this.problemCodeEditing(false);
                    this.problemDescriptionEditing(false);
                    this.lineEditing(false);
                    this.problemCode(this.currentProblemCode());
                    this.problemJdeCode(this.currentProblemJdeCode());
                    this.problemDescription(this.currentProblemDescription());
                };

                //reopen line item.
                self.reopenLine = function () {
                    this.lineEditing(false);
                    reopenServiceCallLineItem(this);
                };

                self.lineNumber = ko.observable(options.lineNumber);

                self.lineNumberWithProblemCode = ko.computed(function () {
                    return self.lineNumber() + " - " + self.problemCode();
                });

                self.serviceCallLineItemStatus = ko.observable(options.serviceCallLineItemStatus);

                self.serviceCallLineItemStatusDisplayName = ko.observable('');
                if (options.serviceCallLineItemStatus) {
                    if (options.serviceCallLineItemStatus.displayName)
                        self.serviceCallLineItemStatusDisplayName(options.serviceCallLineItemStatus.displayName);  //TODO: displayName works for model passed into js file via toJSON().
                    if (options.serviceCallLineItemStatus.DisplayName)
                        self.serviceCallLineItemStatusDisplayName(options.serviceCallLineItemStatus.DisplayName);  //TODO: DisplayName works for model passed from ajax call. Need to keep both similar.
                }

                self.lineItemStatusCSS = ko.computed(function () {
                    return self.serviceCallLineItemStatusDisplayName() ? 'label label-' + self.serviceCallLineItemStatusDisplayName().toLowerCase() + '-service-line-item' : '';
                });

                self.isLineItemCompleted = function () {
                    if (!self.serviceCallLineItemStatusDisplayName())
                        return false;

                    return self.serviceCallLineItemStatusDisplayName().toLowerCase() == serviceCallLineItemStatusData.Complete.DisplayName.toLowerCase() ? true : false;
                };

                self.jumpToServiceCallLineDetailPage = function () {
                    //only jump to detail pg when not editing the line. so btns within elements where this fn is called should have clickBubble: false to ensure the btn click
                    //events do not bubble up and hit the element calling this fn.
                    if (this.lineEditing() == false) {
                        window.location.href = urls.ServiceCall.LineItemDetail + '/' + self.serviceCallLineItemId;
                    }
                };
            }

            function CallNotesViewModel(options) {
                var self = this;
                self.serviceCallNoteId = options.serviceCallNoteId;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = ko.observable(options.serviceCallLineItemId);
                self.note = options.note;
                self.createdBy = options.createdBy;
                self.createdDate = options.createdDate;
                self.serviceCallCommentTypeId = options.serviceCallCommentTypeId;

                self.noteLineNumberWithProblemCode = ko.computed(function () {
                    var lineIdToFilterNotes = options.serviceCallLineItemId;
                    if (!lineIdToFilterNotes || lineIdToFilterNotes == "") {
                        return "";
                    } else {
                        ko.utils.arrayForEach(viewModel.allLineItems(), function (i) {
                            if (i.serviceCallLineItemId == lineIdToFilterNotes) {
                                return i.lineNumber() + " - " + i.problemCode();
                            }
                            return "";
                        });
                        return "";
                    }
                });
            }

            function CallAttachmentsViewModel(options) {
                var self = this;
                self.serviceCallAttachmentId = options.serviceCallAttachmentId;
                self.serviceCallLineItemId = ko.observable(options.serviceCallLineItemId);
                self.serviceCallId = options.serviceCallId;
                self.displayName = ko.observable(options.displayName);
                self.createdBy = options.createdBy;
                self.createdDate = options.createdDate;
            }
             
            function formHasErrors(theModel) {
                var errors = ko.validation.group(theModel);
                
                if (errors().length != 0) {
                    viewModel.errors.showAllMessages(false);
                    errors.showAllMessages();
                    return true;
                }

                return false;
            }

            function updateServiceCallLineItem(line) {
                var selectedProblemCodeLine = ko.utils.arrayFirst(viewModel.theLookups, function (item) {
                    return item.problemId == line.problemJdeCode();
                });

                line.problemCode(selectedProblemCodeLine.problemCode);

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

                        //change to non-edit mode once success has occurred.
                        line.problemCodeEditing(false);
                        line.problemDescriptionEditing(false);
                        line.lineEditing(false);
                    });
            }

            function reopenServiceCallLineItem(line) {
                var lineData = ko.toJSON(line);

                $.ajax({
                    url: urls.ManageServiceCall.ReopenLineItem,
                    type: "POST",
                    data: lineData,
                    dataType: "json",
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                })
                    .fail(function (response) {
                        toastr.error("There was an issue reopening the line item. Please try again!");
                    })
                    .done(function (response) {
                        toastr.success("Success! Item reopened.");
                        line.serviceCallLineItemStatusDisplayName(response.ServiceCallLineItemStatus.DisplayName);
                        viewModel.callSummaryServiceCallStatus(response.ServiceCallStatus.DisplayName);
                    });
            }

            function serviceCallSummaryItemViewModel() {
                var self = this;

                self.allLineItems = ko.observableArray([]);
                self.theLookups = dropdownData.availableLookups;  //dropdown list does not need to be observable. Only the actual elements w/i the array do.
                self.problemDescriptionToAdd = ko.observable('').extend({ required: true });
                self.problemJdeCodeToAdd = ko.observable().extend({ required: true });
                
                self.canBeReopened = ko.observable(modelData.canBeReopened);
                self.isSpecialProject = ko.observable(modelData.isSpecialProject);
                self.specialProjectReason = ko.observable(modelData.specialProjectReason);
                self.specialProjectDate = ko.observable(modelData.specialProjectDate);
                self.isEscalated = ko.observable(modelData.isEscalated);
                self.escalationReason = ko.observable(modelData.escalationReason);
                self.escalationDate = ko.observable(modelData.escalationDate);
                self.allCallNotes = ko.observableArray([]);
                self.allAttachments = ko.observableArray([]);

                self.noteDescriptionToAdd = ko.observable('').extend({ required: true});

                self.problemDetailCodes = ko.observableArray([]);
                self.relatedCalls = ko.observableArray([]);


                self.areAllLineItemsCompleted = ko.computed(function () {
                    var anyNonCompletedLineItem = ko.utils.arrayFirst(self.allLineItems(), function (i) {
                        return (i.serviceCallLineItemStatusDisplayName().toLowerCase() != serviceCallStatusData.Complete.DisplayName.toLowerCase());
                    });

                    if (anyNonCompletedLineItem)
                        return false;
                    else
                        return true;
                }).extend({ notify: 'always' });


                self.problemJdeCodeToAdd.subscribe(function (newValue) {
                    if (newValue != "") {
                        self.loadRelatedCalls();
                    } else {
                        self.problemDetailCodes([]);
                    }
                });
                
                self.loadRelatedCalls = function loadRelatedCalls() {
                    self.relatedCalls.removeAll();
                    var problemCode = $("#addCallLineProblemCode").find('option:selected').val();
                    $.ajax({
                        url: urls.RelatedCall.RelatedCalls,
                        cache: false,
                        data: { serviceCallId: $("#callSummaryServiceCallId").val(), problemCode: problemCode },
                        dataType: "json",
                    })
                        .done(function (response) {
                            $.each(response, function (index, value) {
                                self.relatedCalls.push(new relatedCallViewModel({ serviceCallId: value.ServiceCallId, callNumber: value.CallNumber, problemDescription: value.ProblemDescription, createdDate: value.CreatedDate }));
                            });

                            var problemJdeCode = problemCode;
                            getproblemDetailCodes(problemJdeCode, self.problemDetailCodes);
                        });
                };
                
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

                function getproblemDetailCodes(problemJdeCode, problemDetailCodes) {
                    $.ajax({
                        url: urls.ProblemDetail.ProblemDetails + '?problemJdeCode=' + problemJdeCode,
                        type: "GET",
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    }).done(function (response) {
                        problemDetailCodes(response);
                    });
                }

                
                self.removeAttachment = function (e) {
                    bootbox.confirm(modelData.attachmentRemovalMessage, function(result) {
                        if (result) {
                            var item = $('.boxclose[data-attachment-id="' + e.serviceCallAttachmentId + '"]');
                            var actionUrl = item.data('url');
                            var attachmentId = e.serviceCallAttachmentId;
                            $.ajax({
                                type: "POST",
                                url: actionUrl,
                                data: { id: attachmentId },
                                success: function (data) {
                                    self.allAttachments.remove(e);
                                    toastr.success("Success! Attachment deleted.");
                                }
                            });
                        }
                    });
                };

                self.addLineItem = function () {
                    if (formHasErrors([self.problemJdeCodeToAdd, self.problemDescriptionToAdd]))
                        return;
                    
                    self.serviceCallId = $("#callSummaryServiceCallId").val();
                    self.problemCode = $("#addCallLineProblemCode").find('option:selected').text();
                    self.problemJdeCode = $("#addCallLineProblemCode").val();
                    self.problemDescription = $("#addCallLineProblemDescription").val();

                    var newLineItem = new AllLineItemsViewModel({
                        serviceCallId: self.serviceCallId, problemJdeCode: self.problemJdeCode,
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
                                problemJdeCode: self.problemJdeCode,
                                problemDescription: self.problemDescription,
                                serviceCallLineItemStatus: response.ServiceCallLineItemStatus,
                                completed: false
                            }));

                            toastr.success("Success! Item added.");
                            highlight($("#allServiceCallLineItems").first());

                            $("#addCallLineProblemDescription").val('');
                            $("#addCallLineProblemCode").val('');
                            self.problemDescription = '';
                            self.problemJdeCodeToAdd('');
                            self.problemJdeCodeToAdd.isModified(false);
                            self.problemDescriptionToAdd('');
                            self.problemDescriptionToAdd.isModified(false);
                        });
                };

                self.addCallNote = function () {
                    if (formHasErrors([self.noteDescriptionToAdd])) {
                        return;
                    }

                    self.serviceCallId = $("#callSummaryServiceCallId").val();
                    self.note = $("#addCallNoteDescription").val();

                    var newCallNote = new CallNotesViewModel({
                        serviceCallId: self.serviceCallId,
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
                            toastr.error("There was a problem adding the call note. Please try again.");
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

                            var currentLineItem = ko.utils.arrayFirst(self.allLineItems(), function (i) {
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

                self.callSummaryServiceCallStatus = ko.observable($("#callSummaryServiceCallStatus").val());

                self.deleteServiceCall = function() {
                    bootbox.confirm("Are you sure you want to delete this service call?", function(result) {
                        if (result) {
                            $.ajax({
                                url: urls.ManageServiceCall.DeleteServiceCall,
                                type: "DELETE",
                                data: { serviceCallId: $("#callSummaryServiceCallId").val() }
                            })
                                .fail(function(response) {
                                    toastr.error("There was an issue deleting the service call. Please try again!");
                                })
                                .success(function() {
                                    toastr.success("Success! Service Call deleted.");
                                })
                                .done(function(response) {
                                    window.location = urls.Home.Index;
                                });
                        }
                    });
                };

                self.cssforCallSummaryServiceCallStatus = ko.computed(function () {
                    var className = self.callSummaryServiceCallStatus().toLowerCase().replace(/\ /g, '-');
                    return 'label label-' + className + '-service-call';
                });

                self.callSummaryStatusComplete = ko.computed(function () {
                    if (self.callSummaryServiceCallStatus().toLowerCase() == serviceCallStatusData.Complete.DisplayName.toLowerCase())
                        return true;
                    else
                        return false;
                });
                
                self.callSummaryStatusRequested = ko.computed(function () {
                    if (self.callSummaryServiceCallStatus().toLowerCase() == serviceCallStatusData.Requested.DisplayName.toLowerCase())
                        return true;
                    else
                        return false;
                });

                self.callSummaryStatusOpen = ko.computed(function () {
                    if (self.callSummaryServiceCallStatus().toLowerCase() == serviceCallStatusData.Open.DisplayName.toLowerCase())
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

                self.canBeCompleted = ko.computed(function () {
                    return self.areAllLineItemsCompleted() && !self.callSummaryStatusComplete() && self.callSummaryStatusSigned();
                });
                
                self.canBeDeleted = ko.computed(function () {

                    return self.callSummaryStatusRequested() && self.allLineItems().length == 0;
                });

                self.homeownerVerificationSignature = ko.observable('').extend({ required: true });
                self.homeownerVerificationSignatureDate = ko.observable().extend({required: true}).extend({date: true});
                self.homeownerVerificationCodes = dropdownData.verificationTypes;
                self.homeownerVerificationTypeId = ko.observable().extend({ required: true });                

                self.verifiedHomeownerSignature = ko.observable($("#verifiedHomeownerSignature").val());
                self.verifiedHomeownerSignatureDate = ko.observable($("#verifiedHomeownerSignatureDate").val());
                self.verifiedHomeownerType = ko.observable($("#verifiedHomeownerType").val());

                self.verifiedSignature = ko.computed(function () {
                    var result = self.verifiedHomeownerType() + ' Verification Acknowledged by ' + self.verifiedHomeownerSignature();

                    if (self.verifiedHomeownerSignatureDate()) {
                        result += ' on ' + moment(self.verifiedHomeownerSignatureDate()).format('L');
                    }

                    return result;
                });

                self.saveVerifiedHomeownerSignature = function () {
                    if (formHasErrors([self.homeownerVerificationSignature, self.homeownerVerificationSignatureDate, self.homeownerVerificationTypeId]))
                        return;
                    
                    self.serviceCallId = $("#callSummaryServiceCallId").val();

                    var verifySignatureData = ko.toJSON({
                        serviceCallId: self.serviceCallId, homeownerVerificationSignature: self.homeownerVerificationSignature,
                        homeownerVerificationSignatureDate: self.homeownerVerificationSignatureDate,
                        homeownerVerificationTypeId: self.homeownerVerificationTypeId
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
                            toastr.error("There was an issue saving the homeowner verification. Please try again!");
                        })
                        .done(function (response) {
                            self.callSummaryServiceCallStatus(response.ServiceCallStatus.DisplayName);
                            self.verifiedHomeownerSignature(response.HomeownerVerificationSignature);
                            self.verifiedHomeownerSignatureDate(response.HomeownerVerificationSignatureDate);
                            self.verifiedHomeownerType(response.HomeownerVerificationType.DisplayName);
                            toastr.success("Success! Data saved.");

                            $("#homeownerVerificationSignature").val('');
                            $("#homeownerVerificationSignatureDate").val('');
                            self.homeownerVerificationSignature = '';
                            self.homeownerVerificationSignatureDate = '';
                            self.homeownerVerificationTypeId = '';
                        });
                };
            }
             
            ko.validation.init({
                errorElementClass: 'has-error',
                errorMessageClass: 'help-block',
                decorateElement: true,
                grouping: { deep: true }
            });

            var viewModel = new serviceCallSummaryItemViewModel();

            var persistedAllLineItemsViewModel = modelData.initialServiceLines;

            _(persistedAllLineItemsViewModel).each(function (item) {

                viewModel.allLineItems.push(new AllLineItemsViewModel(item));
            });

            var persistedAllCallNotesViewModel = modelData.initialServiceNotes;

            _(persistedAllCallNotesViewModel).each(function (note) {
                viewModel.allCallNotes.push(new CallNotesViewModel(note));
            });

            var persistedAllAttachmentsViewModel = modelData.initialAttachments;

            _(persistedAllAttachmentsViewModel).each(function (attachment) {
                viewModel.allAttachments.push(new CallAttachmentsViewModel(attachment));
            });

            viewModel.errors = ko.validation.group(viewModel);
             
            ko.applyBindings(viewModel);
         });
        });
    });
});
