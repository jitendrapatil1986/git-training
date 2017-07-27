define(['urls', 'jquery'], function (urls, $) {
    $(function () {
        var size = $('input[name="WidgetSize"]').val();     
        $('.my-service-call:lt(' + size + ')').removeClass('hide');
        $('.my-closed-call:lt(' + size + ')').removeClass('hide');

        $('.show-more-service-calls').click(function () {
            setWidgetSize('.my-service-call', this);
        });

        $('.show-more-closed-calls').click(function () {
            setWidgetSize('.my-closed-call', this);
        });

        function setWidgetSize(className,el) {
            if ($(el).text() == 'All') {
                var defaultWidgetSize = 10000;
                $(className).removeClass('hide');
            } else {
                var show = $(el).text() - 1;
                defaultWidgetSize = $(el).text();
                $(className).removeClass('hide');
                $(className + ':gt("' + show + '")').addClass('hide');
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                url: urls.Home.SetDefaultWidgetSize,
                data: { defaultWidgetSize: defaultWidgetSize },
                error: function () {
                    alert('ajax call failed...');
                }
            });
        }
    });
});



