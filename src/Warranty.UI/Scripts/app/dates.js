define(['jquery', 'datepicker', 'bootstrap'], function ($) {

    $(".datepicker-input").datepicker({
        format: 'm/d/yyyy',
        autoclose: true
    });

    $(document).on('click', '.datepicker-button', function (e) {
        e.preventDefault();
        $('input', e.currentTarget.parentElement).datepicker('show');
    });

    $(".monthpicker-input").datepicker({
        format: "mm-yyyy",
        viewMode: 'months',
        minViewMode: 'months',
        autoclose: true
    });

});
