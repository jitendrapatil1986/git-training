require(['/Scripts/app/main.js'], function() {
    require(['jquery', 'ko', 'ko.x-editable', 'urls', 'toastr', 'modelData', 'dropdownData', 'x-editable', 'enumeration/PhoneNumberType', 'enumeration/ActivityType', 'enumeration/HomeownerContactType', 'jquery.maskedinput', 'enumeration/ServiceCallStatus', 'enumeration/ServiceCallLineItemStatus', 'app/formUploader', '/Scripts/lib/jquery.color-2.1.0.min.js'], function($, ko, koxeditable, urls, toastr, modelData, dropdownData, xeditable, phoneNumberTypeEnum, activityTypeEnum, homeownerContactTypeEnum, maskedInput, serviceCallStatusData, serviceCallLineItemStatusData) {
        $(function () {
            $(".phone-number-with-extension").on('shown', function () {
                $(this).data('editable').input.$input.mask('?(999)-999-9999 **********', { placeholder: " " });
            });
            //debugger;
            var AdditionalContact = function (options) {
                this.init('additionalContact', options, AdditionalContact.defaults);
            };

            //inherit from Abstract input
            $.fn.editableutils.inherit(AdditionalContact, $.fn.editabletypes.abstractinput);

            $.extend(AdditionalContact.prototype, {

                /**
                Renders input from tpl
        
                @method render() 
                **/
                render: function () {
                    ////debugger;
                    this.$input = this.$tpl.find('input');
                },

                /**
                Default method to show value in element. Can be overwritten by display option.
                
                @method value2html(value, element) 
                **/
                value2html: function (value, element) {
                    //debugger;
                    if (!value) {
                        $(element).empty();
                        return;
                    }
                    var html = $('<div>').text(value.contactValue).html() + ' (' + $('<div>').text(value.contactLabel).html() + ')';
                    $(element).html(html);
                },

                /**
                Gets value from element's html
                
                @method html2value(html) 
                **/
                html2value: function (html) {
                    //debugger;
                    /*
                      you may write parsing method to get value by element's html
                      e.g. "Moscow, st. Lenina, bld. 15" => {contactValue: "Moscow", contactLabel: "Lenina", building: "15"}
                      but for complex structures it's not recommended.
                      Better set value directly via javascript, e.g. 
                      editable({
                          value: {
                              contactValue: "Moscow", 
                              contactLabel: "Lenina", 
                              building: "15"
                          }
                      });
                    */
                    return null;
                },

                /**
                 Converts value to string. 
                 It is used in internal comparing (not for sending to server).
                 
                 @method value2str(value)  
                **/
                value2str: function (value) {
                    //debugger;
                    var str = '';
                    if (value) {
                        for (var k in value) {
                            str = str + k + ':' + value[k] + ';';
                        }
                    }
                    return str;
                },

                /*
                 Converts string to value. Used for reading value from 'data-value' attribute.
                 
                 @method str2value(str)  
                */
                str2value: function (str) {
                    //debugger;
                    /*
                    this is mainly for parsing value defined in data-value attribute. 
                    If you will always set value by javascript, no need to overwrite it
                    */
                    return str;
                },

                /**
                 Sets value of input.
                 
                 @method value2input(value) 
                 @param {mixed} value
                **/
                value2input: function (value) {
                    //debugger;
                    if (!value) {
                        return;
                    }
                    this.$input.filter('[name="contactValue"]').val(value.contactValue);
                    this.$input.filter('[name="contactLabel"]').val(value.contactLabel);
                },

                /**
                 Returns value of input.
                 
                 @method input2value() 
                **/
                input2value: function () {
                    //debugger;
                    return {
                        contactValue: this.$input.filter('[name="contactValue"]').val(),
                        contactLabel: this.$input.filter('[name="contactLabel"]').val(),
                    };
                },

                /**
                Activates input: sets focus on the first field.
                
                @method activate() 
               **/
                activate: function () {
                    //debugger;
                    this.$input.filter('[name="contactValue"]').focus();
                },

                /**
                 Attaches handler to submit form in case of 'showbuttons=false' mode
                 
                 @method autosubmit() 
                **/
                autosubmit: function () {
                    //debugger;
                    this.$input.keydown(function (e) {
                        if (e.which === 13) {
                            $(this).closest('form').submit();
                        }
                    });
                }
            });

            AdditionalContact.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
                tpl: '<div class="form-inline"><div class="editable-additionalContact"><input type="text" name="contactValue" class="input-small"></label></div>' +
                     '<div class="editable-additionalContact"><input type="text" name="contactLabel" class="input-small" placeholder="Label"></label></div></div>',
                inputclass: ''
            });

            $.fn.editabletypes.additionalContact = AdditionalContact;
        });
    });
});
