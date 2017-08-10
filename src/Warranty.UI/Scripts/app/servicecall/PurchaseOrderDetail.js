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

                function purchaseOrderViewModel() {
                    var self = this;

                    self.serviceCallLineItemId = modelData.initialPurchaseOrder.serviceCallLineItemId;
                    self.vendorName = ko.observable(modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.vendorName);
                    self.deliveryInstructionCodes = ko.observableArray(modelData.deliveryInstructionCodes);
                    self.selectedDeliveryInstruction = ko.observable(modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.vendorName);
                    self.selectedDeliveryInstruction = ko.observable(modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.deliveryInstructions);
                    self.deliveryDate = ko.observable(moment(modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.createdDate).format("L"));
                    self.isMaterialObjectAccount = ko.computed(function () {
                        if (modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.isMaterialObjectAccount)
                            return 'true';
                        else
                            return 'false';
                    });
                    self.jobNumber = ko.observable(modelData.initialPurchaseOrder.jobNumber);
                    self.address = ko.observable(modelData.initialPurchaseOrder.addressLine);
                    self.city = ko.observable(modelData.initialPurchaseOrder.city);
                    self.stateCode = ko.observable(modelData.initialPurchaseOrder.stateCode);
                    self.postalCode = ko.observable(modelData.initialPurchaseOrder.postalCode);
                    self.notes = ko.observable(modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.purchaseOrderNote);
                    self.totalCost = modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.totalCost;
                    self.allPurchaseOrderLineItems = ko.observableArray([]);
                };

                function PurchaseOrderLineViewModel(option) {
                    var self = this;

                    self.lineNumber = option.lineNumber;
                    self.quantity = option.quantity;
                    self.unitCost = option.unitCost;
                    self.description = option.description !== null ? option.description : "";
                    self.subTotal = ko.computed(function () {
                        if (!self.quantity || !self.unitCost)
                            return 0;

                        return self.quantity * self.unitCost;
                    });
                }
               

                var viewModel = new purchaseOrderViewModel();

                var persistedAllPurchaseOrderLineViewModel = modelData.initialPurchaseOrder.serviceCallLineItemPurchaseOrders.serviceCallLineItemPurchaseOrderLines;

                _(persistedAllPurchaseOrderLineViewModel).each(function (PurchaseOrderLine) {
                    viewModel.allPurchaseOrderLineItems.push(new PurchaseOrderLineViewModel(PurchaseOrderLine));
                });

                viewModel.errors = ko.validation.group(viewModel);
                ko.applyBindings(viewModel);
            });
        });
    });
});