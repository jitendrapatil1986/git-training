require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'moment', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/DeliveryInstruction', 'jquery.maskedinput', 'bootbox', 'app/serviceCall/SearchVendor', 'maxlength'], function ($, ko, koxeditable, moment, urls, toastr, modelData, dropdownData, xeditable, deliveryInstructionEnum, maskedInput, bootbox) {
        window.ko = ko; //manually set the global ko property.

        require(['ko.validation', 'jquery.color'], function () {
            $(function () {
                var nowTemp = new Date();
                var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);

                $(".datepicker").datepicker({
                    onRender: function (date) {
                        return date.valueOf() < now.valueOf() ? 'disabled' : '';
                    }
                });

                $('body').on('focus', '.max-length', function () {
                    $(this).maxlength({
                        alwaysShow: true,
                        separator: ' out of ',
                        postText: ' characters entered',
                    });
                });

                function PurchaseOrderLineViewModel() {
                    var self = this;

                    self.lineNumber = ko.observable();
                    self.quantity = ko.observable();
                    self.unitCost = ko.observable();
                    self.description = ko.observable();
                    self.maxPurchaseOrderLineItemDescriptionLength = modelData.maxPurchaseOrderLineItemDescriptionLength;

                    self.quantity.extend({
                        required: {
                            onlyIf: function () { return (self.unitCost() || self.description()); }
                        }
                    });

                    self.unitCost.extend({
                        required: {
                            onlyIf: function () { return (self.quantity() || self.description()); }
                        }
                    });

                    self.description.extend({
                        required: {
                            onlyIf: function () { return (self.quantity() || self.unitCost()); }
                        },
                        maxLength: modelData.maxPurchaseOrderLineItemDescriptionLength
                    });

                    self.subTotal = ko.computed(function () {
                        if (!self.quantity() || !self.unitCost())
                            return 0;

                        return self.quantity() * self.unitCost();
                    });
                }

                function purchaseOrderViewModel() {
                    var self = this;

                    self.serviceCallLineItemId = modelData.initialPurchaseOrder.serviceCallLineItemId;
                    self.vendorOnHold = ko.observable(false);
                    self.vendorName = ko.observable().extend({ required: true });
                    self.vendorNumber = ko.observable().extend({ required: true, vendorIsOnHold: self.vendorOnHold });
                    self.deliveryInstructionCodes = ko.observableArray(modelData.deliveryInstructionCodes);
                    self.selectedDeliveryInstruction = ko.observable().extend({ required: true });
                    self.deliveryDate = ko.observable().extend({ required: true, minDate: now });
                    self.isMaterialObjectAccount = ko.observable('true').extend({ required: true });
                    self.jobNumber = ko.observable(modelData.initialPurchaseOrder.jobNumber);
                    self.address = ko.observable(modelData.initialPurchaseOrder.addressLine);
                    self.city = ko.observable(modelData.initialPurchaseOrder.city);
                    self.stateCode = ko.observable(modelData.initialPurchaseOrder.stateCode);
                    self.postalCode = ko.observable(modelData.initialPurchaseOrder.postalCode);
                    self.purchaseOrderMaxAmount = ko.observable(modelData.initialPurchaseOrder.purchaseOrderMaxAmount);
                    self.maxPurchaseOrderNotesLength = modelData.maxPurchaseOrderNotesLength;

                    self.notes = ko.observable().extend({ maxLength: modelData.maxPurchaseOrderNotesLength });
                    self.line1 = new PurchaseOrderLineViewModel();
                    self.line2 = new PurchaseOrderLineViewModel();
                    self.line3 = new PurchaseOrderLineViewModel();
                    self.line4 = new PurchaseOrderLineViewModel();
                    self.line5 = new PurchaseOrderLineViewModel();
                    self.line1.lineNumber(1);
                    self.line2.lineNumber(2);
                    self.line3.lineNumber(3);
                    self.line4.lineNumber(4);
                    self.line5.lineNumber(5);

                    self.allPurchaseOrderLines = ko.observableArray([]);

                    self.totalCost = ko.computed(function () {
                        var total = 0;
                        total += self.line1.subTotal() + self.line2.subTotal() + self.line3.subTotal() +
                                 self.line4.subTotal() + self.line5.subTotal();

                        ko.utils.arrayForEach(this.allPurchaseOrderLines(), function (lineItem) {
                            total += lineItem.subTotal();
                        });
                        return total;
                    }, this);

                    self.totalCost.extend(
                    {
                        max: { params: self.purchaseOrderMaxAmount() ? self.purchaseOrderMaxAmount().toFixed(2) : 99999999, message: 'Maximum: ${0}' }
                    });

                    self.canSave = ko.computed(function () {
                        if (((self.line1.quantity() !== undefined && self.line1.quantity() != "") && (self.line1.description() !== undefined && self.line1.description() != "") && (self.line1.unitCost() !== undefined && self.line1.unitCost() != "")) ||
                             ((self.line2.quantity() !== undefined && self.line2.quantity() != "") && (self.line2.description() !== undefined && self.line2.description() != "") && (self.line2.unitCost() !== undefined && self.line2.unitCost() != "")) ||
                             ((self.line3.quantity() !== undefined && self.line3.quantity() != "") && (self.line3.description() !== undefined && self.line3.description() != "") && (self.line3.unitCost() !== undefined && self.line3.unitCost() != "")) ||
                             ((self.line4.quantity() !== undefined && self.line4.quantity() != "") && (self.line4.description() !== undefined && self.line4.description() != "") && (self.line4.unitCost() !== undefined && self.line4.unitCost() != "")) ||
                             ((self.line5.quantity() !== undefined && self.line5.quantity() != "") && (self.line5.description() !== undefined && self.line5.description() != "") && (self.line5.unitCost() !== undefined && self.line5.unitCost() != ""))) {
                            return true;
                        }
                        else if (self.allPurchaseOrderLines() !== undefined) {
                            self.checkPurchaseOrderLines = false;
                            ko.utils.arrayForEach(self.allPurchaseOrderLines(), function (lineItem) {
                                if ((lineItem.quantity() !== undefined && lineItem.quantity() != "") && (lineItem.description() !== undefined && lineItem.description() != "") && (lineItem.unitCost() !== undefined && lineItem.unitCost() != "")) {
                                    self.checkPurchaseOrderLines = true;
                                    return true;
                                }
                            });

                            if (self.checkPurchaseOrderLines == true)
                                return true;
                        }
                        else
                        {
                            return false;
                        }
                        
                    });

                    self.addPurchaseOrderLine = function () {
                        self.allPurchaseOrderLines.push(new PurchaseOrderLineViewModel());
                        $('#quantity_' + self.allPurchaseOrderLines()[self.allPurchaseOrderLines().length - 1].lineNumber()).numeric();
                        $('#unitCost_' + self.allPurchaseOrderLines()[self.allPurchaseOrderLines().length - 1].lineNumber()).numeric();
                    };

                    self.submitPurchaseOrder = function () {
                        if (self.errors().length != 0) {
                            self.errors.showAllMessages();
                            return;
                        }

                        var newPurchaseOrder = {
                            ServiceCallLineItemId: self.serviceCallLineItemId,
                            VendorNumber: self.vendorNumber(),
                            VendorName: self.vendorName(),
                            DeliveryInstructions: self.selectedDeliveryInstruction(),
                            DeliveryDate: self.deliveryDate(),
                            PurchaseOrderNote: self.notes(),
                            IsMaterialObjectAccount: self.isMaterialObjectAccount(),
                            ServiceCallLineItemPurchaseOrderLines: []
                        };

                        newPurchaseOrder.ServiceCallLineItemPurchaseOrderLines.push(self.line1);
                        newPurchaseOrder.ServiceCallLineItemPurchaseOrderLines.push(self.line2);
                        newPurchaseOrder.ServiceCallLineItemPurchaseOrderLines.push(self.line3);
                        newPurchaseOrder.ServiceCallLineItemPurchaseOrderLines.push(self.line4);
                        newPurchaseOrder.ServiceCallLineItemPurchaseOrderLines.push(self.line5);

                        ko.utils.arrayForEach(self.allPurchaseOrderLines(), function (lineItem) {
                            newPurchaseOrder.ServiceCallLineItemPurchaseOrderLines.push(lineItem);
                        });

                        var modelData = ko.toJSON(newPurchaseOrder);

                        $.ajax({
                            url: urls.ManageServiceCall.AddPurchaseOrder,
                            type: "POST",
                            data: modelData,
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        })
                            .fail(function (response) {
                                toastr.error("There was a problem creating the purchase order. Please try again.");
                            })
                            .done(function (response) {
                                toastr.success("Success! Purchase order created.");
                                window.location.href = urls.ServiceCall.LineItemDetail + '/' + self.serviceCallLineItemId;
                            });
                    };

                    $(document).on('vendor-number-selected', function () {
                        var vendorNumber = $('#vendor-search').attr('data-vendor-number');
                        var vendorName = $('#vendor-search').attr('data-vendor-name');
                        var vendorOnHold = $('#vendor-search').attr('data-vendor-on-hold');
                        self.vendorOnHold(vendorOnHold);
                        self.vendorNumber(vendorNumber);
                        self.vendorName(vendorName);
                    });

                    $("#deliveryDate").on('changeDate', function (e) {
                        self.deliveryDate(moment(e.date).format("L"));
                    });
                };

                ko.validation.init({
                    errorElementClass: 'has-error',
                    errorMessageClass: 'help-block',
                    decorateElement: true,
                    grouping: { deep: true }
                });

                ko.validation.rules['minDate'] = {
                    validator: function (val, otherVal) {
                        return Date.parse(val) >= Date.parse(otherVal);
                    },
                    message: 'Date must be greater than or equal to {0}.'
                };

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

                var viewModel = new purchaseOrderViewModel();
                viewModel.errors = ko.validation.group(viewModel);
                ko.applyBindings(viewModel);
            });
        });
    });
});