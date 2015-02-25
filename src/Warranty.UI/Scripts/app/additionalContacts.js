require(['/Scripts/app/main.js'], function() {
    require(['jquery', 'ko', 'ko.x-editable', 'bootbox', 'urls', 'toastr', 'additionalContactsData', 'x-editable', 'enumeration/PhoneNumberType', 'enumeration/HomeownerContactType', 'jquery.maskedinput'], function ($, ko, koxeditable, bootbox, urls, toastr, additionalContactsData, xeditable, phoneNumberTypeEnum, homeownerContactTypeEnum) {
        $(function () {
            $.fn.editable.defaults.mode = 'inline';

            var AdditionalPhoneContact = function (options) {
                this.init('additionalPhoneContact', options, AdditionalPhoneContact.defaults);
            };

            //inherit from Abstract input
            $.fn.editableutils.inherit(AdditionalPhoneContact, $.fn.editabletypes.abstractinput);
            
            var AdditionalEmailContact = function (options) {
                this.init('additionalEmailContact', options, AdditionalEmailContact.defaults);
            };

            //inherit from Abstract input
            $.fn.editableutils.inherit(AdditionalEmailContact, $.fn.editabletypes.abstractinput);

            var defaultMethods = {
                /**
                Renders input from tpl
        
                @method render() 
                **/
                render: function() {
                    //
                    this.$input = this.$tpl.find('input');
                },

                /**
                Default method to show value in element. Can be overwritten by display option.
                
                @method value2html(value, element) 
                **/
                value2html: function(value, element) {
                    
                    if (!value) {
                        $(element).empty();
                        return;
                    }
                    var label = '';
                    if (value.contactLabel) {
                        label = ' (' + $('<div>').text(value.contactLabel).html() + ')';
                    }

                    var contactValue;
                    if (typeof(value.contactValue) === 'function') {  //means this is from ko observable when pg loads.
                        contactValue = value.contactValue();
                    } else {  //means this is a string from editing value.
                        contactValue = value.contactValue;
                    }
                    
                    var html = $('<div>').text(contactValue).html() + label;
                    $(element).html(html);
                },

                /**
                Gets value from element's html
                
                @method html2value(html) 
                **/
                html2value: function(html) {
                    
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
                value2str: function(value) {
                    
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
                str2value: function(str) {
                    
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
                value2input: function(value) {
                    
                    if (!value) {
                        return;
                    }
                    this.$input.filter('[name="contactValue"]').val(value.contactValue());
                    this.$input.filter('[name="contactLabel"]').val(value.contactLabel);
                },

                /**
                 Returns value of input.
                 
                 @method input2value() 
                **/
                input2value: function () {
                    return {
                        contactValue: this.$input.filter('[name="contactValue"]').val(),
                        contactLabel: this.$input.filter('[name="contactLabel"]').val(),
                    };
                },

                /**
                Activates input: sets focus on the first field.
                
                @method activate() 
               **/
                activate: function() {
                    
                    this.$input.filter('[name="contactValue"]').focus();
                },

                /**
                 Attaches handler to submit form in case of 'showbuttons=false' mode
                 
                 @method autosubmit() 
                **/
                autosubmit: function() {
                    
                    this.$input.keydown(function(e) {
                        if (e.which === 13) {
                            $(this).closest('form').submit();
                        }
                    });
                }
            };
            
            $.extend(AdditionalPhoneContact.prototype, defaultMethods);
            $.extend(AdditionalEmailContact.prototype, defaultMethods);

            AdditionalPhoneContact.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
                tpl: '<div class="contact-value"><input type="text" name="contactValue" class="input-small phone-number-with-extension"></label></div>' +
                     '<div><input type="text" name="contactLabel" class="input-small" placeholder="Label"></label></div>',
                inputclass: ''
            });
            
            AdditionalEmailContact.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
                tpl: '<div class="contact-value"><input type="text" name="contactValue" class="input-small"></label></div>' +
                     '<div><input type="text" name="contactLabel" class="input-small" placeholder="Label"></label></div>',
                inputclass: ''
            });

            $.fn.editabletypes.additionalPhoneContact = AdditionalPhoneContact;
            
            $.fn.editabletypes.additionalEmailContact = AdditionalEmailContact;
            
            $(document).on("focus", ".phone-number-with-extension", function () {
                $(this).mask("(999) 999-9999? x99999", { placeholder: " " });
            });
            
            function formatPhoneNumber(phoneNumber) {
                if (!phoneNumber || phoneNumber.length < 10)
                    return phoneNumber;
                var area = phoneNumber.substring(0, 3);
                var major = phoneNumber.substring(3, 6);
                var minor = phoneNumber.substring(6, 10);
                var extension = " x" + phoneNumber.substring(10, phoneNumber.length );

                return '(' + area + ') ' + major + '-' + minor + (extension.length > 2 ? extension : '');
            }
            
            function isEmail(email) {
                var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
                return regex.test(email);
            }
            
            function AddtionalEmailContactViewModel(options) {
                var self = this;
                self.contactValue = ko.observable(options.contactValue);
                self.contactLabel = options.contactLabel;
                self.homeownerContactTypeValue = options.homeownerContactTypeValue;
                self.homeownerId = options.homeownerId;
                self.homeownerContactId = options.homeownerContactId;
            }

            function AddtionalPhoneContactViewModel(options) {
                var self = this;
                self.contactValue = ko.observable(formatPhoneNumber(options.contactValue));
                self.contactLabel = options.contactLabel;
                self.homeownerContactTypeValue = options.homeownerContactTypeValue;
                self.homeownerId = options.homeownerId;
                self.homeownerContactId = options.homeownerContactId;
            }

            function additionalContactsViewModel() {
                var self = this;

                self.additionalEmailContacts = ko.observableArray([]);
                self.additionalPhoneContacts = ko.observableArray([]);


                self.afterAdditionalContactRendered = function (element, index, data) {
                    var type;
                    if (index.homeownerContactTypeValue == homeownerContactTypeEnum.Phone.Value) {
                        type = 'additionalPhoneContact';
                    }
                    else if (index.homeownerContactTypeValue == homeownerContactTypeEnum.Email.Value) {
                        type = 'additionalEmailContact';
                    }

                    var theContactValue;
                    
                    $(element[1]).find("a.additional-contact-email, a.additional-contact-phone").editable({
                        value: index,
                        url: urls.Homeowner.AddOrUpdateAdditionalContact,
                        send: 'always',
                        params: function (params) {
                            params.homeownerId = index.homeownerId;
                            params.homeownerContactTypeValue = index.homeownerContactTypeValue;
                            params.homeownerContactId = index.homeownerContactId;
                            theContactValue = params.value.contactValue;
                            return params;
                        },
                        validate: function (value) {
                                if (index.homeownerContactTypeValue == homeownerContactTypeEnum.Phone.Value) {
                                    if (value.contactValue.length <10) {
                                        return 'Phone is required';
                                    }
                                }
                                else if (index.homeownerContactTypeValue == homeownerContactTypeEnum.Email.Value) {
                                    if ($.trim(value.contactValue) == '') {
                                        return 'Email is required';
                                    }
                                    else if (!isEmail(value.contactValue)) {
                                        return 'Invalid Email';
                                    }
                                }
                        },
                        type: type,
                        success: function (response) {

                            if (response.isNew == true) {

                                var newDomelement;
                                var newElement;

                                if (response.homeOwnercontactTypeVlue == homeownerContactTypeEnum.Phone.Value) {
                                    newDomelement = $('.additional-contact-phone').last();
                                    newDomelement.parent().addClass('can-remove');
                                    newElement = self.additionalPhoneContacts()[self.additionalPhoneContacts().length - 1];
                                    newElement.homeownerContactId = response.homeownerContactId;
                                    newElement.contactValue(theContactValue);
                                    self.addPhoneContact();
                                } else if (response.homeOwnercontactTypeVlue == homeownerContactTypeEnum.Email.Value) {
                                    newDomelement = $('.additional-contact-email').last();
                                    newDomelement.parent().addClass('can-remove');
                                    newElement = self.additionalEmailContacts()[self.additionalEmailContacts().length - 1];
                                    newElement.homeownerContactId = response.homeownerContactId;
                                    newElement.contactValue(theContactValue);
                                    self.addEmailContact();
                                }
                                toastr.success("Success! Contact added.");

                            }
                        }
                    });
                };

                self.removeAdditionalContact = function (e) {


                    bootbox.confirm("Are you sure you want to delete this contact?", function (result) {
                        if (result) {
                            var homeownerConactId = e.homeownerContactId;
                            $.ajax({
                                type: "POST",
                                url: urls.Homeowner.DeleteAdditionalContact,
                                data: { id: homeownerConactId },
                                success: function (data) {

                                    if (e.homeownerContactTypeValue == homeownerContactTypeEnum.Phone.Value) {
                                        self.additionalPhoneContacts.remove(e);
                                    }
                                    else if (e.homeownerContactTypeValue == homeownerContactTypeEnum.Email.Value) {

                                        self.additionalEmailContacts.remove(e);
                                    }
                                    toastr.success("Success! Contact deleted.");
                                }
                            });
                        }
                    });
                };

                self.addEmailContact = function () {
                    self.additionalEmailContacts.push(new AddtionalEmailContactViewModel({
                        contactValue: null,
                        homeownerId: $('#HomeownerId').val(),
                        homeownerContactTypeValue: homeownerContactTypeEnum.Email.Value
                    }));

                    var element = $('.additional-contact-email').last();
                    element.text('+ Add additional email');
                    element.parent().removeClass('can-remove');
                };

                self.addPhoneContact = function () {
                    self.additionalPhoneContacts.push(new AddtionalPhoneContactViewModel({
                        contactValue: null,
                        homeownerId: $('#HomeownerId').val(),
                        homeownerContactTypeValue: homeownerContactTypeEnum.Phone.Value
                    }));

                    var element = $('.additional-contact-phone').last();
                    element.text('+ Add additional phone number');
                    element.parent().removeClass('can-remove');
                };
            }


            var viewModel = new additionalContactsViewModel();

            var additionalEmailContacsViewModel = additionalContactsData.additionalEmailContacts;

            _(additionalEmailContacsViewModel).each(function(contact) {
                viewModel.additionalEmailContacts.push(new AddtionalEmailContactViewModel(contact));
            });

            var additionalPhoneContacsViewModel = additionalContactsData.additionalPhoneContacts;

            _(additionalPhoneContacsViewModel).each(function(contact) {
                viewModel.additionalPhoneContacts.push(new AddtionalPhoneContactViewModel(contact));
            });

            
            ko.applyBindings(viewModel, document.getElementById('Additional_Contacts'));

            viewModel.addPhoneContact();
            viewModel.addEmailContact();
        });
    });
});
