

define(['urls', 'jquery'], function (urls, $) {
    $(function () {
        var size = $('input[name="WidgetSize"]').val();
       
       $('.my-service-call:lt('+size+')').removeClass('hide');

        $('.show-more-service-calls').click(function () {
            if ($(this).text() == 'All') {
                var defaultWidgetSize = 2147483647; //Int32.Maxvalue
                $('.my-service-call').removeClass('hide');
            } else {
                var show = $(this).text() - 1;
                defaultWidgetSize = $(this).text();
                $('.my-service-call').removeClass('hide');
                $('.my-service-call:gt("' + show + '")').addClass('hide');
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                url: urls.Home.SetDefaultWidgetSize,
                //urls.Home.SetDefaultWidgetSize
                data: { defaultWidgetSize: defaultWidgetSize },
                error: function () {
                    alert('ajax call failed...');
                }
            });
        });

        $('.my-closed-call:lt(5)').removeClass('hide');

        $('.show-more-closed-calls').click(function () {
            if ($(this).text() == 'All') {
                $('.my-closed-call').removeClass('hide');
            } else {
                var show = $(this).text() - 1;
                $('.my-closed-call').removeClass('hide');
                $('.my-closed-call:gt("' + show + '")').addClass('hide');
            }
        });
    });
});



