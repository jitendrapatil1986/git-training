define(['jquery'], function($) {
    $(function() {
        $('#toDoSelect').change(function() {
            if ($(this).find('option:selected').text() == 'All') {
                $('.todo').removeClass('hide');
            } else {
                var toDoToShow = $(this).find('option:selected').val();
                $('.todo').addClass('hide');
                $('.' + toDoToShow).removeClass('hide');
            }
        });
    });
});