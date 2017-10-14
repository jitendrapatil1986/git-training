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
                self.employeeId = ko.observable('');
                self.employeeName = ko.observable('');
                self.typeaheadSelectedEmployeeId = ko.observable('');
                self.typeaheadSelectedEmployeeName = ko.observable('');
                viewModel.loading(false);
                
                //employees is the submodel, which is employee list tied to the community, where a community only has 1 employee assigned to it.
                _(options.employees).each(function (item) {
                    self.communityAssignmentId(item.employeeAssignmentId);
                    self.employeeName(item.name);
                });

                self.assignEmployeeToCommunity = function () {
                    $.ajax({
                        type: 'POST',
                        url: urls.AssignWSR.UpdateAssignment,
                        data: {
                            selectedCommunityId: this.communityId,
                            selectedEmployeeId: this.employeeId(),
                            SelectedEmployeeAssignmentId : this.communityAssignmentId,
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
                    $(event.currentTarget).closest(".itemLabel").find(".tt-input").focus();
                };
                
                //specify typeahead options for knockout custom binding.
                self.typeaheadOptions = {
                    name: 'employees',
                    displayKey: 'employeeName',
                    limit: 20,
                    minLength: 2,
                    remote: {
                        url: urls.QuickSearch.Employees + '?query=%QUERY',
                        filter: function (list) {
                            return $.map(list, function (employee) {
                                return {
                                    employeeName: employee.Name,
                                    employeeId: employee.Id
                                };
                            });
                        }
                    }
                };
            }

            function manageCommunityAssignmentsViewModel() {
                var self = this;
                
                self.allCommunityAssignments = ko.observableArray([]);
                self.loading = ko.observable(true);
            }


            //Twitter typeahead setup
            //KO using custom binding with typeahead.
            ko.bindingHandlers.typeahead = {
                init: function (element, valueAccessor, allBindingsAccessor, bindingViewModel) {
                    var el = $(element);
                    var options = ko.utils.unwrapObservable(valueAccessor());
                    var allBindings = allBindingsAccessor();

                    var data = new Bloodhound({
                        datumTokenizer: Bloodhound.tokenizers.obj.whitespace(options.displayKey),
                        queryTokenizer: Bloodhound.tokenizers.whitespace,
                        limit: options.limit,
                        remote: options.remote
                    });

                    // kicks off the loading/processing of data.
                    data.initialize();
                    
                    el.attr("autocomplete", "off").typeahead({
                        hint: true,
                        highlight: true,
                        minLength: 2
                    },
                    {
                        name: options.name,
                        displayKey: options.displayKey,
                        // `ttAdapter` wraps the suggestion engine in an adapter that
                        // is compatible with the typeahead jQuery plugin
                        source: data.ttAdapter()
                    }).bind("typeahead:selected typeahead:autocompleted", function (obj, datum) {
                        //save currently selected value on line item vm property.
                        bindingViewModel.typeaheadSelectedEmployeeId(datum.employeeId);
                        bindingViewModel.typeaheadSelectedEmployeeName(datum.employeeName);
                    }).blur(function () {
                        var currentEmployeeName = $(this).val();

                        //ensures values selected is a valid employee bc user selected or prefilled with typeahead.
                        if ($.trim(currentEmployeeName) != '' && currentEmployeeName === bindingViewModel.typeaheadSelectedEmployeeName()) {
                            bindingViewModel.employeeName(currentEmployeeName);
                            bindingViewModel.employeeId(bindingViewModel.typeaheadSelectedEmployeeId());
                            bindingViewModel.assignEmployeeToCommunity();
                        }

                        $(this).val('');
                        bindingViewModel.assigneeEditing(false);
                        bindingViewModel.typeaheadSelectedEmployeeId('');
                        bindingViewModel.typeaheadSelectedEmployeeName('');
                    });
                }
            };
            

            var viewModel = new manageCommunityAssignmentsViewModel();
            ko.applyBindings(viewModel);

            var persistedCommunityAssignmentsItemsViewModel = modelData.initialCommunityAssignmentLines;

            _(persistedCommunityAssignmentsItemsViewModel).each(function (item) {
                viewModel.allCommunityAssignments.push(new CommunityAssignmentsItemsViewModel(item));
            });
            
        });
    });
});