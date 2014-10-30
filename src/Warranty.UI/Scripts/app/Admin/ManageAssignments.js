require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'ko', 'ko.x-editable', 'urls', 'toastr', 'modelData', 'dropdownData', 'typeahead', 'bloodhound'], function ($, ko, koxeditable, urls, toastr, modelData, dropdownData, typeahead, bloodhound) {
        $(function() {
            //$('#search').hideseek({
            //    nodata: 'No results found'
            //});

            //$('.combobox').combobox();

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
                
                _(options.employees).each(function (item) {  //Employee list tied to the community and NOT just the employee list of all emps.
                    self.communityAssignedEmployees.push(new EmployeeCommunityAssignmentItemViewModel(item));
                    self.communityAssignmentId(item.employeeAssignmentId);
                    self.selectedEmployeeName(item.name);
                });

                self.assignEmployeeToCommunity = function() {
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
                        if (response.Success == true) toastr.success("Success! Community assignment updated.");;
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
                self.editLine = function () {
                    this.assigneeEditing(true);
                    this.lineEditing(true);
                    this.currentSelectedEmployeeId(this.selectedEmployeeId());
                    this.currentSelectedEmployeeName(this.selectedEmployeeName());
                };

                ////save line item changes.
                //self.saveLineItemChanges = function () {
                //    updateServiceCallLineItem(this);
                //};

                //cancel line item changes.
                self.cancelLineItemChanges = function () {
                    this.assigneeEditing(false);
                    this.lineEditing(false);
                    this.selectedEmployeeId(this.currentSelectedEmployeeId());
                    this.selectedEmployeeName(this.currentSelectedEmployeeName());
                };
            }

            function manageCommunityAssignmentsViewModel() {
                var self = this;
                
                self.allCommunityAssignments = ko.observableArray([]);
                self.selectedEmployeeId = ko.observable('');
                self.selectedEmployeeName = ko.observable('');
                self.theLookups = dropdownData.availableLookups; //dropdown list does not need to be observable. Only the actual elements w/i the array do.
            }

            var viewModel = new manageCommunityAssignmentsViewModel();
            ko.applyBindings(viewModel);

            var persistedCommunityAssignmentsItemsViewModel = modelData.initialCommunityAssignmentLines;

            _(persistedCommunityAssignmentsItemsViewModel).each(function (item) {
                viewModel.allCommunityAssignments.push(new CommunityAssignmentsItemsViewModel(item));
            });

            $("a[data-assignmentId]").on("click", function() {
                var element = $(this);

                $.ajax({
                    type: 'POST',
                    url: urls.AssignWSR.RemoveAssignment,
                    data: {
                        assignmentId: $(this).attr("data-assignmentId")
                    },
                    dataType: "json"
                }).fail(function() {
                    toastr.error("Unable to remove assignment at this time.");
                }).done(function(response) {
                    if (response.Success == true) element.parent().hide();
                });
            });


            //>>Testing twitter typeahead.
            var employeeList = dropdownData.availableLookups;

            // constructs the suggestion engine
            var employees = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.obj.whitespace('employeeName'),
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                // `states` is an array of state names defined in "The Basics"
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
                
                var currentValue = $(this).val();
                var selectionInList = false;
                
                _(employees.local).each(function(item) {
                    if (item.employeeName === currentValue) {
                        selectionInList = true;
                        return;
                    }
                });

                if (selectionInList === false) {
                    $(this).closest('#employeeSearch').val('');
                    setEmployeeLineDetails(this, '', '');
                }
                
            }).on('typeahead:selected typeahead:autocompleted', function (event, data) {
                setEmployeeLineDetails(this, data.employeeName, data.employeeId);
            });
            
            function setEmployeeLineDetails(currentElement, theEmployeeName, theEmployeeId) {
                var communityRow = $(currentElement).closest('.communityDesc');
                var arrayIndex = communityRow.attr('data-manage-assignment-line-item') - 1;

                viewModel.allCommunityAssignments()[arrayIndex].selectedEmployeeName(theEmployeeName);
                viewModel.allCommunityAssignments()[arrayIndex].selectedEmployeeId(theEmployeeId);
            }
            
            //<<Testing twitter typeahead.



                        var states = ['Alabama', 'Alaska', 'Arizona', 'Arkansas', 'California',
              'Colorado', 'Connecticut', 'Delaware', 'Florida', 'Georgia', 'Hawaii',
              'Idaho', 'Illinois', 'Indiana', 'Iowa', 'Kansas', 'Kentucky', 'Louisiana',
              'Maine', 'Maryland', 'Massachusetts', 'Michigan', 'Minnesota',
              'Mississippi', 'Missouri', 'Montana', 'Nebraska', 'Nevada', 'New Hampshire',
              'New Jersey', 'New Mexico', 'New York', 'North Carolina', 'North Dakota',
              'Ohio', 'Oklahoma', 'Oregon', 'Pennsylvania', 'Rhode Island',
              'South Carolina', 'South Dakota', 'Tennessee', 'Texas', 'Utah', 'Vermont',
              'Virginia', 'Washington', 'West Virginia', 'Wisconsin', 'Wyoming'
                        ];
            
                        // constructs the suggestion engine
                        var states = new Bloodhound({
                            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
                            queryTokenizer: Bloodhound.tokenizers.whitespace,
                            // `states` is an array of state names defined in "The Basics"
                            local: $.map(states, function (state) { return { value: state }; })
                        });

                        // kicks off the loading/processing of `local` and `prefetch`
                        states.initialize();

                        $('#bloodhound .typeahead').typeahead({
                            hint: true,
                            highlight: true,
                            minLength: 1
                        },
                        {
                            name: 'states',
                            displayKey: 'value',
                            // `ttAdapter` wraps the suggestion engine in an adapter that
                            // is compatible with the typeahead jQuery plugin
                            source: states.ttAdapter()
                        });
        });
        
        
    });
});