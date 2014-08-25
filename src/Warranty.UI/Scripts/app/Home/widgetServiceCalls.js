define(['jquery'], function ($) {
    $('.my-service-call:lt(5)').removeClass('hide');

    $('.show-more').click(function () {
        if ($(this).text() == 'All') {
            $('.my-service-call').removeClass('hide');
        } else {
            var show = $(this).text();
            $('.my-service-call:lt("' + show + '")').removeClass('hide');
            $('.my-service-call:gt("' + show + '")').addClass('hide');
        }
    });
});
