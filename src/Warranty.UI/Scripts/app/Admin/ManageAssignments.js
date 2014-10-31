require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'urls', 'toastr', 'modelData', 'dropdownData', 'typeahead', 'bloodhound', '/Scripts/lib/jquery.hideseek.min.js'], function ($, ko, koxeditable, urls, toastr, modelData, dropdownData, typeahead, bloodhound) {
        $(function() {
            $('#search').hideseek({
                nodata: 'No results found'
            });

            function EmployeeCommunityAssignmentItemViewModel(options) {
                var self = this;

                self.communityAssignedEmployeeId = ko.observable(options.employeeAssignmentId);
                self.communityAssignedEmployeeName = ko.observable(options.name);
            }
            
            function CommunityAssignmentsItemsViewModel(options) {
                var self = this;
                
                self.communityId = options.id;
                self.communityName = options.name;
                self.communityAssignmentId = ko.observable('');
                self.communityEmployeeLookups = viewModel.theLookups;
                self.communityAssignedEmployees = ko.observableArray([]);
                self.selectedEmployeeId = ko.observable('');
                self.selectedEmployeeName = ko.observable('');
                
                _(options.employees).each(function (item) {  //employees is the submodel, which is employee list tied to the community and not just the regular employee list for all emps.
                    self.communityAssignedEmployees.push(new EmployeeCommunityAssignmentItemViewModel(item));
                    self.communityAssignmentId(item.employeeAssignmentId);
                    self.selectedEmployeeName(item.name);
                });

                self.assignEmployeeToCommunity = function () {
                    $.ajax({
                        type: 'POST',
                        url: urls.AssignWSR.UpdateAssignment,
                        data: {
                            selectedCommunityId: this.communityId,
                            selectedEmployeeId: this.selectedEmployeeId()
                        },
                        dataType: "json"
                    }).fail(function () {
                        toastr.error("There was an issue updating the community assignment. Please try again!");
                    }).done(function (response) {
                        if (response.Success == true)
                            toastr.success("Success! Community assignment updated.");
                        self.lineEditing(false);
                        self.assigneeEditing(false);
                    });
                };
                
                self.removeAssignmentFromCommunity = function () {
                    $.ajax({
                        type: 'POST',
                        url: urls.AssignWSR.RemoveAssignment,
                        data: {
                            assignmentId: this.selectedCommunityAssignmentId()
                        },
                        dataType: "json"
                    }).fail(function () {
                        toastr.error("There was an issue unassigning the community. Please try again!");
                    }).done(function (response) {
                        if (response.Success == true) toastr.success("Success! Community has been unassigned.");;
                    });
                };

                //track editing line altogether.
                self.assigneeEditing = ko.observable();
                self.currentSelectedEmployeeId = ko.observable();
                self.currentSelectedEmployeeName = ko.observable();
                self.lineEditing = ko.observable("");

                //edit line item.
                self.editLine = function (data, event) {
                    this.assigneeEditing(true);
                    this.lineEditing(true);
                    this.currentSelectedEmployeeId(this.selectedEmployeeId());
                    this.currentSelectedEmployeeName(this.selectedEmployeeName());
                    $(event.currentTarget).closest(".communityDesc").find(".typeahead").focus();
                };
            }

            function manageCommunityAssignmentsViewModel() {
                var self = this;
                
                self.allCommunityAssignments = ko.observableArray([]);
                self.selectedEmployeeId = ko.observable('');
                self.selectedEmployeeName = ko.observable('');
                self.theLookups = dropdownData.availableLookups;
            }

            var viewModel = new manageCommunityAssignmentsViewModel();
            ko.applyBindings(viewModel);

            var persistedCommunityAssignmentsItemsViewModel = modelData.initialCommunityAssignmentLines;

            _(persistedCommunityAssignmentsItemsViewModel).each(function (item) {
                viewModel.allCommunityAssignments.push(new CommunityAssignmentsItemsViewModel(item));
            });


            //Twitter typeahead setup
            var employeeList = dropdownData.availableLookups;

            //Constructs the suggestion engine
            var employees = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.obj.whitespace('employeeName'),
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                local: $.map(employeeList, function (employee) {
                    return {
                        employeeName: employee.employeeName,
                        employeeId: employee.employeeId
                    };
                })
            });

            employees.initialize();
            
            $('#employeeList .typeahead').typeahead({
                hint: true,
                highlight: true,
                minLength: 1
            },
            {
                name: 'employees',
                displayKey: 'employeeName',
                // `ttAdapter` wraps the suggestion engine in an adapter that
                // is compatible with the typeahead jQuery plugin
                source: employees.ttAdapter()
            }).blur(function () {
                var currentEmployeeName = $(this).val();
                var selectionInList = false;
                var currentEmployeeId = '';
                
                _(employees.local).each(function (item) {
                    if ($.trim(item.employeeName) != '' && item.employeeName === currentEmployeeName) {
                        selectionInList = true;
                        currentEmployeeId = item.employeeId;
                        return;
                    }
                });

                if (selectionInList === true) {
                    setEmployeeLineDetails(this, currentEmployeeName, currentEmployeeId);
                }

                $('.typeahead').typeahead('val', '');
                $(this).closest('#employeeSearch').val('');
                resetEmployeeLineEdit(this);
            });
            
            function resetEmployeeLineEdit(currentElement) {
                var communityRow = $(currentElement).closest('.communityDesc');
                var arrayIndex = communityRow.attr('data-manage-assignment-line-item') - 1;

                viewModel.allCommunityAssignments()[arrayIndex].lineEditing(false);
                viewModel.allCommunityAssignments()[arrayIndex].assigneeEditing(false);
            }
            
            function setEmployeeLineDetails(currentElement, theEmployeeName, theEmployeeId) {
                var communityRow = $(currentElement).closest('.communityDesc');
                var arrayIndex = communityRow.attr('data-manage-assignment-line-item') - 1;

                viewModel.allCommunityAssignments()[arrayIndex].selectedEmployeeName(theEmployeeName);
                viewModel.allCommunityAssignments()[arrayIndex].selectedEmployeeId(theEmployeeId);
                viewModel.allCommunityAssignments()[arrayIndex].assignEmployeeToCommunity();
            }
        });
    });
});