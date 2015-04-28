require(['/Scripts/app/main.js'], function () {    
    require(['jquery', 'x-editable', 'ko', 'ko.x-editable', 'toastr', 'urls', 'enumeration/HomeownerContactType', 'enumeration/PhoneNumberType'], function ($, xeditable, ko, koxeditable, toastr, urls, homeownerContactTypeEnum, phoneNumberTypeEnum) {
        window.ko = ko;  //manually set the global ko property.

        require(['ko.validation', 'jquery.color'], function () {
            $(function() {
                function changeHomeownerViewModel() {
                    var self = this;

                    self.jobId = $("#changeHomeownerJobId").val();
                    self.ownerName = ko.observable("").extend({ required: true });
                    self.ownerPhone = ko.observable("");
                    self.ownerEmail = ko.observable("").extend({ email: true });
                    self.additionalPhoneContacts = ko.observableArray([]);
                    self.additionalEmailContacts = ko.observableArray([]);
                    self.addPhoneContactDescription = ko.observable('');
                    self.addEmailContactDescription = ko.observable('');

                    self.addPhoneContact = function() {
                        self.additionalPhoneContacts.push(new HomeownerContactViewModel({
                            contactType: homeownerContactTypeEnum.Phone.Value,
                            contactValue: $("#addPhoneContactDescription").val()
                        }));
                    };

                    self.removePhoneContact = function(lineItem) {
                        self.additionalPhoneContacts.remove(lineItem);
                    };

                    self.addEmailContact = function() {
                        self.additionalEmailContacts.push(new HomeownerContactViewModel({
                            contactType: homeownerContactTypeEnum.Email.Value,
                            contactValue: $("#addEmailContactDescription").val()
                        }));
                    };

                    self.removeEmailContact = function(lineItem) {
                        self.additionalEmailContacts.remove(lineItem);
                    };

                    self.submit = function () {
                        if (self.errors().length != 0) {
                            self.errors.showAllMessages();
                            return;
                        }
                        
                        var newHomeowner =
                        {
                            jobId: self.jobId,
                            newHomeownerName: self.ownerName,
                            newHomeownerHomePhone: self.ownerPhone,
                            newHomeownerEmailAddress: self.ownerEmail,
                            additionalPhoneContacts: self.additionalPhoneContacts,
                            additionalEmailContacts: self.additionalEmailContacts
                        };

                        var newHomeownerData = ko.toJSON(newHomeowner);

                        $.ajax({
                            url: urls.ManageJob.ChangeHomeowner,
                            type: "POST",
                            data: newHomeownerData,
                            dataType: "json",
                            processData: false,
                            contentType: "application/json; charset=utf-8"
                        })
                        .fail(function (response) {
                            toastr.error("There was an issue changing the homeowner. Please try again!");
                        })
                        .done(function (response) {
                            toastr.success("Success! Homeowner changed.");
                            window.location = urls.Job.JobSummary + "/" + $("#changeHomeownerJobId").val();
                        });
                    };
                }

                function HomeownerContactViewModel(option) {
                    var self = this;

                    self.contactType = option.contactType;
                    self.contactValue = ko.observable(option.contactValue);
                }

                ko.validation.init({
                    errorElementClass: 'has-error',
                    errorMessageClass: 'help-block',
                    decorateElement: true
                });
                
                var viewModel = new changeHomeownerViewModel();
                viewModel.errors = ko.validation.group(viewModel);
                ko.applyBindings(viewModel);
            });
        });

    });
});