require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'urls', 'toastr'], function ($, urls, toastr) {
        $("#bAddAssignment").on("click", function () {
            $.ajax({
                type: 'POST',
                url: urls.AssignWSR.AddAssignment,
                data: {
                    communityId: $("#community").val(),
                    employeeId: $("#employee").val()
                },
                dataType: "json",
                success: function () {
                    location.reload();
                }
            }).fail(function () {
                toastr.error("Unable to add assignment at this time.");
            });
        });

        $("span[data-assignmentId]").on("click", function () {
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
            }).done(function (response) {
                if (response == true) element.parent().hide();
            });
        });
    });
});