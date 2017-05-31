define(['urls', 'jquery'], function (urls, $) {
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
        
        function changeToDo() {
            if ($("#toDoSelect").find('option:selected').text() == 'All') {
                $('.todo').removeClass('hide');
            } else {
                var toDoToShow = $("#toDoSelect").find('option:selected').val();
                $('.todo').addClass('hide');
                $('.' + toDoToShow).removeClass('hide');
                saveLastFilter(toDoToShow);
            }
            hideShowTodos();
        }

        $('#toDoSelect').val($("#initialToDo").val());
        changeToDo();

        $('#toDoSelect').change(function () {
            changeToDo($(this));
        });
        
        function saveLastFilter(value) {
            $.ajax({
                type: "POST",
                url: urls.Home.SaveLastSelectedToDoFilter,
                data: { value: value }
            });
        }

        $(".approve-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Approve;
            executeApproval(url, serviceCallId, '#service-call-approval-todo-');
        });

        $(".deny-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Deny;
            executeApproval(url, serviceCallId, '#service-call-approval-todo-');
        });

        function executeApproval(url, dataId, elementPrefix) {
            $.ajax({
                type: "POST",
                url: url,
                data: { id: dataId },
                success: function (result) {
                    var element = $(elementPrefix + dataId);
                    element.remove();
                    updateTodoWidgetElements(element);
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
                        var element = $('#community-todo-' + communityId);
                        element.remove();
                        updateTodoWidgetElements(element);
                    }
                }
            });
        });

        function updateTodoWidgetElements(hiddenelement) {
            var cssClass = hiddenelement.attr('class').replace(/[\s]+/g, ' ').replace(/ /g, ".");
            var nextTodo = $("." + cssClass + ".hide").first();
            if (nextTodo) {
                nextTodo.removeClass("hide");
            }
        }

        $(".complete-task").click(function (e) {
            e.preventDefault();
            var taskId = $(this).data("task-id");
            var url = urls.Task.Complete;
            executeApproval(url, taskId, '#task-todo-');
        });

        $(".no-action-task").click(function (e) {
            e.preventDefault();
            var taskId = $(this).data("task-id");
            var url = urls.Task.NoAction;
            executeApproval(url, taskId, '#task-todo-');
        });
        
        $(".submit-for-approval-task").click(function (e) {
            e.preventDefault();
            var taskId = $(this).data("task-id");
            var url = urls.Task.SubmitForApproval;
            executeApproval(url, taskId, '#task-todo-');
        });
    });
});