define(['knockout', 'jquery', 'moment', 'datepicker'], function (ko, $, moment) {

    function numberWithCommas(x) {
        var parts = x.toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    }

    //custom binding for currency formatting (wrapper to text binding)
    ko.bindingHandlers.currency = {
        init: function (element, valueAccessor) {
            if ($(element).is("input")) {
                $(element).blur(function () {
                    var value = valueAccessor();
                    value($(this).val().replace('$', '').replace(',', ''));
                });
            }
        },
        update: function (element, valueAccessor) {
            //unwrap the amount (could be observable or not)
            var amount = parseFloat(ko.utils.unwrapObservable(valueAccessor())) || 0;

            //could set the innerText/textContent directly or use $.text(), but we will just let the real text binding handle it by passing it our formatted text
            var newValueAccessor = function () {
                return "$" + numberWithCommas(amount.toFixed(2));
            };

            //call real text binding and set the underlying value with the numeric amount
            ko.bindingHandlers.text.update(element, newValueAccessor);
            $(element).val && $(element).val(amount === 0 ? '' : amount);
        }
    };

    ko.bindingHandlers.executeOnEnter = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var allBindings = allBindingsAccessor();
            $(element).keypress(function (event) {
                var keyCode = (event.which ? event.which : event.keyCode);
                if (keyCode === 13) {
                    allBindings.executeOnEnter.call(viewModel);
                    return false;
                }
                return true;
            });
        }
    };

    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            //initialize datepicker with some optional options
            var options = allBindingsAccessor().datepickerOptions || {format: 'm/d/yyyy'};
            $(element).datepicker(options);

            //when a user changes the date, update the view model
            ko.utils.registerEventHandler(element, "changeDate", function (event) {
                var value = valueAccessor();
                if (ko.isObservable(value) && event.date) {
                    value(event.date.toLocaleDateString());
                }
            });
        },
        update: function (element, valueAccessor) {
            var widget = $(element).data("datepicker");
            //when the view model is updated, update the widget
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) {
                var dt = new Date(value);
                if (widget && !isNaN(dt.getMonth())) {
                    widget.date = dt;
                    widget.setValue(dt);
                }
                widget.hide();
            }		
        },
    };

    ko.bindingHandlers.phoneNumber = {
        init: function (element, valueAccessor) {
            // Use the masked input plugin from: http://digitalbush.com/projects/masked-input-plugin/
            $(element).mask('(999) 999-9999? x9999'); // phone number with an optional extension.
        
            $(element).blur(function () {
                var value = valueAccessor();
                value($(this).val());
            });

            $(element).focus();
        },
        update: function(element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            $(element).val(value);
        }
    };

    ko.bindingHandlers.date = {
        update: function(element, valueAccessor) {
            var value = valueAccessor();
            if (value != null) {
                var newValueAccessor = function () {
                    return convertDateToString(value);
                };

                ko.bindingHandlers.text.update(element, newValueAccessor);
            }
        }
    };
    
    ko.bindingHandlers.datetime = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var jsonDate = valueAccessor();
            var value = new Date(parseInt(jsonDate.substr(6)));
            var ret = value.getMonth() + 1 + "/" + value.getDate() + "/" + value.getFullYear();
            element.innerHTML = ret;
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        }
    };

    ko.bindingHandlers.liveEditor = {
        init: function (element, valueAccessor) {
            var observable = valueAccessor();
            observable.extend({ liveEditor: this });
        },
        update: function (element, valueAccessor) {
            var observable = valueAccessor();
            ko.bindingHandlers.css.update(element, function () {
                return { editing: observable.editing };
            });
        }
    };

    ko.extenders.liveEditor = function (target) {
        target.editing = ko.observable(false);

        target.edit = function () {
            target.editing(true);
        };

        target.stopEditing = function () {
            target.editing(false);
        };

        return target;
    };

    function convertDateToString(dateToConvert) {
        var jsDate = new Date(Date.parse(dateToConvert));
        return moment(jsDate).format('L');
    }

    ko.bindingHandlers.tooltip = {
        init: function(element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                options = {};

            ko.utils.extend(options, ko.bindingHandlers.tooltip.options);
            ko.utils.extend(options, value);

            $(element).tooltip(options);
        },
        options: {
            placement: 'bottom',
            trigger: 'hover'
        }
    };

    ko.bindingHandlers.stopBinding = {
        init: function () {
            return { controlsDescendantBindings: true };
        }
    };

    return ko;
});
