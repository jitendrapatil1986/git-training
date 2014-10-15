require(['/Scripts/app/main.js'], function() {
    require(['jquery', 'ko', 'datepicker'], function ($, ko) {
        
        $(function () {
            var currentDate = new Date();
            
            $(".monthpicker-input").datepicker({
                //maxDate: new Date(),
                format: "mm-yyyy",
                viewMode: 'months',
                minViewMode: 'months',
                defaultDate: new Date(),
                //beforeShow: function() {
                //    jQuery(this).datepicker('option', 'maxdate', new Date());
                //}
            });

            //$(".monthpicker-input").datepicker("setDate", currentDate);
            $("#warrantyBonusSummaryDate").val(currentDate);
        });
        
    });
});
   