define(['jquery','datepicker','bootstrap'], function($, datepicker, bootstrap) {
    $(function () {
        $('.required-field').each(function() {
            if (!$(this).val()) {
                $(this).css('border-color', '#b94a48');
            }
        });

        $('.required-field').on('blur change', function () {
            if ($(this).val()) {
                $(this).css('border-color', '#ccc');
            } else {
                $(this).css('border-color', '#b94a48');
            }
        });
    });
});
