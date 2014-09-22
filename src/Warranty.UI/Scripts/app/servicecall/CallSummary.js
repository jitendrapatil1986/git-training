require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'urls', 'toastr', 'modelData', 'dropdownData', '/Scripts/lib/jquery.color-2.1.0.min.js'], function ($, ko, urls, toastr, modelData, dropdownData) {
        $(function () {

            $('.btn-action-with-popup').click(function (e) {
                $(this).addClass("active");
                $('.popup-action-with-message').hide();
                var right = ($(window).width() - ($(this).offset().left + $(this).outerWidth()));
                var actionwithPopup = $(this).data('action-with-popup');
                $("#" + actionwithPopup).css({
                    'position': 'absolute',
                    'right': right,
                    'top': $(this).offset().top + $(this).height() + 15
                }).show();
            });
            
            $('.btn-cancel-popup').click(function (e) {
                var popupWindow = $(this).parent();
                var parentButton = $("#btn_" + popupWindow.attr('id'));
                parentButton.removeClass("active");
                popupWindow.hide();
            });

            $('.btn-execute-action').click(function (e) {
                var popupWindow = $(this).parent();
                var actionUrl = $(this).data('action-url');
                var textArea = $(this).prev('textarea');
                var message = textArea.val();
                textArea.val('');
                var serviceCallId = $(this).data('service-call-id');
                var parentButton = $("#btn_" + popupWindow.attr('id'));
                $.ajax({
                    type: "POST",
                    url: actionUrl,
                    data: { id: serviceCallId, message: message },

                    success: function (data) {
                        changeButtonText(parentButton);
                        parentButton.removeClass("active");
                        popupWindow.hide();
                    }
                });
                
                function changeButtonText(button) {
                    var currentText = button.text();
                    var nextText = button.data('next-text');
                    button.data('next-text', currentText);
                    button.text(nextText);
                }
            });
            

            $(".approve-button").click(function (e) {
                e.preventDefault();
                var serviceCallId = $(this).data("service-call-id");
                var url = urls.ServiceCall.Approve;
                executeApproval(url, serviceCallId, $(this), 'Open');
            });

            $(".deny-button").click(function (e) {
                e.preventDefault();
                var serviceCallId = $(this).data("service-call-id");
                var url = urls.ServiceCall.Deny;
                executeApproval(url, serviceCallId, $(this), 'Closed');
            });

            function executeApproval(url, serviceCallId, button, status) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { id: serviceCallId },
                    success: function (result) {
                        button.parent().html('<span>Status: <span class="label label-' + status.toLowerCase() + '-service-call">' + status + '</span></span>');
                    }
                });
            }
            
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
                $("#addCallNoteLineReferenceDropDown").val('');
                self.selectedLineToAttachToNote('');
                self.noteDescriptionToAdd('');
            }
            
            function AllLineItemsViewModel(options) {
                var self = this;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.completed = options.completed;

                //track line item properties.
                self.problemCodeId = ko.observable(options.problemCodeId);
                self.problemCode = ko.observable(options.problemCode);
                self.problemDescription = ko.observable(options.problemDescription);
                self.currentProblemCode = ko.observable();
                self.currentProblemDescription = ko.observable();
                
                //track editing problem code, desc, and line altogether.
                self.problemCodeEditing = ko.observable();
                self.problemDescriptionEditing = ko.observable("");
                self.lineEditing = ko.observable("");
                
                //edit line item.
                self.editLine = function() {
                    this.problemCodeEditing(true);
                    this.problemDescriptionEditing(true);
                    this.lineEditing(true);
                    this.currentProblemCode(this.problemCode());
                    this.currentProblemDescription(this.problemDescription());
                };
                
                //save line item changes.
                self.saveLineItemChanges = function () {
                    this.problemCodeEditing(false);
                    this.problemDescriptionEditing(false);
                    this.lineEditing(false);
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
                
                self.lineNumber = ko.observable(options.lineNumber);
                
                self.lineNumberWithProblemCode = ko.computed(function() {
                    return self.lineNumber() + " - " + self.problemCode();
                });

            }
            
            function CallNotesViewModel(options) {
                var self = this;
                self.serviceCallNoteId = options.serviceCallNoteId;
                self.serviceCallId = options.serviceCallId;
                self.serviceCallLineItemId = options.serviceCallLineItemId;
                self.note = options.note;
                self.createdBy = options.createdBy;
                self.createdDate = options.createdDate;
                self.serviceCallCommentTypeId = options.serviceCallCommentTypeId;
            }

            function updateServiceCallLineItem(line) {
                //TODO: Add validations.

                var lineData = ko.toJSON(line);

                $.ajax({
                    url: urls.ManageServiceCall.EditLineItem,
                    type: "POST",
                    data: lineData,
                    dataType: "json",
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                })
                    .fail(function (response) {
                        toastr.error("There was an issue updating the line item. Please try again!");
                    })
                    .done(function (response) {
                        toastr.success("Success! Item updated.");
                        self.problemCode = line.problemCode;
                    });
            }
            
            function createServiceCallLineItemViewModel() {
                var self = this;
                
                self.allLineItems = ko.observableArray([]);
                self.theLookups = dropdownData.availableLookups;  //dropdown list does not need to be observable. Only the actual elements w/i the array do.
                self.allCallNotes = ko.observableArray([]);
                self.selectedLineToAttachToNote = ko.observable();
                self.selectedLineToFilterNotes = ko.observable();
                self.noteDescriptionToAdd = ko.observable('');
                self.filteredCallNotes = ko.computed(function() {
                    var lineIdToFilterNotes = self.selectedLineToFilterNotes();
                    if (!lineIdToFilterNotes || lineIdToFilterNotes == "") {
                        return self.allCallNotes();
                    } else {
                        return ko.utils.arrayFilter(self.allCallNotes(), function (i) {
                            return i.serviceCallLineItemId == lineIdToFilterNotes;
                        });
                    }
                });
                
                self.addLineItem = function() {
                    self.serviceCallId = $("#addCallLineServiceCallId").val();
                    self.problemCode = $("#addCallLineProblemCode").find('option:selected').text();
                    self.problemCodeId = $("#addCallLineProblemCode").val();
                    self.problemDescription = $("#addCallLineProblemDescription").val();
                    
                    var newProblemCode = $("#addCallLineProblemCode");
                    if (newProblemCode.val() == "") {
                        $(newProblemCode).parent().addClass("has-error");
                        return;
                    }
                    
                    var newProblemDescription = $("#addCallLineProblemDescription");
                    if (newProblemDescription.val() == "") {
                        $(newProblemDescription).parent().addClass("has-error");
                        return;
                    }
                    
                    var newLineItem = new AllLineItemsViewModel({
                        serviceCallId: self.serviceCallId, problemCodeId: self.problemCodeId,
                        problemCode: self.problemCode, problemDescription: self.problemDescription
                    });
                    
                    var lineData = ko.toJSON(newLineItem);

                    $.ajax({
                        url: urls.ManageServiceCall.AddLineItem,
                        type: "POST",
                        data: lineData,
                        dataType: "json",
                        processData: false,
                        contentType: "application/json; charset=utf-8"
                    })
                        .fail(function (response) {
                            toastr.error("There was an issue adding the line item. Please try again!");
                        })
                        .done(function (response) {
                            self.allLineItems.unshift(new AllLineItemsViewModel({
                                serviceCallId: self.serviceCallId,
                                serviceCallLineItemId: response.ServiceCallLineItemId,
                                lineNumber: response.LineNumber,
                                problemCode: self.problemCode,
                                problemCodeId: self.problemCodeId,
                                problemDescription: self.problemDescription,
                                completed: false
                            }));

                            toastr.success("Success! Item added.");
                            highlight($("#allServiceCallLineItems").first());
                            
                            $("#addCallLineProblemDescription").val('');
                            $("#addCallLineProblemCode").val('');
                            self.problemDescription = '';
                        });
                };

                self.addCallNote = function () {
                    self.serviceCallId = $("#addCallLineServiceCallId").val();
                    self.serviceCallLineItemId = $("#addCallNoteLineReferenceDropDown").find('option:selected').val();
                    self.note = $("#addCallNoteDescription").val();

                    var newNoteDescription = $("#addCallNoteDescription");
                    if (newNoteDescription.val() == "") {
                        $(newNoteDescription).parent().addClass("has-error");
                        return;
                    }
                    
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
                            toastr.error("There was an issue adding the call note. Please try again!");
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

                            toastr.success("Success! Note added.");
                            highlight($("#allServiceCallNotes").first());
                            clearNoteFields();
                        });
                };

                self.cancelCallNote = function () {
                    clearNoteFields();
                };
                
                self.resetCallNoteFilter = function () {
                    $("#filterCallNoteLineReferenceDropDown").val('');
                    self.selectedLineToFilterNotes('');
                };
            }

            var viewModel = new createServiceCallLineItemViewModel();
            ko.applyBindings(viewModel);

            var persistedAllLineItemsViewModel = modelData.initialServiceLines;
                
            _(persistedAllLineItemsViewModel).each(function(item) {
                viewModel.allLineItems.push(new AllLineItemsViewModel(item));
            });

            var persistedAllCallNotesViewModel = modelData.initialServiceNotes;

            _(persistedAllCallNotesViewModel).each(function(note) {
                viewModel.allCallNotes.push(new CallNotesViewModel(note));
            });
        });
    });
});