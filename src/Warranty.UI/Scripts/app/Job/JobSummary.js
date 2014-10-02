require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'x-editable', 'ko', 'toastr', 'urls', 'modelData', 'enumeration/PhoneNumberType', 'jquery.maskedinput', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, xeditable, ko, toastr, urls, modelData, phoneNumberTypeEnum, maskedInput) {
        $(function () {
            $.fn.editable.defaults.mode = 'inline';
            $.fn.editable.defaults.emptytext = 'Add';
            $.fn.editableform.buttons =
                '<button type="submit" class="btn btn-primary editable-submit btn-xs"><i class="icon-ok icon-white"></i>Save</button>';

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
                $(this).data('editable').input.$input.mask('?(999)-999-9999 **********', { placeholder: " " });
            });

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
                self.jobNoteDescriptionToAdd('');
            }
            
            function jobSummaryViewModel() {
                var self = this;

                self.jobId = ko.observable($("#jobId").val());
                self.userCanAddNotes = ko.observable(modelData.canAddNotes);
                self.allJobNotes = ko.observableArray([]);
                self.jobNoteDescriptionToAdd = ko.observable('');
                
                self.addJobNote = function () {
                    var newNoteDescription = $("#addJobNoteDescription");
                    if (newNoteDescription.val() == "") {
                        $(newNoteDescription).parent().addClass("has-error");
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

                self.cancelJobNote = function() {
                    clearNoteFields();
                };
            }

            function JobNotesViewModel(option) {
                var self = this;
                
                self.jobNoteId = option.jobNoteId;
                self.jobId = option.jobId;
                self.note = option.note;
                self.createdBy = option.createdBy;
                self.createdDate = option.createdDate;
            };

            var viewModel = new jobSummaryViewModel();
            ko.applyBindings(viewModel);

            var persistedAllJobNotesViewModel = modelData.initialJobNotes;

            _(persistedAllJobNotesViewModel).each(function(note) {
                viewModel.allJobNotes.push(new JobNotesViewModel(note));
            });

        });
    });
});