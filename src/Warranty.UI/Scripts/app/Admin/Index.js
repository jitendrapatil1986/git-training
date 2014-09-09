require(['urls', 'jquery'], function () {
    $("#bAddAssignment").on("click", function (urls) {
        $.ajax({
            type: 'post',
            url: urls.Admin.AddAssignment,
            data: {
                communityId: $("#community").val(),
                employeeId: $("#employee").val()
            },
            success: function() {
                location.reload();
            },
            fail: function() {
                toastr.error("Unable to add assignment at this time.");
            }
        });
    });
});
