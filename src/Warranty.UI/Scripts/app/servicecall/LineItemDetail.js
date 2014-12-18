require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'moment', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/PaymentStatus', 'enumeration/BackchargeStatus', 'enumeration/PhoneNumberType', 'enumeration/ActivityType', 'jquery.maskedinput', 'enumeration/ServiceCallStatus', 'enumeration/ServiceCallLineItemStatus', 'enumeration/PurchaseOrderLineItemStatus', 'bootbox', 'app/formUploader', 'app/serviceCall/SearchVendor', 'app/serviceCall/SearchBackchargeVendor', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, ko, koxeditable, moment, urls, toastr, modelData, dropdownData, xeditable, paymentStatusEnum, backchargeStatusEnum, phoneNumberTypeEnum, activityTypeEnum, maskedInput, serviceCallStatusData, serviceCallLineItemStatusData, purchaseOrderLineItemStatusEnum, bootbox) {
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

                $(".attached-file-display-name").editable();

                //$("#rootProblemId").editable({
                //    type: 'select',
                //    pk: modelData.initialServiceCallLineItem.serviceCallLineItemId,
                //    value: modelData.initialServiceCallLineItem.rootProblem,
                //    emptytext: 'Set Root Problem',
                //    source: modelData.rootProblemCodes,
                //    success: function (response) {
                //        viewModel.completeButtonClicked(false);
                //        toastr.success("Successfully updated root problem");
                //    },
                //    error: function (response) { toastr.error("There was an error updating the root problem"); },
                //});
                
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

                function PurchaseOrderViewModel(options) {
                    var self = this;

                    self.serviceCallLineItemId = options.serviceCallLineItemId;
                    self.purchaseOrderNumber = options.purchaseOrderNumber;
                    self.vendorNumber = options.vendorNumber;
                    self.vendorName = options.vendorName;
                    self.createdDate = moment(options.createdDate).format("L");
                    self.totalCost = options.totalCost;
                    self.purchaseOrderStatusDisplayName = ko.observable(options.purchaseOrderStatusDisplayName);

                    self.purchaseOrderStatusBadgeClassName = ko.computed(function() {
                        if (self.purchaseOrderStatusDisplayName() == purchaseOrderLineItemStatusEnum.Closed.DisplayName) {
                            return "success";
                        }
                        return "default";
                    });
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
                    self.backchargeId = options.backchargeId;
                    self.isBackcharge = options.isBackcharge;
                    self.backchargeReason = options.backchargeReason;
                    self.personNotified = options.personNotified;
                    self.personNotifiedPhoneNumber = options.personNotifiedPhoneNumber;
                    self.personNotifiedDate = options.personNotifiedDate;
                    self.backchargeResponseFromVendor = options.backchargeResponseFromVendor;
                    self.paymentId = options.paymentId;
                    self.paymentCreatedDate = options.paymentCreatedDate;
                    self.holdComments = ko.observable(options.holdComments);
                    self.backchargeHoldComments = ko.observable(options.backchargeHoldComments);
                    self.backchargeDenyComments = ko.observable(options.backchargeDenyComments);
                    self.holdDate = ko.observable(options.holdDate);
                    self.backchargeHoldDate = ko.observable(options.backchargeHoldDate);
                    self.backchargeDenyDate = ko.observable(options.backchargeDenyDate);
                    self.selectedCostCode = options.selectedCostCode;
                    self.paymentStatusDisplayName = ko.observable(options.paymentStatusDisplayName);
                    self.backchargeStatusDisplayName = ko.observable(options.backchargeStatusDisplayName);

                    self.isBackchargeHeld = ko.computed(function () {
                        return self.backchargeStatusDisplayName() == backchargeStatusEnum.RequestedHold.DisplayName || self.backchargeStatusDisplayName() == backchargeStatusEnum.Hold.DisplayName;
                    });
                    
                    self.isBackchargeDenied = ko.computed(function () {
                        return self.backchargeStatusDisplayName() == backchargeStatusEnum.RequestedDeny.DisplayName || self.backchargeStatusDisplayName() == backchargeStatusEnum.Denied.DisplayName;
                    });
                    
                    self.isHeld = ko.computed(function () {
                        return self.paymentStatusDisplayName() == paymentStatusEnum.RequestedHold.DisplayName || self.paymentStatusDisplayName() == paymentStatusEnum.Hold.DisplayName;
                    });

                    self.displayHold = function (item, event) {
                        displayPopoUp('hold', item, event);
                    };
                    
                    self.displayHoldBackcharge = function (item, event) {
                        displayPopoUp('holdBackcharge', item, event, true);
                    };
                    
                    self.displayDenyBackcharge = function (item, event) {
                        displayPopoUp('denyBackcharge', item, event, true);
                    };


                    var displayPopoUp = function (name, item, event, displayToTheRight) {
                        var button = $(event.target);
                        $('.btn-action-with-popup').removeClass('active');
                        button.removeClass('btn-hover-show');
                        button.addClass("active");
                        $('.popup-action-with-message').hide();
                        var right = ($(window).width() - (button.offset().left + button.outerWidth()));
                        var actionwithPopup = name + '-' + item.paymentId;
                        if (displayToTheRight) {
                            right = right - 250;
                        }
                        $("#" + actionwithPopup).css({
                            'position': 'absolute',
                            'right': right,
                            'top': button.offset().top + button.height() + 15
                        }).show();
                    };
                    

                    self.approvePayment = function (item, event) {
                        var actionUrl = urls.ManageServiceCall.ApprovePayment;
                        $.ajax({
                            type: "POST",
                            url: actionUrl,
                            data: { PaymentId: item.paymentId },
                            success: function (response) {
                                item.paymentStatusDisplayName(response);
                                toastr.success("Success! Approval Request sent.");
                                closeWindow(event);
                            }
                        });
                    };
                    
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
                    
                    self.holdBackcharge = function (item, event) {
                        var actionUrl = urls.ManageServiceCall.HoldBackcharge;
                        $.ajax({
                            type: "POST",
                            url: actionUrl,
                            data: { BackchargeId: item.backchargeId, message: item.backchargeHoldComments },
                            success: function (response) {
                                item.backchargeStatusDisplayName(response.NewStatusDisplayName);
                                item.backchargeHoldDate(response.Date);
                                toastr.success("Success! Backcharge Hold request sent.");
                                closeWindow(event);
                            }
                        });
                    };
                    
                    self.denyBackcharge = function (item, event) {
                        var actionUrl = urls.ManageServiceCall.DenyBackcharge;
                        $.ajax({
                            type: "POST",
                            url: actionUrl,
                            data: { BackchargeId: item.backchargeId, message: item.backchargeDenyComments },
                            success: function (response) {
                                item.backchargeStatusDisplayName(response.NewStatusDisplayName);
                                item.backchargeDenyDate(response.Date);
                                toastr.success("Success! Backcharge deny request sent.");
                                closeWindow(event);
                            }
                        });
                    };
                    
                    self.approveBackcharge = function (item, event) {
                        var actionUrl = urls.ManageServiceCall.ApproveBackcharge;
                        $.ajax({
                            type: "POST",
                            url: actionUrl,
                            data: { BackchargeId: item.backchargeId },
                            success: function (response) {
                                item.backchargeStatusDisplayName(response);
                                toastr.success("Success! Backcharge Approval Request sent.");
                                closeWindow(event);
                            }
                        });
                    };

                    self.shouldDisplayHoldPayment = ko.computed(function () {
                        return self.paymentStatusDisplayName() == paymentStatusEnum.Pending.DisplayName;
                    });

                    self.paymentStatusBadgeClassName = ko.computed(function () {
                        if (self.paymentStatusDisplayName() == paymentStatusEnum.Hold.DisplayName) {
                            return "warning";
                        }
                        else if (self.paymentStatusDisplayName() == paymentStatusEnum.Approved.DisplayName) {
                            return "success";
                        }
                        return "default";
                    });
                    
                    self.backchargeStatusBadgeClassName = ko.computed(function () {
                        if (self.backchargeStatusDisplayName() == backchargeStatusEnum.Hold.DisplayName) {
                            return "warning";
                        }
                        else if (self.backchargeStatusDisplayName() == backchargeStatusEnum.Approved.DisplayName) {
                            return "success";
                        }
                        else if (self.backchargeStatusDisplayName() == backchargeStatusEnum.Denied.DisplayName) {
                            return "danger";
                        }
                        return "default";
                    });

                    self.shouldDisplayApprovePayment = ko.computed(function () {
                        return self.paymentStatusDisplayName() == paymentStatusEnum.Pending.DisplayName || self.paymentStatusDisplayName() == paymentStatusEnum.Hold.DisplayName;
                    });
                    
                    self.shouldDisplayDeletePayment = ko.computed(function () {
                        return self.paymentStatusDisplayName() == paymentStatusEnum.Pending.DisplayName || self.paymentStatusDisplayName() == paymentStatusEnum.Hold.DisplayName;
                    });
                    
                    self.shouldDisplayApproveBackcharge = ko.computed(function () {
                        return self.backchargeStatusDisplayName() == backchargeStatusEnum.Pending.DisplayName || self.backchargeStatusDisplayName() == backchargeStatusEnum.Hold.DisplayName;
                    });
                    
                    self.shouldDisplayDenyBackcharge = ko.computed(function () {
                        return self.backchargeStatusDisplayName() == backchargeStatusEnum.Pending.DisplayName || self.backchargeStatusDisplayName() == backchargeStatusEnum.Hold.DisplayName;
                    });
                    
                    self.shouldDisplayHoldBackcharge = ko.computed(function () {
                        return self.backchargeStatusDisplayName() == backchargeStatusEnum.Pending.DisplayName;
                    });
                                        
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
                }

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
                                $("#undoLastCompletedLineItemAlert").attr("tabindex", -1).focus(); //focus only after setting lineJustCompleted observable which visibly shows control on form first and then focus.
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
                            line.serviceCallLineItemStatusDisplayName(response.ServiceCallLineItemStatus.DisplayName);
                        });
                }

                function serviceCallLineItemViewModel() {
                    var self = this;

                    self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                    self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;
                    self.completed = modelData.initialServiceCallLineItem.completed;
                    self.completeButtonClicked = ko.observable(false);

                    //track line item properties.
                    self.problemCodeId = ko.observable(modelData.initialServiceCallLineItem.problemCodeId);
                    self.problemCode = ko.observable(modelData.initialServiceCallLineItem.problemCode);
                    self.problemDetailCode = ko.observable(modelData.initialServiceCallLineItem.problemDetailCode);
                    self.problemDescription = ko.observable(modelData.initialServiceCallLineItem.problemDescription);
                    self.currentProblemCode = ko.observable();
                    self.currentProblemDescription = ko.observable();
                    self.jobNumber = ko.observable(modelData.initialServiceCallLineItem.jobNumber);
                    self.costCode = ko.observable(modelData.initialServiceCallLineItem.costCode);
                    self.constructionVendors = ko.observableArray([]);
                    self.constructionVendorsLoading = ko.observable(true);

                    self.rootCause = ko.observable(modelData.initialServiceCallLineItem.rootCause);  //Value saved in db is string but ddl needs id to set default value..
                    var selectedRootCause = ko.utils.arrayFirst(modelData.rootCauseCodes, function (item) {
                        return item.displayText === modelData.initialServiceCallLineItem.rootCause;
                    });
                    self.rootCauseId = ko.observable(selectedRootCause ? selectedRootCause.value : '').extend({
                        required:
                        {
                            onlyIf: function() {
                                return self.completeButtonClicked() === true;
                            }
                        }
                    });
                    
                    self.rootProblem = ko.observable(modelData.initialServiceCallLineItem.rootProblem);
                    var selectedRootProblem = ko.utils.arrayFirst(modelData.rootProblemCodes, function (item) {
                        debugger;
                        return item.displayText === modelData.initialServiceCallLineItem.rootProblem;
                    });
                    self.rootProblemId = ko.observable(selectedRootProblem ? selectedRootProblem.value : '').extend({
                        required: {
                            onlyIf: function () {
                                return self.completeButtonClicked() === true;
                            }
                        }
                    });

                    self.rootCauseCodes = ko.observableArray(modelData.rootCauseCodes);
                    self.rootProblemCodes = ko.observableArray(modelData.rootProblemCodes);

                    //TODO: Review why this is called 3 times.
                    self.rootCauseId.subscribe(function (rootCauseId) {
                        //var matchedRootCause = ko.utils.arrayFirst(modelData.rootCauseCodes, function (item) {
                        //    debugger;
                        //    return Number(item.value) === Number(rootCauseId);
                        //});
                        
                        $.ajax({
                            url: urls.ManageServiceCall.EditLineItem,
                            type: "POST",
                            data: ko.toJSON({ serviceCallLineItemId: self.serviceCallLineItemId, rootCause: rootCauseId }), //matchedRootCause.displayText }),
                            dataType: "json",
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        }).fail(function() {
                            toastr.error("There was an error updating the root cause");
                        }).success(function() {
                            toastr.success("Successfully updated root cause");
                        });
                    });

                    self.rootProblemId.subscribe(function (rootProblemId) {
                        debugger;
                        $.ajax({
                            url: urls.ManageServiceCall.EditLineItem,
                            type: "POST",
                            data: ko.toJSON({ serviceCallLineItemId: self.serviceCallLineItemId, rootProblem: rootProblemId }),
                            dataType: "json",
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        }).fail(function () {
                            toastr.error("There was an error updating the root problem");
                        }).success(function () {
                            toastr.success("Successfully updated root problem");
                        });
                    });

                    //track editing problem code, desc, and line altogether.
                    self.problemCodeEditing = ko.observable();
                    self.problemDescriptionEditing = ko.observable("");
                    self.lineEditing = ko.observable("");

                    self.invoiceNumber = ko.observable('').extend({ required: true });
                    self.amount = ko.observable().extend({ required: true, min: 0 });
                    self.isBackcharge = ko.observable(false);
                    self.selectedCostCode = ko.observable(undefined).extend({ required: true });
                    self.warrantyCostCodes = ko.observableArray(modelData.warrantyCostCodes);
                    self.backchargeAmount = ko.observable().extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true); }
                        },
                        validation: {
                            validator: function (val, someOtherVal) {
                                if (self.isBackcharge() === false)
                                    return true;
                                
                                return (Number(val) >= 0) && (Number(val) <= Number(someOtherVal()));
                            },
                            message: 'Must be greater than 0 and less than or equal to payment amount.',
                            params: self.amount
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
                    self.vendorOnHold = ko.observable(false);
                    self.vendorName = ko.observable('').extend({ required: true });
                    self.vendorNumber = ko.observable('').extend({ required: true, vendorIsOnHold: self.vendorOnHold });

                    self.backchargeVendorOnHold = ko.observable(false);
                    self.backchargeVendorName = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true); }
                        }
                    });
                    self.backchargeVendorNumber = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true); }
                        }, vendorIsOnHold: self.backchargeVendorOnHold
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
                        var vendorNumber = $('#vendor-search').attr('data-vendor-number');
                        var vendorName = $('#vendor-search').attr('data-vendor-name');
                        var vendorOnHold = $('#vendor-search').attr('data-vendor-on-hold');
                        self.vendorOnHold(vendorOnHold);
                        self.vendorNumber(vendorNumber);
                        self.vendorName(vendorName);
                    });
                    
                    $(document).on('backcharge-vendor-number-selected', function () {
                        var vendorNumber = $('#backcharge-vendor-search').attr('data-vendor-number');
                        var vendorName = $('#backcharge-vendor-search').attr('data-vendor-name');
                        var vendorOnHold = $('#backcharge-vendor-search').attr('data-vendor-on-hold');
                        self.backchargeVendorOnHold(vendorOnHold);
                        self.backchargeVendorNumber(vendorNumber);
                        self.backchargeVendorName(vendorName);
                    });

                    function formHasErrors(theModel) {
                        debugger;
                        var errors = ko.validation.group(theModel);

                        if (errors().length != 0) {
                            viewModel.errors.showAllMessages(false);
                            errors.showAllMessages();
                            return true;
                        }

                        return false;
                    }

                    self.addPayment = function () {
                        
                        if (formHasErrors([self.invoiceNumber, self.amount, self.selectedCostCode, self.backchargeAmount, self.backchargeReason, self.personNotified, self.personNotifiedPhoneNumber, self.personNotifiedDate, self.backchargeResponseFromVendor, self.vendorNumber, self.backchargeVendorName, self.backchargeVendorNumber]))
                            return;

                        self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                        self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;

                        var newPayment = new PaymentViewModel({
                            serviceCallLineItemId: self.serviceCallLineItemId,
                            vendorNumber: self.vendorNumber(),
                            vendorName: self.vendorName(),
                            backchargeVendorNumber: self.backchargeVendorNumber(),
                            backchargeVendorName: self.backchargeVendorName(),
                            invoiceNumber: self.invoiceNumber(),
                            amount: self.amount(),
                            backchargeAmount: self.backchargeAmount(),
                            isBackcharge: self.isBackcharge(),
                            backchargeReason: self.backchargeReason(),
                            personNotified: self.personNotified(),
                            personNotifiedPhoneNumber: self.personNotifiedPhoneNumber(),
                            personNotifiedDate: self.personNotifiedDate(),
                            backchargeResponseFromVendor: self.backchargeResponseFromVendor(),
                            paymentStatusDisplayName: paymentStatusEnum.Requested.DisplayName,
                            backchargeStatusDisplayName: backchargeStatusEnum.Requested.DisplayName,
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
                        self.completeButtonClicked(true);

                        if (formHasErrors([self.rootCauseId, self.rootProblemId])) {
                            //self.completeButtonClicked(false);
                            return;
                        }

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
                            self.serviceCallLineItemStatusDisplayName(modelData.initialServiceCallLineItem.serviceCallLineItemStatus.displayName); //TODO: displayName works for model passed into js file via toJSON().
                        if (modelData.initialServiceCallLineItem.serviceCallLineItemStatus.DisplayName)
                            self.serviceCallLineItemStatusDisplayName(modelData.initialServiceCallLineItem.serviceCallLineItemStatus.DisplayName); //TODO: DisplayName works for model passed from ajax call. Need to keep both similar.
                    }

                    self.lineItemStatusCSS = ko.computed(function () {
                        return self.serviceCallLineItemStatusDisplayName() ? 'label label-' + self.serviceCallLineItemStatusDisplayName().toLowerCase() + '-service-line-item' : '';
                    });

                    self.isLineItemCompleted = function () {
                        if (!self.serviceCallLineItemStatusDisplayName())
                            return false;

                        return self.serviceCallLineItemStatusDisplayName().toLowerCase() == serviceCallLineItemStatusData.Complete.DisplayName.toLowerCase() ? true : false;
                    };

                    self.theLookups = dropdownData.availableLookups; //dropdown list does not need to be observable. Only the actual elements w/i the array do.
                    self.allCallNotes = ko.observableArray([]);
                    self.allAttachments = ko.observableArray([]);
                    self.allPurchaseOrders = ko.observableArray([]);
                    self.noteDescriptionToAdd = ko.observable('').extend({required: true});
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
                        var errors = ko.validation.group([self.noteDescriptionToAdd]);

                        if (errors().length != 0) {
                            errors.showAllMessages();
                            return;
                        }
                        
                        self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                        self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;
                        self.note = $("#addCallNoteDescription").val();

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

                    $.ajax({
                        url: urls.ConstructionVendor.ConstructionVendors + '?jobNumber=' + self.jobNumber() + '&costCode=' + self.costCode(),
                        type: "GET",
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    }).done(function (response) {
                        self.constructionVendors(response);
                        self.constructionVendorsLoading(false);
                    });
                    
                    self.createPurchaseOrder = function () {
                        window.location.href = urls.ServiceCall.CreatePurchaseOrder + '/' + self.serviceCallLineItemId;
                    };
                }
                
                ko.validation.init({
                    errorElementClass: 'has-error',
                    errorMessageClass: 'help-block',
                    decorateElement: true
                });

                ko.validation.rules["vendorIsOnHold"] = {
                    validator: function (val, condition) {
                        if (condition() === 'true' || condition() === true) {
                            return false;
                        }

                        return true;
                    },
                    message: 'Vendor on hold.'
                };

                ko.validation.registerExtenders();

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

                var persistedAllPurchaseOrdersViewModel = modelData.initialServiceCallLinePurchaseOrders;

                _(persistedAllPurchaseOrdersViewModel).each(function(purchaseOrder) {
                    viewModel.allPurchaseOrders.push(new PurchaseOrderViewModel(purchaseOrder));
                });

            });
        });
    });
});