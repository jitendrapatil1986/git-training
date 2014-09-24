define(['urls','jquery'], function(urls, $) {
    $(function () {
        $('.todo:lt(5)').removeClass('hide');

        $('.show-more-todos').click(function () {
            var value = $(this).text();
            $("#Last_Display_Size").val(value);
            hideShowTodos();
        });
        
        function hideShowTodos() {
            var classToIntersect = "";
            var selectedTodoType = $("#toDoSelect").find('option:selected').val();
            
            if (selectedTodoType) {
                classToIntersect = "." + selectedTodoType;
            }
            
            var value = $("#Last_Display_Size").val();
            if (value == 'All') {
                $('.todo').addClass('hide');
                $('.todo' + classToIntersect).removeClass('hide');
            } else {
                var show = value - 1;
                $('.todo').addClass('hide');
                $('.todo' + classToIntersect + ':lt("' + show + '")').removeClass('hide');
            }
            
            if ($('.todo' + classToIntersect).length >= 5) {
                $('#ToDo_Display_Size_Controls').show();
            } else {
                $('#ToDo_Display_Size_Controls').hide();
            }
        }
        
        $('#toDoSelect').change(function() {
            if ($(this).find('option:selected').text() == 'All') {
                $('.todo').removeClass('hide');
            } else {
                var toDoToShow = $(this).find('option:selected').val();
                $('.todo').addClass('hide');
                $('.' + toDoToShow).removeClass('hide');
            }
            hideShowTodos();
        });
            
        $(".approve-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Approve;
            executeApproval(url, serviceCallId);
        });
            
        $(".deny-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Deny;
            executeApproval(url, serviceCallId);
        });
            
        function executeApproval(url, serviceCallId) {
            $.ajax({
                type: "POST",
                url: url,
                data: { id: serviceCallId },
                success: function (result) {
                    $('#service-call-approval-todo-' + serviceCallId).remove();
                    updateTodoWidgetElements();
                }
            });
        }
        
        $(".assign-employee-community-button").click(function (e) {
            e.preventDefault();
            var communityId = $(this).data("community-id");
            var employeeNumber = $("#list_assignable_employee_" + communityId).val();
            
            var url = urls.Community.AssignEmployee;
            
            $.ajax({
                type: "POST",
                url: url,
                data: { CommunityId: communityId, EmployeeNumber: employeeNumber },
                success: function (result) {
                        if (result.success == true) {
                            $('#community-todo-' + communityId).remove();
                            updateTodoWidgetElements();
                        }
                }
            });
        });
        
        function updateTodoWidgetElements() {
            var nextTodo = $(".todo.hide").first();
            if (nextTodo) {
                nextTodo.removeClass("hide");
            }
        }
    });
});