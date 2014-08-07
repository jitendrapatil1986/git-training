define(['jquery', 'urls'], function ($, urls) {
    $(function () {
        var $feedbackForm = $("#feedbackForm");
        $feedbackForm.submit(function (event) {
            event.preventDefault();
            $(".errorSend").hide();
            $.get(urls.Home.SendFeedback + "?" + $feedbackForm.serialize()).
                done(function (data) {
                    if (data == "error") {
                        $(".errorSend").show();
                    } else {
                        $(".modal-body").hide();
                        $("#feedbackSendButton").hide();
                        $(".successfulSend").show();
                    }
                });
        });

        $("#feedbackModal").on('hidden', function () {
            $(".modal-body").show();
            $("#feedbackSendButton").show();
            $(".errorSend").hide();
            $(".successfulSend").hide();
            $(".feedbackInput").val("");
        });
    });
});
