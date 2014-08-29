define(['app/search', 'config/job-search', 'config/call-search'], function (search, jobSearch, callSearch) {
    var getEndpoints = function (window) {
        if (window && window.location && window.location.pathname && window.location.pathname.indexOf('/ServiceCall/') === 0) {
            return [callSearch, jobSearch];
        } else {
            return [jobSearch, callSearch];
        }
    };

    return search.init({
        searchBarId: '#searchBar',
        endpoints: getEndpoints(window)
    });
});
