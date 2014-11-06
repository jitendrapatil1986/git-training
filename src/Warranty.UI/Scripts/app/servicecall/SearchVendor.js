require(['/Scripts/app/main.js'], function () {
    require(['urls', 'app/search', 'config/vendor-search'], function (urls, search, vendorSearch) {

        search.init({
            searchBarId: '#vendor-search',
            endpoints: [vendorSearch]
        });
    });
});
