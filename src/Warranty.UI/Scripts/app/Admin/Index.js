require(['/Scripts/app/main.js'], function() {
    require(['jquery', 'urls', 'toastr'], function ($, urls, toastr) {
        $("#bAddAssignment").on("click", function() {
            $.ajax({
                type: 'POST',
                url: urls.AssignWSR.AddAssignment,
                data: {
                    communityId: $("#community").val(),
                    employeeId: $("#employee").val()
                },
                success: function () {
                    console.log(urls.AssignWSR.AddAssignment);
                    toastr.success("Successfully added the WSR to the community.");
                    location.reload();
                }
            }).fail(function() {
                toastr.error("Unable to add assignment at this time.");
            });
        });
    });
});