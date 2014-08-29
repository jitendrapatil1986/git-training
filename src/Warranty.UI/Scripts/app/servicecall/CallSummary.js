define(['urls','jquery'], function(urls, $) {
    $(function() {
           
        $(".approve-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Approve;
            executeApproval(url, serviceCallId, $(this), 'Open');
        });
            
        $(".deny-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Deny;
            executeApproval(url, serviceCallId, $(this), 'Closed');
        });
            
        function executeApproval(url, serviceCallId, button, status) {
            debugger;
            $.ajax({
                type: "POST",
                url: url,
                data: { id: serviceCallId },
                success: function (result) {
                    button.parent().html('<span>Status: <span class="label label-' + status.toLowerCase() + '-service-call">' + status + '</span></span>');
                }
            });
        }
    });
});