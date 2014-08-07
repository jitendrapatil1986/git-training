define(['jquery', 'tooltip'], function ($) {

    var init = function() {
        $('.has-legend-tooltip').tooltip({
            placement: 'bottom',
            trigger: 'hover focus',
            delay: { show: 200, hide: 200 },
            html: true
        });

        $('.has-tooltip').tooltip({
            placement: 'right',
            trigger: 'focus',
            delay: { show: 700, hide: 200 }
        });

        $('.has-bottom-tooltip').tooltip({
            placement: 'bottom',
            trigger: 'hover',
            delay: { show: 200, hide: 100 }
        });
    };

    init();

    return {
        init: init
    };
});
