define(['jquery'], function ($) {
    $('.my-service-call:lt(5)').removeClass('hide');

    $('.show-more-service-calls').click(function () {
        if ($(this).text() == 'All') {
            $('.my-service-call').removeClass('hide');
        } else {
            var show = $(this).text() - 1;
            $('.my-service-call').removeClass('hide');
            $('.my-service-call:gt("' + show + '")').addClass('hide');
        }
    });
});
