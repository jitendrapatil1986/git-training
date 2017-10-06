
require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'x-editable', 'ko', 'ko.x-editable', 'toastr', 'urls', 'modelData', 'enumeration/PhoneNumberType', 'enumeration/PaymentStatus', 'enumeration/BackchargeStatus', 'bootbox', 'jquery.maskedinput', 'app/formUploader', 'app/additionalContacts', 'app/approveServiceCalls'], function ($, xeditable, ko, koxeditable, toastr, urls, modelData, phoneNumberTypeEnum, paymentStatusEnum, backchargeStatusEnum, bootbox) {
        window.ko = ko; //manually set the global ko property.
        require(['ko.validation', 'jquery.color'], function () {

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

                $(".phone-number-with-extension").on('shown', function () {
                    $(this).data('editable').input.$input.mask('(999) 999-9999? x99999', { placeholder: " " });
                });
             
                $(".map").on('click', function () {
                    
                    var add = $(this).text();
                    window.open("https://maps.google.com?daddr='"+add+"'");                  
                });

                $(".attached-file-display-name").editable();          
               
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
                    $("#addJobNoteDescription").val('');
                    viewModel.jobNoteDescriptionToAdd('');
                    viewModel.jobNoteDescriptionToAdd.isModified(false);
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

                function jobSummaryViewModel() {
                    var self = this;
                    self.jobId = ko.observable($("#jobId").val());
                    self.allJobNotes = ko.observableArray([]);
                    self.jobNoteDescriptionToAdd = ko.observable('').extend({ required: true });
                    self.vendors = ko.observableArray([]);
                    self.selectedCostCode = ko.observable();
                    self.costCodes = modelData.costCodes;

                    self.filteredVendors = ko.computed(function () {
                        var costCode = self.selectedCostCode();
                        if (!costCode || costCode == "") {
                            return self.vendors();
                        }
                        else {

                            return ko.utils.arrayFilter(self.vendors(), function (vendor) {
                                var isMatch = false;
                                var first = ko.utils.arrayFirst(vendor.costCodes, function (vendorCostCode) {
                                    return vendorCostCode.costCode == costCode.costCode;
                                });
                                if (first) {
                                    isMatch = true;
                                }

                                return isMatch;
                            });
                        }
                    });

                    self.groupedVendors = ko.computed(function () {
                        var rows = [], current = [];
                        rows.push(current);
                        for (var i = 0; i < self.filteredVendors().length; i += 1) {
                            current.push(self.filteredVendors()[i]);
                            if (((i + 1) % 3) === 0) {
                                current = [];
                                rows.push(current);
                            }
                        }
                        return rows;
                    }, this);

                    self.addJobNote = function () {
                        if (formHasErrors([self.jobNoteDescriptionToAdd])) {
                            return;
                        }

                        var newJobNote = new JobNotesViewModel(
                            {
                                jobId: self.jobId,
                                note: self.jobNoteDescriptionToAdd,
                            });

                        var lineNoteData = ko.toJSON(newJobNote);

                        $.ajax({
                            url: urls.ManageJob.AddNote,
                            type: "POST",
                            data: lineNoteData,
                            dataType: "json",
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        })
                        .fail(function (response) {
                            toastr.error("There was an issue adding a note. Please try again!");
                        })
                        .done(function (response) {
                            self.allJobNotes.unshift(new JobNotesViewModel({
                                jobNoteId: response.JobNoteId,
                                jobId: response.JobId,
                                note: response.Note,
                                createdBy: response.CreatedBy,
                                createdDate: response.CreatedDate
                            }));

                            toastr.success("Success! Note added.");
                            highlight($("#allJobNotes").first());
                            clearNoteFields();
                        });
                    };

                    self.cancelJobNote = function () {
                        clearNoteFields();
                    };

                    self.allJobAttachments = ko.observableArray([]);

                    self.removeAttachment = function (e) {
                        bootbox.confirm(modelData.attachmentRemovalMessage, function (result) {
                            if (result) {
                                var element = $('.boxclose[data-attachment-id="' + e.jobAttachmentId + '"]');
                                var actionUrl = element.data('url');
                                var attachmentId = e.jobAttachmentId;
                                $.ajax({
                                    type: "POST",
                                    url: actionUrl,
                                    data: { id: attachmentId }
                                })
                                    .fail(function (response) {
                                        alert(JSON.stringify(response));
                                        toastr.error("There was an issue deleting the attachment. Please try again!");
                                    })
                                    .success(function (response) {
                                        self.allJobAttachments.remove(e);
                                        toastr.success("Success! Attachment deleted.");
                                    });
                            }
                        });
                    };

                    self.allJobPayments = ko.observableArray([]);

                    $("#creteServiceCall").click(function () {                        
                        $("#creteServiceCall").attr("disabled", true);
                        $("#frmCreteServiceCall").submit();
                    });
                }

                function JobNotesViewModel(option) {
                    var self = this;

                    self.jobNoteId = option.jobNoteId;
                    self.jobId = option.jobId;
                    self.note = option.note;
                    self.createdBy = option.createdBy;
                    self.createdDate = option.createdDate;
                };

                function VendorViewModel(option) {
                    var self = this;
                    self.name = option.name;
                    self.vendorId = option.vendorId;
                    self.number = option.number;
                    self.contactInfo = option.contactInfo;
                    self.costCodes = option.costCodes;
                    self.costCodesSeparatedByComma = option.costCodesSeparatedByComma;
                };

                function JobAttachmentViewModel(option) {
                    var self = this;

                    self.jobAttachmentId = option.jobAttachmentId;
                    self.jobId = option.jobId;
                    self.filePath = option.filePath;
                    self.displayName = ko.observable(option.displayName);
                    self.isDeleted = option.isDeleted;
                    self.createdBy = option.createdBy;
                    self.createdDate = option.createdDate;
                };

                function JobPaymentViewModel(options) {
                    var self = this;

                    self.vendorName = options.vendorName;
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
                    self.backchargeHoldComments = ko.observable(options.backchargeHoldComments);
                    self.backchargeDenyComments = ko.observable(options.backchargeDenyComments);
                    self.holdDate = ko.observable(options.holdDate);
                    self.backchargeHoldDate = ko.observable(options.backchargeHoldDate);
                    self.backchargeDenyDate = ko.observable(options.backchargeDenyDate);
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
                }

                ko.validation.init({
                    errorElementClass: 'has-error',
                    errorMessageClass: 'help-block',
                    decorateElement: true,
                });

                var viewModel = new jobSummaryViewModel();

                var persistedVendors = modelData.vendors;

                _(persistedVendors).each(function (vendor) {
                    viewModel.vendors.push(new VendorViewModel(vendor));
                });

                var persistedAllJobNotesViewModel = modelData.initialJobNotes;

                _(persistedAllJobNotesViewModel).each(function (note) {
                    viewModel.allJobNotes.push(new JobNotesViewModel(note));
                });

                var persistedAllJobAttachmentsViewModel = modelData.initialJobAttachments;

                _(persistedAllJobAttachmentsViewModel).each(function (attachment) {
                    viewModel.allJobAttachments.push(new JobAttachmentViewModel(attachment));
                });

                var persistedAllJobPaymentsViewModel = modelData.initialJobPayments;

                _(persistedAllJobPaymentsViewModel).each(function (payment) {
                    viewModel.allJobPayments.push(new JobPaymentViewModel(payment));
                });

                viewModel.errors = ko.validation.group(viewModel);

                ko.applyBindings(viewModel);
            });
        });
    });
});