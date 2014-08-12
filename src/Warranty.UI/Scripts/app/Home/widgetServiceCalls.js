define(['jquery'], function ($) {
    $('.service-call-line:lt(5)').removeClass('hide');

    $('.show-more').click(function () {
        if ($(this).text() == 'All') {
            $('.service-call-line').removeClass('hide');
        } else {
            var show = $(this).text();
            $('.service-call-line:lt("' + show + '")').removeClass('hide');
            $('.service-call-line:gt("' + show + '")').addClass('hide');
        }
    });
});
