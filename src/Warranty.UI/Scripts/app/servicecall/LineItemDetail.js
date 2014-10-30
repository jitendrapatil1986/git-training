require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/PhoneNumberType', 'enumeration/ActivityType', 'jquery.maskedinput', 'enumeration/ServiceCallStatus', 'enumeration/ServiceCallLineItemStatus', 'bootbox', 'app/formUploader', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, ko, koxeditable, urls, toastr, modelData, dropdownData, xeditable, phoneNumberTypeEnum, activityTypeEnum, maskedInput, serviceCallStatusData, serviceCallLineItemStatusData, bootbox) {

        $(function () {
            $("#undoLastCompletedLineItem, #undoLastCompletedLineItemAlert").blur(function () {
                $(this).hide();
            });

            $.fn.editable.defaults.mode = 'inline';
            $.fn.editable.defaults.emptytext = 'Add';
            $.fn.editableform.buttons =
                '<button type="submit" class="btn btn-primary editable-submit btn-sm"><i class="glyphicon glyphicon-ok"></i></button>';

            $(".attached-file-display-name").editable();

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

            function CallNotesViewModel(options) {
                var self = this;
                self.serviceCallNoteId = options.serviceCallNoteId;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = ko.observable(options.serviceCallLineItemId);
                self.note = options.note;
                self.createdBy = options.createdBy;
                self.createdDate = options.createdDate;
                self.serviceCallCommentTypeId = options.serviceCallCommentTypeId;
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

            function updateServiceCallLineItem(line) {
                var updateProblemCode = $("#allServiceCallLineItems[data-service-call-line-item='" + line.lineNumber() + "'] #updateCallLineProblemCode");
                if (updateProblemCode.val() == "") {
                    $(updateProblemCode).parent().addClass("has-error");
                    return;
                }

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

            function serviceCallLineItemViewModel() {
                var self = this;

                self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;
                self.completed = modelData.initialServiceCallLineItem.completed;

                //track line item properties.
                self.problemCodeId = ko.observable(modelData.initialServiceCallLineItem.problemCodeId);
                self.problemCode = ko.observable(modelData.initialServiceCallLineItem.problemCode);
                self.problemDetailCode = ko.observable(modelData.initialServiceCallLineItem.problemDetailCode);
                self.problemDescription = ko.observable(modelData.initialServiceCallLineItem.problemDescription);
                self.currentProblemCode = ko.observable();
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

                //reopen line item.
                self.reopenLine = function () {
                    this.lineEditing(false);
                    reopenServiceCallLineItem(this);
                };

                self.lineNumber = ko.observable(modelData.initialServiceCallLineItem.lineNumber);

                self.lineNumberWithProblemCode = ko.computed(function () {
                    return self.lineNumber() + " - " + self.problemCode();
                });

                self.serviceCallLineItemStatus = ko.observable(modelData.initialServiceCallLineItem.serviceCallLineItemStatus);

                self.serviceCallLineItemStatusDisplayName = ko.observable('');
                if (modelData.initialServiceCallLineItem.serviceCallLineItemStatus) {
                    if (modelData.initialServiceCallLineItem.serviceCallLineItemStatus.displayName)
                        self.serviceCallLineItemStatusDisplayName(modelData.initialServiceCallLineItem.serviceCallLineItemStatus.displayName);  //TODO: displayName works for model passed into js file via toJSON().
                    if (modelData.initialServiceCallLineItem.serviceCallLineItemStatus.DisplayName)
                        self.serviceCallLineItemStatusDisplayName(modelData.initialServiceCallLineItem.serviceCallLineItemStatus.DisplayName);  //TODO: DisplayName works for model passed from ajax call. Need to keep both similar.
                }

                self.lineItemStatusCSS = ko.computed(function () {
                    return self.serviceCallLineItemStatusDisplayName() ? 'label label-' + self.serviceCallLineItemStatusDisplayName().toLowerCase() + '-service-line-item' : '';
                });

                self.isLineItemCompleted = function () {
                    if (!self.serviceCallLineItemStatusDisplayName())
                        return false;

                    return self.serviceCallLineItemStatusDisplayName().toLowerCase() == serviceCallLineItemStatusData.Complete.DisplayName.toLowerCase() ? true : false;
                };

                self.theLookups = dropdownData.availableLookups;  //dropdown list does not need to be observable. Only the actual elements w/i the array do.
                self.allCallNotes = ko.observableArray([]);
                self.allAttachments = ko.observableArray([]);
                self.noteDescriptionToAdd = ko.observable('');
                self.userCanAlwaysReopenCallLines = ko.observable();

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

                self.addCallNote = function () {
                    self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                    self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;
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

                            self.serviceCallLineItemStatusDisplayName(response.ServiceCallLineItemStatus.DisplayName);
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
                    reopenServiceCallLineItem(this);
                    self.lineJustCompleted(false);
                };
            }

            var viewModel = new serviceCallLineItemViewModel();
            ko.applyBindings(viewModel);

            var persistedAllCallNotesViewModel = modelData.initialServiceCallLineNotes;

            _(persistedAllCallNotesViewModel).each(function (note) {
                viewModel.allCallNotes.push(new CallNotesViewModel(note));
            });

            var persistedAllAttachmentsViewModel = modelData.initialServiceCallLineAttachments;

            _(persistedAllAttachmentsViewModel).each(function (attachment) {
                viewModel.allAttachments.push(new CallAttachmentsViewModel(attachment));
            });
        });
    });
});