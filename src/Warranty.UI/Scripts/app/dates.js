define(['jquery', 'datepicker', 'bootstrap'], function ($) {

    $(".datepicker-input").datepicker({
        format: 'm/d/yyyy'
    });

    $(document).on('changeDate', '.datepicker-input', function (e) {
        $(e.currentTarget).datepicker('hide');
    });

    $(document).on('click', '.datepicker-button', function (e) {
        e.preventDefault();
        $('input', e.currentTarget.parentElement).datepicker('show');
    });

    $(".monthpicker-input").datepicker({
        format: "mm-yyyy",
        viewMode: 'months',
        minViewMode: 'months'
    });

    $(document).on('changeDate', '.monthpicker-input', function (e) {
        $(e.currentTarget).datepicker('hide');
    });
});
