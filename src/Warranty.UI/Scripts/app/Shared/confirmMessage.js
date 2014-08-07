define(['jquery'], function($) {

    var init = function(selector, messageCallback) {

        $(selector).on("click", function (e) {
            var self = $(this),
                confirmMessage = self.data('confirm-message'),
                message = (messageCallback && messageCallback(confirmMessage, self)) || confirmMessage;

            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    };

    return {
        init: init
    }
});
