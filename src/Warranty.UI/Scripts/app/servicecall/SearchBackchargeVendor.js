require(['/Scripts/app/main.js'], function () {
    require(['urls', 'app/search', 'config/backcharge-vendor-search'], function (urls, search, backchargeVendorSearch) {

        search.init({
            searchBarId: '#backcharge-vendor-search',
            endpoints: [backchargeVendorSearch]
        });
    });
});
