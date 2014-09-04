require(['/Scripts/app/main.js'], function () {
    require(['urls', 'app/search', 'config/customer-search'], function (urls, search, customerSearch) {

        search.init({
            searchBarId: '#customer-search',
            endpoints: [customerSearch]
        });
    });
});
