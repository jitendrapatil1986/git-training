require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'urls', 'toastr', 'modelData', 'typeahead', 'bloodhound', 'hideseek'], function ($, ko, koxeditable, urls, toastr, modelData) {
        $(function() {
            $('#search').hideseek({
                nodata: 'No results found'
            });

            function CommunityAssignmentsItemsViewModel(options) {
                var self = this;
                
                self.communityId = options.id;
                self.communityName = options.name;
                self.communityAssignmentId = ko.observable('');
                self.communityAssignedEmployees = ko.observableArray([]);
                self.selectedEmployeeId = ko.observable('');
                self.selectedEmployeeName = ko.observable('');

                //employees is the submodel, which is employee list tied to the community, where a community only has 1 employee assigned to it.
                _(options.employees).each(function (item) {
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
                        self.assigneeEditing(false);
                    });
                };

                //track editing line altogether.
                self.assigneeEditing = ko.observable();

                //edit line item.
                self.editLine = function (data, event) {
                    this.assigneeEditing(true);
                    $(event.currentTarget).closest(".communityDesc").find(".typeahead").focus();
                };
            }

            function manageCommunityAssignmentsViewModel() {
                var self = this;
                
                self.allCommunityAssignments = ko.observableArray([]);
                self.typeaheadSelectedEmployeeId = ko.observable('');
                self.typeaheadSelectedEmployeeName = ko.observable('');
                self.theLookups = employees;
            }

            var viewModel = new manageCommunityAssignmentsViewModel();
            ko.applyBindings(viewModel);

            var persistedCommunityAssignmentsItemsViewModel = modelData.initialCommunityAssignmentLines;

            _(persistedCommunityAssignmentsItemsViewModel).each(function (item) {
                viewModel.allCommunityAssignments.push(new CommunityAssignmentsItemsViewModel(item));
            });


            //Twitter typeahead setup
            
            //Constructs the suggestion engine
            var employees = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.obj.whitespace('employeeName'),
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                limit: 20,
                remote: {
                    url: urls.QuickSearch.Employees + '?query=%QUERY',
                    filter: function (list) {
                        return $.map(list, function(employee) {
                            return {
                                employeeName: employee.Name,
                                employeeId: employee.Id
                            };
                        });
                    }
                }
            });

            employees.initialize();
            
            $('#employeeList .typeahead').typeahead({
                hint: true,
                highlight: true,
                minLength: 2,
            },
            {
                name: 'employees',
                displayKey: 'employeeName',
                // `ttAdapter` wraps the suggestion engine in an adapter that
                // is compatible with the typeahead jQuery plugin. used for Bloodhound.
                source: employees.ttAdapter()
            }).bind("typeahead:selected typeahead:autocompleted", function (obj, datum) {
                //keep track of valid employee bc user selected or prefilled with typeahead.
                viewModel.typeaheadSelectedEmployeeName = datum.employeeName;
                viewModel.typeaheadSelectedEmployeeId = datum.employeeId;
            }).blur(function () {
                var currentEmployeeName = $(this).val();
                
                //ensures values selected is a valid employee bc user selected or prefilled with typeahead.
                if ($.trim(currentEmployeeName) != '' && currentEmployeeName === viewModel.typeaheadSelectedEmployeeName) {
                    setEmployeeLineDetails(this, currentEmployeeName, viewModel.typeaheadSelectedEmployeeId);
                }

                $('.typeahead').typeahead('val', '');
                $(this).closest('#employeeSearch').val('');
                resetEmployeeLineEdit(this);
            });
            
            function setEmployeeLineDetails(currentElement, theEmployeeName, theEmployeeId) {
                var communityRow = $(currentElement).closest('.communityDesc');
                var arrayIndex = communityRow.attr('data-manage-assignment-line-item') - 1;

                viewModel.allCommunityAssignments()[arrayIndex].selectedEmployeeName(theEmployeeName);
                viewModel.allCommunityAssignments()[arrayIndex].selectedEmployeeId(theEmployeeId);
                viewModel.allCommunityAssignments()[arrayIndex].assignEmployeeToCommunity();
            }
            
            function resetEmployeeLineEdit(currentElement) {
                var communityRow = $(currentElement).closest('.communityDesc');
                var arrayIndex = communityRow.attr('data-manage-assignment-line-item') - 1;

                viewModel.allCommunityAssignments()[arrayIndex].assigneeEditing(false);
            }
        });
    });
});