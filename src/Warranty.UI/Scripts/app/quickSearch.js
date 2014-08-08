define(['app/search', 'config/job-search', 'config/vendor-search'], function (search, jobSearch, vendorSearch) {
    var getEndpoints = function(window) {
        if (window && window.location && window.location.pathname && window.location.pathname.indexOf('/VendorSummary/') === 0) {
            return [vendorSearch, jobSearch];
        } else {
            return [jobSearch, vendorSearch];
        }
    };

    return search.init({
        searchBarId: '#searchBar',
        endpoints: getEndpoints(window)
    });
});
