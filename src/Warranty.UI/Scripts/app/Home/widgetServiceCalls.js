define(['jquery'], function ($) {
    $('.service-call-line:lt(4)').removeClass('hide');

    $('.show-more').click(function () {
        if ($(this).text() == 'All') {
            $('.service-call-line').removeClass('hide');
        } else {
            var show = $(this).text() - 1;
            $('.service-call-line:lt("' + show + '")').removeClass('hide');
            $('.service-call-line:gt("' + show + '")').addClass('hide');
        }
    });
});
