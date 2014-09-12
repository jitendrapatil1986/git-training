require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'urls', 'toastr'], function ($, urls, toastr) {
        $("a[data-assignmentId]").on("click", function () {
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
                if (response.Success == true) element.parent().hide();
            });
        });
    });
});