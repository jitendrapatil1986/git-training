define(['urls','jquery'], function(urls, $) {
    $(function() {
        $('#toDoSelect').change(function() {
            if ($(this).find('option:selected').text() == 'All') {
                $('.todo').removeClass('hide');
            } else {
                var toDoToShow = $(this).find('option:selected').val();
                $('.todo').addClass('hide');
                $('.' + toDoToShow).removeClass('hide');
            }
        });
            
        $(".approve-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Approve;
            executeApproval(url, serviceCallId, $(this));
        });
            
        $(".deny-button").click(function (e) {
            e.preventDefault();
            var serviceCallId = $(this).data("service-call-id");
            var url = urls.ServiceCall.Deny;
            executeApproval(url, serviceCallId, $(this));
        });
            
        function executeApproval(url, serviceCallId, button) {
            $.ajax({
                type: "POST",
                url: url,
                data: { id: serviceCallId },
                success: function (result) {
                    var divToHide = button.parent().parent();
                    divToHide.addClass('hide');
                }
            });
        }
    });
});