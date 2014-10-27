define(['jquery','urls','toastr'], function ($, urls, toastr) {
    $(function () {

        $(".approve-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var displaystatus = $(this).data("display-status");
            var url = urls.ServiceCall.Approve;
            executeApproval(url, serviceCallId, $(this), 'Open', displaystatus, 'approved');
        });

        $(".deny-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var displaystatus = $(this).data("display-status");
            var url = urls.ServiceCall.Deny;
            executeApproval(url, serviceCallId, $(this), 'Completed', displaystatus, 'denied');
        });

        function executeApproval(url, serviceCallId, button, status, displayStatus, actionName) {
            $.ajax({
                type: "POST",
                url: url,
                data: { id: serviceCallId },
                success: function (result) {
                    if (displayStatus == true) {
                        button.parent().html('<span>Status: <span class="label label-' + status.toLowerCase() + '-service-call">' + status + '</span></span>');
                    } else {
                        button.parent().html('');
                    }
                    toastr.success("Success! Service Call has been " + actionName);
                }
            });
        }
       
    }());

});
