require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'moment', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/PaymentStatus', 'enumeration/PhoneNumberType', 'enumeration/ActivityType', 'jquery.maskedinput', 'enumeration/ServiceCallStatus', 'enumeration/ServiceCallLineItemStatus', 'bootbox', 'app/formUploader', 'app/serviceCall/SearchVendor', 'app/serviceCall/SearchBackchargeVendor', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, ko, koxeditable, moment, urls, toastr, modelData, dropdownData, xeditable, paymentStatusEnum, phoneNumberTypeEnum, activityTypeEnum, maskedInput, serviceCallStatusData, serviceCallLineItemStatusData, bootbox) {
        window.ko = ko;  //manually set the global ko property.

        require(['ko.validation'], function() {
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
            
            function PaymentViewModel(options) {
                var self = this;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.vendorNumber = options.vendorNumber;
                self.vendorName = options.vendorName;
                self.backchargeVendorNumber = options.backchargeVendorNumber;
                self.backchargeVendorName = options.backchargeVendorName;
                self.invoiceNumber = options.invoiceNumber;
                self.amount = options.amount;
                self.backchargeAmount = options.backchargeAmount;
                self.isBackcharge = options.isBackcharge;
                self.backchargeReason = options.backchargeReason;
                self.personNotified = options.personNotified;
                self.personNotifiedPhoneNumber = options.personNotifiedPhoneNumber;
                self.personNotifiedDate = options.personNotifiedDate;
                self.backchargeResponseFromVendor = options.backchargeResponseFromVendor;
                self.paymentId = options.paymentId;
                self.paymentCreatedDate = options.paymentCreatedDate;
                self.holdComments = ko.observable(options.holdComments);
                self.holdDate = ko.observable(options.holdDate);
                self.selectedCostCode = options.selectedCostCode;
                if (options.paymentStatusDisplayName) {
                    self.paymentStatusDisplayName = ko.observable(options.paymentStatusDisplayName);
                }

                self.isHeld = ko.computed(function () {
                    return self.paymentStatusDisplayName() == paymentStatusEnum.RequestedHold.DisplayName || self.paymentStatusDisplayName() == paymentStatusEnum.Hold.DisplayName;
                });
              
                self.deletePayment = function () {
                    var payment = this;
                    bootbox.confirm("Are you sure you want to delete this payment?", function (result) {
                        if (result) {
                            var actionUrl = urls.ManageServiceCall.DeletePayment;
                            $.ajax({
                                type: "DELETE",
                                url: actionUrl,
                                data: { PaymentId: payment.paymentId },
                                success: function () {
                                    viewModel.allPayments.remove(payment);
                                    toastr.success("Success! Payment deleted.");
                                }
                            });
                        }
                    });
                };

                self.displayHold = function (item, event) {
                    displayPopoUp('hold', item, event);
                };

                var displayPopoUp = function(name, item, event) {
                    var button = $(event.target);
                    $('.btn-action-with-popup').removeClass('active');
                    button.removeClass('btn-hover-show');
                    button.addClass("active");
                    $('.popup-action-with-message').hide();
                    var right = ($(window).width() - (button.offset().left + button.outerWidth()));
                    var actionwithPopup = name + '-' + item.paymentId;
                    $("#" + actionwithPopup).css({
                        'position': 'absolute',
                        'right': right,
                        'top': button.offset().top + button.height() + 15
                    }).show();
                };

                self.holdPayment = function (item, event) {
                    var actionUrl = urls.ManageServiceCall.AddPaymentOnHold;
                    $.ajax({
                        type: "POST",
                        url: actionUrl,
                        data: { PaymentId: item.paymentId, message: item.holdComments },
                        success: function (response) {
                            item.paymentStatusDisplayName(response.NewStatusDisplayName);
                            item.holdDate(response.Date);
                            toastr.success("Success! Hold request sent.");
                            closeWindow(event);
                        }
                    });
                };

                self.shouldDisplayOnHold = ko.computed(function () {
                    return self.paymentStatusDisplayName() != paymentStatusEnum.RequestedHold.DisplayName && self.paymentStatusDisplayName() != paymentStatusEnum.RequestedDeny.DisplayName;
                });
                
                self.approvePayment = function (item, event) {
                    var actionUrl = urls.ManageServiceCall.ApprovePayment;
                    $.ajax({
                        type: "POST",
                        url: actionUrl,
                        data: { PaymentId: item.paymentId, message: item.holdComments },
                        success: function (response) {
                            item.paymentStatusDisplayName(response);
                            toastr.success("Success! Approval Request sent.");
                            closeWindow(event);
                        }
                    });
                };
                
                self.cancelPopup = function (item, event) {
                    closeWindow(event);
                };

                var closeWindow = function (event) {
                    var button = $(event.target);
                    var window = button.parent();
                    var parentButton = $("#btn_" + window.attr('id'));
                    parentButton.removeClass("active");
                    parentButton.addClass('btn-hover-show');
                    window.hide();
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

                self.invoiceNumber = ko.observable('').extend({ required: true });
                self.amount = ko.observable('').extend({ required: true });
                self.isBackcharge = ko.observable(false);
                self.selectedCostCode = ko.observable(undefined).extend({ required: true });
                self.warrantyCostCodes = ko.observableArray(modelData.warrantyCostCodes);
                self.backchargeAmount = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.backchargeReason = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.personNotified = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.personNotifiedPhoneNumber = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.personNotifiedDate = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.backchargeResponseFromVendor = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.vendorName = ko.observable('').extend({ required: true });
                self.vendorNumber = ko.observable('').extend({ required: true });
                self.backchargeVendorName = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.backchargeVendorNumber = ko.observable('').extend({
                    required: {
                        onlyIf: function () { return (self.isBackcharge() === true); }
                    }
                });
                self.allPayments = ko.observableArray([]);
                
                self.canAddPayment = ko.computed(function () {
                    return true;
                });
                
                self.clearPaymentFields = function () {
                    
                    $('#vendor-search').val('');
                    $('#backcharge-vendor-search').val('');
                    self.vendorNumber('');
                    self.backchargeVendorNumber('');
                    self.invoiceNumber('');
                    self.amount('');
                    self.backchargeAmount('');
                    self.isBackcharge(false);
                    self.backchargeReason('');
                    self.personNotified('');
                    self.personNotifiedPhoneNumber('');
                    self.personNotifiedDate('');
                    self.backchargeResponseFromVendor('');
                    self.selectedCostCode(undefined);
                    self.errors.showAllMessages(false);
                };
                
                $(document).on('vendor-number-selected', function () {
                    var vendorNumber = $('#vendor-search').data('vendor-number');
                    var vendorName = $('#vendor-search').data('vendor-name');
                    self.vendorNumber(vendorNumber);
                    self.vendorName(vendorName);
                });
                $(document).on('backcharge-vendor-number-selected', function () {
                    var vendorNumber = $('#backcharge-vendor-search').data('vendor-number');
                    var vendorName = $('#backcharge-vendor-search').data('vendor-name');
                    
                    self.backchargeVendorNumber(vendorNumber);
                    self.backchargeVendorName(vendorName);
                });
                
                self.addPayment = function () {
                    if (self.errors().length != 0) {
                        self.errors.showAllMessages();
                        return;
                    }
                    
                    self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                    self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;
                    
                    var newPayment = new PaymentViewModel({
                        serviceCallLineItemId : self.serviceCallLineItemId,
                        vendorNumber : self.vendorNumber(),
                        vendorName : self.vendorName(),
                        backchargeVendorNumber : self.backchargeVendorNumber(),
                        backchargeVendorName: self.backchargeVendorName(),
                        invoiceNumber : self.invoiceNumber(),
                        amount : self.amount(),
                        backchargeAmount: self.backchargeAmount(),
                        isBackcharge : self.isBackcharge(),
                        backchargeReason : self.backchargeReason(),
                        personNotified : self.personNotified(),
                        personNotifiedPhoneNumber : self.personNotifiedPhoneNumber(),
                        personNotifiedDate : self.personNotifiedDate(),
                        backchargeResponseFromVendor: self.backchargeResponseFromVendor(),
                        paymentStatusDisplayName: paymentStatusEnum.Requested.DisplayName,
                        selectedCostCode: self.selectedCostCode
                    });

                    var paymentData = ko.toJSON(newPayment);

                    $.ajax({
                        url: urls.ManageServiceCall.AddPayment,
                        type: "POST",
                        data: paymentData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function (response) {
                            toastr.error("There was a problem adding the payment. Please try again.");
                        })
                        .done(function (response) {
                            
                            newPayment.paymentId = response;
                            self.allPayments.unshift(newPayment);
                            toastr.success("Success! Payment added.");
                            highlight($("#allServiceCallPayments").first());
                            self.clearPaymentFields();
                        });
                };
                
                $("#Person_Notified_Date").datepicker({
                    format: 'm/d/yyyy'
                });
                
                $("#Person_Notified_Date").on('changeDate', function (e) {
                    self.personNotifiedDate(moment(e.date).format("L"));
                });
                
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
                    bootbox.confirm(modelData.attachmentRemovalMessage, function (result) {
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
            
            ko.validation.init({
                errorElementClass: 'has-error',
                errorMessageClass: 'help-block',
                decorateElement: true
            });
            
            var viewModel = new serviceCallLineItemViewModel();
            viewModel.errors = ko.validation.group(viewModel);
            ko.applyBindings(viewModel);

            var persistedAllCallNotesViewModel = modelData.initialServiceCallLineNotes;

            _(persistedAllCallNotesViewModel).each(function (note) {
                viewModel.allCallNotes.push(new CallNotesViewModel(note));
            });

            var persistedAllAttachmentsViewModel = modelData.initialServiceCallLineAttachments;

            _(persistedAllAttachmentsViewModel).each(function (attachment) {
                viewModel.allAttachments.push(new CallAttachmentsViewModel(attachment));
            });
            
            var persistedAllPaymentsViewModel = modelData.initialServiceCallLinePayments;

            _(persistedAllPaymentsViewModel).each(function (payment) {
                viewModel.allPayments.push(new PaymentViewModel(payment));
            });
        });
    });
});
});