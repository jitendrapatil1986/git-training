require(['/Scripts/app/main.js'], function() {
    require(['jquery', 'ko', 'moment'], function ($, ko, moment) {
        $(function () {
            var currentDate = new Date();

            $(".monthpicker-input").datepicker({
                maxDate: '0',
                format: "mm-yyyy",
                viewMode: 'months',
                minViewMode: 'months',
            });

            if (!$("#warrantyBonusSummaryDate").val()) {
                $("#warrantyBonusSummaryDate").val(moment(currentDate).format('MM-YYYY'));
            }
        });
        
    });
});
   