require.config({
    baseUrl: '/Scripts',
    paths: {
        'jquery': 'lib/jquery-1.10.2.min',
        'jquery.validate': 'lib/jquery.validate.min',
        'jquery.maskedinput': 'lib/jquery.maskedinput.min',
        'jquery.unobtrusive': 'lib/jquery.validate.unobtrusive.min',
        'jquery.numeric': 'lib/jquery.numeric',
        'jquery.color': 'lib/jquery.color-2.1.0.min',
        'bootstrap': 'lib/bootstrap.min',
        'tooltip': 'lib/bootstrap-tooltip',
        'datepicker': 'lib/bootstrap-datepicker',
        'collapse': 'lib/bootstrap-collapse',
        'underscore': 'lib/underscore',
        'toastr': 'lib/toastr.min',
        'moment': 'lib/moment.min',
        'knockout': 'lib/knockout-2.2.1',
        'ko': 'app/customBindings',
        'bootbox': 'lib/bootbox.min',
        'text': 'lib/text',
        'highcharts': 'lib/highcharts',
        'typeahead': 'lib/typeahead.bundle.min',
        'bloodhound': 'lib/bloodhound.min',
        'handlebars': 'lib/handlebars.min',
        'responsive.tabs': 'lib/responsive.tabs',
    },
    shim: {
        'jquery.validate': ['jquery'],
        'jquery.unobtrusive': ['jquery.validate'],
        
        'bootstrap': ['jquery', 'jquery.validate', 'jquery.unobtrusive'],
        'tooltip': { deps: ['bootstrap'], exports: '$.fn.tooltip' },
        'datepicker': { deps: ['bootstrap'], exports: '$.fn.datepicker' },
        'collapse': { deps: ['bootstrap'], exports: '$.fn.collapse' },
        'underscore': { exports: '_' },
        'ko': ['underscore'],
        'highcharts': { deps: ['jquery'], exports: 'Highcharts' },
        'bloodhound': { deps: ['jquery'], exports: 'Bloodhound' },
        'responsive.tabs': { deps: ['bootstrap'] },
    },
    callback: function () {
        require(['app/feedbackForm', 'app/quickSearch', 'app/tooltips', 'app/ajaxEvents', 'app/dates', 'app/formInputs', 'app/toastrNotify', 'app/requiredFields', 'app/responsiveTabs']);
    }
});
