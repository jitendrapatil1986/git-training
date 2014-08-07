define(['jquery'], function ($) {
    $(document).ajaxStart(function () {
        $("#loadingIndicator").show();
    });

    $(document).ajaxError(function (e, s) {
        //Refresh page when authentication timeout during ajax call
        if(s.statusText == "error" && s.status == 0) //would expect 401 status code, but only get jquery's error status when timeout occurs
                location.reload();
    });

    $(document).ajaxComplete(function () {
        $("#loadingIndicator").hide();
    });
});
