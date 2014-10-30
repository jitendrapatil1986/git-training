require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/PhoneNumberType', 'enumeration/ActivityType', 'enumeration/HomeownerContactType', 'jquery.maskedinput', 'enumeration/ServiceCallStatus', 'enumeration/ServiceCallLineItemStatus', 'bootbox', 'app/formUploader', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, ko, koxeditable, urls, toastr, modelData, dropdownData, xeditable, phoneNumberTypeEnum, activityTypeEnum, homeownerContactTypeEnum, maskedInput, serviceCallStatusData, serviceCallLineItemStatusData, bootbox) {
        window.ko = ko; //manually set the global ko property.
        require(['ko.validation'], function () {

        $(function () {
            $("#undoLastCompletedLineItem, #undoLastCompletedLineItemAlert").blur(function () {
                $(this).hide();
            });

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
                $(this).data('editable').input.$input.mask('?(999)-999-9999 **********', { placeholder: " " });
            });

            $(".datepicker-input").datepicker();

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
                self.noteDescriptionToAdd('');
            }

            function AllLineItemsViewModel(options) {
                debugger;
                var self = this;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.completed = options.completed;
                self.numberOfAttachments = options.numberOfAttachments;
                self.numberOfNotes = options.numberOfNotes;
                self.rootCauses = ko.observableArray([]);

                //track line item properties.
                self.problemJdeCode = ko.observable(options.problemJdeCode);
                self.problemCode = ko.observable(options.problemCode);
                self.rootCause = ko.observable(options.rootCause);
                self.editRootCause = ko.observable();
                self.problemDescription = ko.observable(options.problemDescription);
                self.currentProblemCode = ko.observable();
                self.currentProblemJdeCode = ko.observable();
                self.currentRootCause = ko.observable();
                self.currentProblemDescription = ko.observable();

                //track editing problem code, desc, and line altogether.
                self.problemCodeEditing = ko.observable();
                self.problemDescriptionEditing = ko.observable("");
                self.lineEditing = ko.observable("");

                //edit line item.
                self.editLine = function () {
                    this.problemCodeEditing(true);
                    this.problemDescriptionEditing(true);
                    this.lineEditing(true);
                    this.currentProblemCode(this.problemCode());
                    this.currentProblemJdeCode(this.problemJdeCode());
                    this.currentRootCause(this.rootCause());
                    this.currentProblemDescription(this.problemDescription());
                    getRootCauses(self.problemJdeCode(), self.rootCauses, self.rootCause);
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
                    this.problemJdeCode(this.currentProblemJdeCode());
                    this.rootCause(this.currentRootCause());
                    this.problemDescription(this.currentProblemDescription());
                };

                //complete line item.
                self.completeLine = function () {
                    this.lineEditing(false);
                    completeServiceCallLineItem(this);
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

                self.problemCode.subscribe(function (newValue) {
                    if(this.lineEditing())
                        getRootCauses(newValue, self.rootCauses);
                });

                function getRootCauses(problemJdeCode, rootCauses, rootCause) {
                    $.ajax({
                        url: urls.RootCause.RootCauses + '?problemJdeCode=' + problemJdeCode,
                        type: "GET",
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    }).done(function(response) {
                        rootCauses(response);
                        self.editRootCause(rootCause());
                    });
                }
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
            
            function AddtionalEmailContactViewModel(options) {
                var self = this;
                self.contactValue = ko.observable(options.contactValue).extend({ required: true, email: true });
            }

            function AddtionalPhoneContactViewModel(options) {
                var self = this;

                self.contactValue = ko.observable(options.contactValue).extend({ required: true });
            }

            function updateServiceCallLineItem(line) {
                var updateProblemCode = $("#allServiceCallLineItems[data-service-call-line-item='" + line.lineNumber() + "'] #updateCallLineProblemCode");
                if (updateProblemCode.val() == "") {
                    $(updateProblemCode).parent().addClass("has-error");
                    return;
                }
                line.problemCode($("#allServiceCallLineItems[data-service-call-line-item='" + line.lineNumber() + "'] #updateCallLineProblemCode").find('option:selected').text());

                var updateRootCause = $("#allServiceCallLineItems[data-service-call-line-item='" + line.lineNumber() + "'] #updateCallLineRootCause");
                if (updateRootCause.val() == "") {
                    $(updateRootCause).parent().addClass("has-error");
                    return;
                }
                line.rootCause($("#allServiceCallLineItems[data-service-call-line-item='" + line.lineNumber() + "'] #updateCallLineRootCause").find('option:selected').text());

                var updateProblemDescription = $("#allServiceCallLineItems[data-service-call-line-item='" + line.lineNumber() + "'] #updateCallLineProblemDescription");
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
                        line.serviceCallLineItemStatusDisplayName(response.DisplayName);

                        //if user is not allowed to ALWAYS reopen Completed lines at anytime, then allow them to reopen only right after completing a line.
                        if ($("#userCanReopenCallLinesAnytime").val() == false) {
                            $("#undoLastCompletedLineItemAlert").attr('data-service-line-id-to-undo', line.serviceCallLineItemId);
                            $("#undoLastCompletedLineItemAlert").show();
                            viewModel.lineJustCompleted(true);
                            $("#undoLastCompletedLineItemAlert").attr("tabindex", -1).focus();  //focus only after setting lineJustCompleted observable which visibly shows control on form first and then focus.
                        } else {
                            toastr.success("Success! Item completed.");
                        }
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
                        line.serviceCallLineItemStatusDisplayName(response.DisplayName);
                    });
            }

            function serviceCallSummaryItemViewModel() {
                var self = this;

                self.allLineItems = ko.observableArray([]);
                self.theLookups = dropdownData.availableLookups;  //dropdown list does not need to be observable. Only the actual elements w/i the array do.
                self.problemDescriptionToAdd = ko.observable('');
                self.problemJdeCodeToAdd = ko.observable();
                self.rootCauseToAdd = ko.observable();
                self.canBeReopened = ko.observable(modelData.canBeReopened);
                self.isSpecialProject = ko.observable(modelData.isSpecialProject);
                self.specialProjectReason = ko.observable(modelData.specialProjectReason);
                self.specialProjectDate = ko.observable(modelData.specialProjectDate);
                self.isEscalated = ko.observable(modelData.isEscalated);
                self.escalationReason = ko.observable(modelData.escalationReason);
                self.escalationDate = ko.observable(modelData.escalationDate);
                self.allCallNotes = ko.observableArray([]);
                self.allAttachments = ko.observableArray([]);
                self.additionalEmailContacts = ko.observableArray([]);
                self.additionalPhoneContacts = ko.observableArray([]);
                self.noteDescriptionToAdd = ko.observable('');
                self.userCanAlwaysReopenCallLines = ko.observable();
                self.rootCauses = ko.observableArray([]);

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
                        var problemJdeCode = $("#addCallLineProblemCode").find('option:selected').val();
                        getRootCauses(problemJdeCode, self.rootCauses);
                    } else {
                        self.rootCauses([]);
                    }
                    
                    
                });

                function getRootCauses(problemJdeCode, rootCauses) {
                    $.ajax({
                        url: urls.RootCause.RootCauses + '?problemJdeCode=' + problemJdeCode,
                        type: "GET",
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    }).done(function (response) {
                        rootCauses(response);
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
                

                self.submit = function () {

                    if (self.errors().length != 0) {
                        self.errors.showAllMessages();
                        return;
                    }

                    var addtionalContactInfo =
                    {
                        additionalPhoneContacts: self.additionalPhoneContacts,
                        homeownerId: $("#HomeownerId").val(),
                        additionalEmailContacts: self.additionalEmailContacts
                    };

                    var newAddtionalContactInfo = ko.toJSON(addtionalContactInfo);

                    $.ajax({
                        url: modelData.saveAdditionalContactsUrl,
                        type: "POST",
                        data: newAddtionalContactInfo,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function (response) {

                            toastr.error("There was an issue saving the additional contact info. Please try again!");
                        })
                        .done(function (response) {
                            toastr.success("Success! Additional contact info saved.");
                        });
                };

                self.addEmailContact = function () {
                    self.additionalEmailContacts.push(new AddtionalEmailContactViewModel({
                        contactType: homeownerContactTypeEnum.Email.Value,
                        contactValue: null
                    }));
                };

                self.addPhoneContact = function () {
                    self.additionalPhoneContacts.push(new AddtionalPhoneContactViewModel({
                        contactType: homeownerContactTypeEnum.Phone.Value,
                        contactValue: null
                    }));
                };

                self.addLineItem = function () {
                    self.serviceCallId = $("#callSummaryServiceCallId").val();
                    self.problemCode = $("#addCallLineProblemCode").find('option:selected').text();
                    self.rootCause = $("#addCallLineRootCause").find('option:selected').text();
                    self.problemJdeCode = $("#addCallLineProblemCode").val();
                    self.problemDescription = $("#addCallLineProblemDescription").val();

                    var newProblemCode = $("#addCallLineProblemCode");
                    if (newProblemCode.val() == "") {
                        $(newProblemCode).parent().addClass("has-error");
                        return;
                    }

                    var newRootCause = $("#addCallLineRootCause");
                    if (newRootCause.val() == "") {
                        $(newRootCause).parent().addClass("has-error");
                        return;
                    }

                    var newProblemDescription = $("#addCallLineProblemDescription");
                    if (newProblemDescription.val() == "") {
                        $(newProblemDescription).parent().addClass("has-error");
                        return;
                    }

                    var newLineItem = new AllLineItemsViewModel({

                        serviceCallId: self.serviceCallId, problemJdeCode: self.problemJdeCode,
                        problemCode: self.problemCode, problemDescription: self.problemDescription,
                        rootCause: self.rootCause
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
                                rootCause: self.rootCause,
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
                        });

                    $(newProblemCode).parent().removeClass("has-error");
                    $(newProblemDescription).parent().removeClass("has-error");
                };

                self.addCallNote = function () {
                    self.serviceCallId = $("#callSummaryServiceCallId").val();
                    self.note = $("#addCallNoteDescription").val();

                    var newNoteDescription = $("#addCallNoteDescription");
                    if (newNoteDescription.val() == "") {
                        $(newNoteDescription).parent().addClass("has-error");
                        return;
                    }

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

                self.lineJustCompleted = ko.observable();

                //undo last line item which was completed.
                self.undoLastCompletedLine = function () {
                    var lineId = $("#undoLastCompletedLineItemAlert").attr('data-service-line-id-to-undo');
                    var lineToReopen = ko.utils.arrayFirst(self.allLineItems(), function (i) {
                        return (i.serviceCallLineItemId == lineId);
                    });
                    reopenServiceCallLineItem(lineToReopen);
                    self.lineJustCompleted(false);
                };

                self.callSummaryServiceCallStatus = ko.observable($("#callSummaryServiceCallStatus").html());

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

                self.homeownerVerificationSignature = ko.observable('');
                self.homeownerVerificationSignatureDate = ko.observable();

                self.verifiedHomeownerSignature = ko.observable($("#verifiedHomeownerSignature").val());
                self.verifiedHomeownerSignatureDate = ko.observable($("#verifiedHomeownerSignatureDate").val());

                self.verifiedSignature = ko.computed(function () {
                    var result = 'Verification signed by ' + self.verifiedHomeownerSignature();

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
            ko.validation.init({
                errorElementClass: 'has-error',
                errorMessageClass: 'help-block',
                decorateElement: true
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

            var additionalEmailContacsViewModel = modelData.additionalEmailContacts;

            _(additionalEmailContacsViewModel).each(function (contact) {
                viewModel.additionalEmailContacts.push(new AddtionalEmailContactViewModel(contact));
            });

            var additionalPhoneContacsViewModel = modelData.additionalPhoneContacts;

            _(additionalPhoneContacsViewModel).each(function (contact) {
                viewModel.additionalPhoneContacts.push(new AddtionalPhoneContactViewModel(contact));
            });

            viewModel.errors = ko.validation.group(viewModel);
            ko.applyBindings(viewModel);
    });
});
});
});