require(['/Scripts/app/main.js'], function() {
    require(['jquery', 'ko', 'datepicker'], function ($, ko) {
        
        $(function () {
            $(".monthpicker-input").datepicker({
                //maxDate: new Date(),
                format: "mm-yyyy",
                viewMode: 'months',
                minViewMode: 'months',
                //beforeShow: function() {
                //    jQuery(this).datepicker('option', 'maxdate', new Date());
                //}
            });
            

        });
    });
});
   