require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'moment', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/PaymentStatus', 'enumeration/BackchargeStatus', 'enumeration/PhoneNumberType', 'enumeration/ActivityType', 'jquery.maskedinput', 'enumeration/ServiceCallStatus', 'enumeration/ServiceCallLineItemStatus', 'enumeration/PurchaseOrderLineItemStatus', 'bootbox', 'app/formUploader', 'app/serviceCall/SearchVendor', 'app/serviceCall/SearchProjectCoordinator', 'app/serviceCall/SearchBackchargeVendor', "maxlength"], function ($, ko, koxeditable, moment, urls, toastr, modelData, dropdownData, xeditable, paymentStatusEnum, backchargeStatusEnum, phoneNumberTypeEnum, activityTypeEnum, maskedInput, serviceCallStatusData, serviceCallLineItemStatusData, purchaseOrderLineItemStatusEnum, bootbox) {
        window.ko = ko; //manually set the global ko property.

        require(['ko.validation', 'jquery.color'], function () {
            $(function () {

                $.fn.editable.defaults.mode = 'inline';
                $.fn.editable.defaults.emptytext = 'Add';
                
                $.fn.editableform.buttons =
                    '<button type="submit" class="btn btn-primary editable-submit btn-sm"><i class="glyphicon glyphicon-ok"></i></button>';
                
                $('body').on('focus', '.max-length', function () {
                    $(this).maxlength({
                        alwaysShow: true,
                        separator: ' out of ',
                        postText: ' characters entered',
                    });
                });

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
                    viewModel.noteDescriptionToAdd('');
                    viewModel.noteDescriptionToAdd.isModified(false);
                }

                function PurchaseOrderViewModel(options) {
                    var self = this;

                    self.serviceCallLineItemId = options.serviceCallLineItemId;
                    self.purchaseOrderNumber = options.purchaseOrderNumber;
                    self.vendorNumber = options.vendorNumber;
                    self.vendorName = options.vendorName;
                    self.costCode = options.costCode;
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
                
                function PaymentAndBackchargeViewModel(options) {
                    var self = this;
                    self.serviceCallLineItemId = options.serviceCallLineItemId;
                    self.vendorNumber = options.vendorNumber;
                    self.vendorName = options.vendorName;
                    self.backchargeVendorNumber = options.backchargeVendorNumber;
                    self.backchargeVendorName = options.backchargeVendorName;
                    self.invoiceNumber = options.invoiceNumber;
                    self.comments = options.comments;
                    self.amount = options.amount;
                    self.isBackcharge = options.isBackcharge;
                    self.backchargeAmount = options.backchargeAmount;
                    self.backchargeId = options.backchargeId;
                    self.backchargeServiceCallLineItemId = options.backchargeServiceCallLineItemId;
                    self.backchargeReason = options.backchargeReason;
                    self.personNotified = options.personNotified;
                    self.personNotifiedPhoneNumber = options.personNotifiedPhoneNumber;
                    self.personNotifiedDate = options.personNotifiedDate;
                    self.backchargeResponseFromVendor = options.backchargeResponseFromVendor;
                    self.paymentId = options.paymentId;
                    self.createdDate = options.paymentCreatedDate ? options.paymentCreatedDate : options.backchargeCreatedDate;
                    self.holdComments = ko.observable(options.holdComments);
                    self.backchargeHoldComments = ko.observable(options.backchargeHoldComments);
                    self.backchargeDenyComments = ko.observable(options.backchargeDenyComments);
                    self.holdDate = ko.observable(options.holdDate);
                    self.backchargeHoldDate = ko.observable(options.backchargeHoldDate);
                    self.backchargeDenyDate = ko.observable(options.backchargeDenyDate);
                    self.paymentStatusDisplayName = ko.observable(options.paymentStatusDisplayName);
                    self.backchargeStatusDisplayName = ko.observable(options.backchargeStatusDisplayName);
                    self.costCode = options.costCode ? options.costCode : options.backchargeCostCode;
                    self.standAloneBackcharge = ko.observable(!options.paymentId);
                    self.notifiedProjectCoordinator = options.notifiedProjectCoordinator;
                    self.projectCoordinatorEmailToNotify = options.projectCoordinatorEmailToNotify;
                    self.sendCheckToProjectCoordinator = options.sendCheckToProjectCoordinator;
                                        

                    self.isHomeownerPayment = ko.computed(function() {
                        if (self.sendCheckToProjectCoordinator)
                            return true;
                        return false;
                    });
                    
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
                        displayPopUp('hold', item, event);
                    };
                    
                    self.displayHoldBackcharge = function (item, event) {
                        displayPopUp('holdBackcharge', item, event, true, true);
                    };
                    
                    self.displayDenyBackcharge = function (item, event) {
                        displayPopUp('denyBackcharge', item, event, true, true);
                    };

                    var displayPopUp = function (name, item, event, forBackcharge, displayToTheRight) {
                        var button = $(event.target);
                        $('.btn-action-with-popup').removeClass('active');
                        button.removeClass('btn-hover-show');
                        button.addClass("active");
                        $('.popup-action-with-message').hide();
                        var right = ($(window).width() - (button.offset().left + button.outerWidth()));
                        var actionwithPopup = name + '-' + (forBackcharge ? item.backchargeId : item.paymentId);
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
                                        viewModel.allPaymentsAndBackcharges.remove(payment);
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

                    self.deleteBackcharge = function () {
                        var backcharge = this;
                        bootbox.confirm("Are you sure you want to delete this backcharge?", function (result) {
                            if (result) {
                                var actionUrl = urls.ManageServiceCall.DeleteStandAloneBackcharge;
                                $.ajax({
                                    type: "DELETE",
                                    url: actionUrl,
                                    data: { BackchargeId: backcharge.backchargeId },
                                    success: function () {
                                        viewModel.allPaymentsAndBackcharges.remove(backcharge);
                                        toastr.success("Success! Backcharge deleted.");
                                    }
                                });
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
                    
                    self.shouldDisplayDeleteBackcharge = ko.computed(function () {
                        return self.backchargeStatusDisplayName() == backchargeStatusEnum.Pending.DisplayName || self.backchargeStatusDisplayName() == backchargeStatusEnum.Hold.DisplayName;
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
                    self.note = ko.observable(options.note).extend({required: true});
                    self.createdBy = options.createdBy;
                    self.createdDate = options.createdDate;
                    self.serviceCallCommentTypeId = options.serviceCallCommentTypeId;
                    self.editingNote = ko.observable(false);
                    self.currentNote = ko.observable(options.note);
                    
                    self.saveNoteChanges = function () {
                        var errors = ko.validation.group(self);

                        if (errors().length != 0) {
                            viewModel.errors.showAllMessages(false);
                            self.errors.showAllMessages();
                            return;
                        }

                        var noteData = ko.toJSON(self);

                        $.ajax({
                            url: urls.ManageServiceCall.EditServiceCallLineItemNote,
                            type: "POST",
                            data: noteData,
                            dataType: "json",
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        })
                            .fail(function (response) {
                                toastr.error("There was an issue updating the note. Please try again!");
                            })
                            .done(function (response) {
                                toastr.success("Success! Note updated.");
                                self.editingNote(false);
                                self.currentNote(self.note());
                            });
                    };

                    self.cancelNoteChanges = function () {
                        this.note(this.currentNote());
                        this.editingNote(false);
                    };

                    self.deleteNote = function (e) {
                        var note = ko.toJSON(e);

                        bootbox.confirm("Are you sure you want to delete this note?", function (result) {
                            if (result) {
                                $.ajax({
                                    url: urls.ManageServiceCall.DeleteServiceCallLineItemNote,
                                    type: "DELETE",
                                    data: note,
                                    dataType: "json",
                                    processData: false,
                                    contentType: "application/json; charset=utf-8"
                                })
                                .fail(function (response) {
                                    toastr.error("There was an issue deleting the note. Please try again!");
                                })
                                .done(function (response) {
                                    viewModel.allCallNotes.remove(e);
                                    toastr.success("Success! Note deleted.");
                                });
                            }
                        });
                    };
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
                            line.hasEverBeenCompleted(true);
                            toastr.success("Success! Item completed.");
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
                
                function noActionServiceCallLineItem(line) {
                    var lineData = ko.toJSON(line);

                    $.ajax({
                        url: urls.ManageServiceCall.NoActionLineItem,
                        type: "POST",
                        data: lineData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function (response) {
                            toastr.error("There was an issue setting the line item to no action. Please try again!");
                        })
                        .done(function (response) {
                            line.serviceCallLineItemStatusDisplayName(response.DisplayName);
                            line.rootProblemId(modelData.noActionRootProblemCode.value);
                            line.rootProblem(modelData.noActionRootProblemCode.displayName);
                            line.rootCauseId(modelData.noActionRootCauseCode.value);
                            line.rootCause(modelData.noActionRootCauseCode.displayName);
                            toastr.success("Success! Item set to no action.");
                        });
                }

                function serviceCallLineItemViewModel() {
                    var self = this;

                    self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                    self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;
                    self.completed = modelData.initialServiceCallLineItem.completed;
                    self.completeButtonClicked = ko.observable(false);

                    self.problemCodeId = ko.observable(modelData.initialServiceCallLineItem.problemCodeId);
                    self.problemCode = ko.observable(modelData.initialServiceCallLineItem.problemCode);
                    self.problemDescription = ko.observable(modelData.initialServiceCallLineItem.problemDescription);
                    self.jobNumber = ko.observable(modelData.initialServiceCallLineItem.jobNumber);
                    self.constructionVendors = modelData.vendors;
                    self.hasEverBeenCompleted = ko.observable(modelData.initialServiceCallLineItem.hasEverBeenCompleted);
                    self.hasAnyPayments = ko.observable(modelData.initialServiceCallLineItem.hasAnyPayments);
                    self.hasAnyPurchaseOrders = ko.observable(modelData.initialServiceCallLineItem.hasAnyPurchaseOrders);                    
                    		                   
                    self.maxPersonNotifiedLength = modelData.maxPersonNotifiedLength;		
                    self.maxInvoiceNumberLength = modelData.maxInvoiceNumberLength;		
                   
                    self.groupedConstructionVendors = ko.computed(function () {
                        var rows = [], current = [];
                        rows.push(current);
                        for (var i = 0; i < self.constructionVendors.length; i += 1) {
                            current.push(self.constructionVendors[i]);
                            if (((i + 1) % 3) === 0) {
                                current = [];
                                rows.push(current);
                            }
                        }
                        return rows;
                    }, this);

                    //Value saved in db is string but ddl needs id to set default value.
                    self.rootCause = ko.observable(modelData.initialServiceCallLineItem.rootCause);
                    self.hasRootCause = ko.computed(function () {
                        return self.rootCause() != null && self.rootCause() != 'Imported';
                    });
                    
                    var selectedRootCause = ko.utils.arrayFirst(modelData.rootCauseCodes, function (item) {
                        return item.text === modelData.initialServiceCallLineItem.rootCause;
                    });
                    self.rootCauseId = ko.observable(selectedRootCause ? selectedRootCause.value : '').extend({
                        required: {
                            onlyIf: function () {
                                return self.completeButtonClicked() === true || self.hasRootCause();
                            }
                        }
                    });
                    
                    self.rootProblem = ko.observable(modelData.initialServiceCallLineItem.rootProblem);
                    self.hasRootProblem = ko.computed(function() {
                        return self.rootProblem() != null && self.rootProblem() != 'Imported';
                    });
                    
                    var selectedRootProblem = ko.utils.arrayFirst(modelData.rootProblemCodes, function (item) {
                        return item.text === modelData.initialServiceCallLineItem.rootProblem;
                    });
                    self.rootProblemId = ko.observable(selectedRootProblem ? selectedRootProblem.value : '').extend({
                        required: {
                            onlyIf: function () {
                                return self.completeButtonClicked() === true || self.hasRootProblem();
                            }
                        }
                    });

                    self.rootCauseCodes = ko.observableArray(modelData.rootCauseCodes);
                    self.rootProblemCodes = ko.observableArray(modelData.rootProblemCodes);

                    self.rootCauseId.subscribe(function(rootCauseId) {
                        var matchedRootCause = ko.utils.arrayFirst(modelData.rootCauseCodes, function (item) {
                            return Number(item.value) === Number(rootCauseId);
                        });

                        if (matchedRootCause) {
                            self.rootCause(matchedRootCause.text);

                            $.ajax({
                                url: urls.ManageServiceCall.EditLineItem,
                                type: "POST",
                                data: ko.toJSON({ serviceCallLineItemId: self.serviceCallLineItemId, rootCause: rootCauseId }),
                                dataType: "json",
                                processData: false,
                                contentType: "application/json; charset=utf-8"
                            }).fail(function() {
                                viewModel.completeButtonClicked(false);
                                toastr.error("There was an error updating the root cause");
                            }).success(function() {
                                viewModel.completeButtonClicked(false);
                                toastr.success("Successfully updated root cause");
                            });
                        }
                    });
                    
                    self.rootProblemId.subscribe(function (rootProblemId) {
                        var matchedRootProblem = ko.utils.arrayFirst(modelData.rootProblemCodes, function (item) {
                            return Number(item.value) === Number(rootProblemId);
                        });

                        if (matchedRootProblem) {
                            self.rootProblem(matchedRootProblem.text);

                            $.ajax({
                                url: urls.ManageServiceCall.EditLineItem,
                                type: "POST",
                                data: ko.toJSON({ serviceCallLineItemId: self.serviceCallLineItemId, rootProblem: rootProblemId }),
                                dataType: "json",
                                processData: false,
                                contentType: "application/json; charset=utf-8"
                            }).fail(function() {
                                viewModel.completeButtonClicked(false);
                                toastr.error("There was an error updating the root problem");
                            }).success(function() {
                                viewModel.completeButtonClicked(false);
                                toastr.success("Successfully updated root problem");
                            });
                        }
                    });

                    self.paymentTypes = ko.observableArray(modelData.paymentTypes);
                    self.paymentTypeId = ko.observable();
                    self.isStandAloneBackcharge = ko.computed(function () {
                        return self.paymentTypeId() === modelData.standAloneBackchargePaymentType.value;
                    });
                    self.isPayment = ko.computed(function () {
                        return self.paymentTypeId() === modelData.paymentPaymentType.value;
                    });
                    
                    self.invoiceNumber = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return self.isPayment() === true; }
                        }
                    });
                    self.comments = ko.observable('');
                    self.amount = ko.observable().extend({
                        required: {
                            onlyIf: function () { return self.isPayment() === true; }
                        }, min: 0
                    });
                    self.isBackcharge = ko.observable(false);
                    self.backchargeAmount = ko.observable().extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        },
                        validation: {
                            validator: function (val) {
                                if (self.isBackcharge() === false || self.isStandAloneBackcharge() === false)
                                    return true;
                                
                                return Number(val) > 0;
                            },
                            message: 'Must be greater than 0',
                        }
                    });
                    self.backchargeReason = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        }
                    });
                    self.personNotified = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        }
                    });
                    self.personNotifiedPhoneNumber = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        }
                    });
                    self.personNotifiedDate = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        }
                    });
                    self.backchargeResponseFromVendor = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        }
                    });
                    self.vendorOnHold = ko.observable(false);
                    self.vendorName = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return self.isPayment() === true && !self.payHomeownerSelected(); }
                        }
                    });
                    self.vendorNumber = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return self.isPayment() === true && !self.payHomeownerSelected(); }
                        },
                        vendorIsOnHold: self.vendorOnHold
                    });

                    self.backchargeVendorOnHold = ko.observable(false);
                    self.backchargeVendorName = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        }
                    });
                    self.backchargeVendorNumber = ko.observable('').extend({
                        required: {
                            onlyIf: function () { return (self.isBackcharge() === true || self.isStandAloneBackcharge() === true); }
                        }
                    });
                    
                    self.allPaymentsAndBackcharges = ko.observableArray([]);

                    self.canAddPayment = ko.computed(function () {
                        return true;
                    });

                    self.clearPaymentFields = function () {
                        $('#vendor-search').val('');
                        $('#backcharge-vendor-search').val('');
                        $('#pc-search').val('');
                        self.vendorNumber('');
                        self.backchargeVendorNumber('');
                        self.invoiceNumber('');
                        self.comments('');
                        self.amount('');
                        self.payHomeownerSelected(false);
                        self.projectCoordinatorEmail('');
                        self.sendCheckToPC(false);
                        self.backchargeAmount('');
                        self.isBackcharge(false);
                        self.backchargeReason('');
                        self.personNotified('');
                        self.personNotifiedPhoneNumber('');
                        self.personNotifiedDate('');
                        self.backchargeResponseFromVendor('');
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

                    self.canPayHomeowner = ko.observable(false);
                    self.cannotPayHomeowner = ko.computed(function() {
                        return !self.canPayHomeowner();
                    });
                    self.cannotPayHomeownerHelpText = ko.computed(function() {
                        return self.canPayHomeowner() ? "" : "Cannot find homeowner in supply master; please contact your PC.";
                    });
                    
                    $.ajax({
                        url: urls.ManageServiceCall.GetHomeownerId,
                        type: "GET",
                        data: { jobNumber: self.jobNumber() },
                        contentType: "application/json; charset=utf-8"
                    })
                    .fail(function (response) {
                        console.error(response);
                        toastr.error("Failed to validate whether homeowner is payable");
                    })
                    .done(function (response) {
                        if (response.IsValid) {
                            self.canPayHomeowner(true);
                            self.homeownerName(response.HomeownerName);
                            self.homeownerId(response.HomeownerNumber);
                        }
                    });
                    self.homeownerId = ko.observable();
                    self.homeownerName = ko.observable();
                    self.payHomeownerSelected = ko.observable(false);
                    self.payHomeownerSelected.subscribe(function (newValue) {
                        if (newValue) {
                            self.vendorOnHold(false);
                            self.vendorNumber(self.homeownerId());
                            self.vendorName(self.homeownerName());
                            $('#vendor-search').val(self.homeownerName());
                            $("#invoiceNumber").focus();
                        } else {
                            self.vendorOnHold(false);
                            self.vendorNumber('');
                            self.vendorName('');
                            $('#vendor-search').val('');
                            $("#vendor-search").focus();
                        }
                    });

                    self.sendCheckToPC = ko.observable();
                    self.notifiedProjectCoordinator = ko.observable();
                    self.projectCoordinatorEmail = ko.observable().extend({
                        required: {
                            onlyIf: function () { return (self.payHomeownerSelected() === true); }
                        }
                    });;
                    $(document).on('pc-selected', function () {
                        var projectCoordinatorEmail = $('#pc-search').attr('data-pc-email');
                        var notifiedProjectCoordinator = $('#pc-search').attr('data-pc-name');
                        self.projectCoordinatorEmail(projectCoordinatorEmail);
                        self.notifiedProjectCoordinator(notifiedProjectCoordinator);
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
                        var errors = ko.validation.group(theModel);

                        if (errors().length != 0) {
                            viewModel.errors.showAllMessages(false);
                            errors.showAllMessages();
                            return true;
                        }

                        return false;
                    }

                    self.addPayment = function () {
                        
                        if (formHasErrors([self.invoiceNumber, self.amount, self.backchargeAmount, self.backchargeReason, self.personNotified, self.personNotifiedPhoneNumber, self.personNotifiedDate, self.backchargeResponseFromVendor, self.vendorNumber, self.backchargeVendorName, self.backchargeVendorNumber, self.projectCoordinatorEmail]))
                            return;

                        self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                        self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;

                        var newPayment = new PaymentAndBackchargeViewModel({
                            serviceCallLineItemId: self.serviceCallLineItemId,
                            vendorNumber: self.vendorNumber(),
                            vendorName: self.vendorName(),
                            backchargeVendorNumber: self.backchargeVendorNumber(),
                            backchargeVendorName: self.backchargeVendorName(),
                            invoiceNumber: self.invoiceNumber(),
                            comments: self.comments(),
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
                            notifiedProjectCoordinator: self.notifiedProjectCoordinator(),
                            projectCoordinatorEmailToNotify: self.projectCoordinatorEmail(),
                            sendCheckToProjectCoordinator: self.sendCheckToPC(),                           	
                            maxInvoiceNumberLength: self.maxInvoiceNumberLength,
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
                                if (response.responseJSON.ExceptionMessage === "EMAIL_SEND_FAILURE")
                                    toastr.error("Failed to send email to Project Coordinator");
                                else
                                    toastr.error("There was a problem adding the payment. Please try again.");
                            })
                            .done(function (response) {
                                newPayment.paymentId = response.PaymentId;
                                newPayment.costCode = { costCode: response.CostCode.CostCode, displayName: response.CostCode.DisplayName };
                                newPayment.standAloneBackcharge(false);
                                
                                if (response.BackchargeId) {
                                    newPayment.BackchargeId = response.BackchargeId;
                                }
                                
                                self.allPaymentsAndBackcharges.unshift(newPayment);
                                toastr.success("Success! Payment added.");
                                highlight($("#allServiceCallPaymentsAndBackcharges").first());
                                self.hasAnyPayments(true);
                                self.clearPaymentFields();
                            });
                    };

                    self.addStandAloneBackcharge = function () {

                        if (formHasErrors([self.backchargeAmount, self.backchargeReason, self.personNotified, self.personNotifiedPhoneNumber, self.personNotifiedDate, self.backchargeResponseFromVendor, self.backchargeVendorName, self.backchargeVendorNumber]))
                            return;

                        self.serviceCallId = modelData.initialServiceCallLineItem.serviceCallId;
                        self.serviceCallLineItemId = modelData.initialServiceCallLineItem.serviceCallLineItemId;

                        var newStandAloneBackcharge = new PaymentAndBackchargeViewModel({
                            serviceCallLineItemId: self.serviceCallLineItemId,
                            backchargeVendorNumber: self.backchargeVendorNumber(),
                            backchargeVendorName: self.backchargeVendorName(),
                            backchargeAmount: self.backchargeAmount(),
                            backchargeReason: self.backchargeReason(),
                            personNotified: self.personNotified(),
                            personNotifiedPhoneNumber: self.personNotifiedPhoneNumber(),
                            personNotifiedDate: self.personNotifiedDate(),
                            backchargeResponseFromVendor: self.backchargeResponseFromVendor(),
                            backchargeStatusDisplayName: backchargeStatusEnum.Requested.DisplayName,
                            maxPersonNotifiedLength: self.maxPersonNotifiedLength,		                        
                            
                        });

                        var standAloneBackchargeData = ko.toJSON(newStandAloneBackcharge);
                        
                        $.ajax({
                            url: urls.ManageServiceCall.AddStandAloneBackcharge,
                            type: "POST",
                            data: standAloneBackchargeData,
                            dataType: "json",
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        })
                            .fail(function (response) {
                                toastr.error("There was a problem adding the stand alone backcharge. Please try again.");
                            })
                            .done(function (response) {
                                newStandAloneBackcharge.backchargeId = response.BackchargeId;
                                newStandAloneBackcharge.costCode = { costCode: response.CostCode.CostCode, displayName: response.CostCode.DisplayName };
                                newStandAloneBackcharge.standAloneBackcharge(true);
                                self.allPaymentsAndBackcharges.unshift(newStandAloneBackcharge);
                                toastr.success("Success! Stand alone backcharge added.");
                                highlight($("#allServiceCallPaymentsAndBackcharges").first());
                                self.hasAnyPayments(true);
                                self.clearPaymentFields();
                            });
                    };
                    
                    $("#Person_Notified_Date").datepicker({
                        format: 'm/d/yyyy'
                    });

                    $("#Person_Notified_Date").on('changeDate', function (e) {
                        self.personNotifiedDate(moment(e.date).format("L"));
                    });

                    self.completeLine = function () {
                        self.completeButtonClicked(true);

                        if (formHasErrors([self.rootCauseId, self.rootProblemId])) {
                            return;
                        }

                        completeServiceCallLineItem(this);
                    };

                    self.reopenLine = function () {
                        reopenServiceCallLineItem(this);
                    };

                    self.noActionForLine = function () {
                        noActionServiceCallLineItem(this);
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
                        var displayName = self.serviceCallLineItemStatusDisplayName().toLowerCase();
                        displayName = displayName.replace(' ', '-');
                        
                        return self.serviceCallLineItemStatusDisplayName() ? 'label label-' + displayName + '-service-line-item' : '';
                    });

                    self.isLineItemCompleted = function () {
                        if (!self.serviceCallLineItemStatusDisplayName())
                            return false;

                        return self.serviceCallLineItemStatusDisplayName().toLowerCase() == serviceCallLineItemStatusData.Complete.DisplayName.toLowerCase() ? true : false;
                    };

                    self.isLineItemNoAction = function () {
                        if (!self.serviceCallLineItemStatusDisplayName())
                            return false;

                        return self.serviceCallLineItemStatusDisplayName().toLowerCase() == serviceCallLineItemStatusData.NoAction.DisplayName.toLowerCase() ? true : false;
                    };
                    
                    self.theLookups = dropdownData.availableLookups; //dropdown list does not need to be observable. Only the actual elements w/i the array do.
                    self.allCallNotes = ko.observableArray([]);
                    self.allAttachments = ko.observableArray([]);
                    self.allPurchaseOrders = ko.observableArray([]);
                    self.noteDescriptionToAdd = ko.observable('').extend({ required: true });

                    self.canRecodeImportedData = ko.computed(function () {
                        return self.rootProblem() == 'Imported' && self.isLineItemCompleted();
                    });

                    self.recodeEnabled = ko.observable(false);
                    self.enableRecode = function () {
                        self.recodeEnabled(true);
                    };

                    self.enableRootProblem = ko.computed(function () {
                        // Enable root problem if it was imported and there are no payments
                        if (self.recodeEnabled())
                            return true;

                        return !(self.hasAnyPayments() || self.hasAnyPurchaseOrders() || self.isLineItemNoAction());
                    });

                    self.enableRootCause = ko.computed(function () {
                        return !(self.hasAnyPayments() || self.hasAnyPurchaseOrders() || self.isLineItemNoAction()) || !self.hasRootCause();
                    });
                    
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

                    self.createPurchaseOrderClicked = ko.observable().extend({
                        required: {
                            onlyIf: function () {
                                return (self.hasRootProblem() === false);
                            },
                            message: 'To create a PO, select a Root Problem'
                        }
                    });
                    
                    self.createPurchaseOrder = function () {
                        if(formHasErrors([self.createPurchaseOrderClicked]))
                            return;
                        
                        window.location.href = urls.ServiceCall.CreatePurchaseOrder + '/' + self.serviceCallLineItemId;
                    };
                    
                    self.expandPaymentClicked = ko.observable().extend({
                        required: {
                            onlyIf: function () {
                                return (self.hasRootProblem() === false);
                            },
                            message: 'To add a payment, select a Root Problem'
                        }
                    });
                    
                    self.expandPayment = function (e) {
                        formHasErrors([self.expandPaymentClicked]);
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

                var persistedAllPaymentsAndBackchargesViewModel = modelData.initialServiceCallLinePayments;

                _(persistedAllPaymentsAndBackchargesViewModel).each(function (payment) {
                    viewModel.allPaymentsAndBackcharges.push(new PaymentAndBackchargeViewModel(payment));
                });

                var persistedAllStandAloneBackchargesViewModel = modelData.initialServiceCallLineStandAloneBackcharges;

                _(persistedAllStandAloneBackchargesViewModel).each(function (backcharge) {
                    viewModel.allPaymentsAndBackcharges.push(new PaymentAndBackchargeViewModel(backcharge));
                });

                var persistedAllPurchaseOrdersViewModel = modelData.initialServiceCallLinePurchaseOrders;

                _(persistedAllPurchaseOrdersViewModel).each(function(purchaseOrder) {
                    viewModel.allPurchaseOrders.push(new PurchaseOrderViewModel(purchaseOrder));
                });

            });
        });
    });
});