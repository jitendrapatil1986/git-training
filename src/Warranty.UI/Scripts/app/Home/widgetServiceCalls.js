//Passing model to the js file.
//define(['jquery', 'highcharts', 'modeldata'], function($) {

//});

define(['jquery', 'highcharts'], function ($) {
    $('.service-call-line:lt(5)').removeClass('hide');

    $('.show-more').click(function () {
        if ($(this).text() == 'All') {
            $('.service-call-line').removeClass('hide');
        } else {
            var show = $(this).text();
            $('.service-call-line:lt("' + show + '")').removeClass('hide');
            $('.service-call-line:gt("' + show + '")').addClass('hide');
        }
    });
    
    //$('#my_service_chart').highcharts({
    //    title: {
    //        text: 'My Team',
    //        x: 0 //center
    //    },
    //    //subtitle: {
    //    //    text: '',
    //    //    x: -20
    //    //},
    //    xAxis: {
    //        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
    //            'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    //    },
    //    yAxis: {
    //        title: {
    //            text: 'Warranty Dollars (Thousands)'
    //        },
    //        min: 0,
    //        plotLines: [{
    //            value: 0,
    //            width: 1,
    //            color: '#808080'
    //        }]
    //    },
    //    tooltip: {
    //        valueSuffix: 'K'
    //    },
    //    legend: {
    //        layout: 'vertical',
    //        align: 'right',
    //        verticalAlign: 'middle',
    //        borderWidth: 0
    //    },
    //    series: [{
    //        name: 'US',
    //        //data: [31.0, 45.9, 38.5, 37.5, 49.2, 40.0, 39.2, 5.5, 0.0, 0.0, 0.0, 0.0]
    //        data: [31.0, 45.9, 38.5, 37.5, 49.2, 40.0, 39.2, 5.5, 0.0, 0.0, 0.0, 0.0]
    //    }]
    //});
});
