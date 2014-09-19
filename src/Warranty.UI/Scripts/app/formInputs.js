define(['jquery', 'underscore', 'jquery.numeric', 'jquery.maskedinput', 'bootbox'], function ($, _, numeric) {

    //Globally ignore all number inputs by turning them into text types
    //to prevent the browser from using its sucky native incrementer/decrementer
    var $numberInputs = $('input[type=number]');
    $numberInputs.prop('type', 'text');

    $('.modal')
        .on('shown', function () {
            if ($('input.input-validation-error').length > 0) {
                $('input.input-validation-error:text:visible:first', this).focus();
            } else {
                $('input:text:visible:first', this).focus();
            }
        });

    $('input:text, select, textarea')
        .not(function() {
            return $(this).closest('.modal').length > 0;
        })
        .not('#searchBar, #searchBarMobile')
        .first().focus();

    $('a.btn.submit').on('click', function (e) {
        e.preventDefault();
        $(this).parents('form').submit();
    });
    
    $('.numeric').numeric();

    $('.phone-number').mask('(999)-999-9999');
});
