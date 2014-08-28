//define(['app/search', 'config/job-search', 'config/call-search'], function (search, jobSearch, callSearch) {
define(['app/search', 'config/job-search'], function (search, jobSearch) {
    var getEndpoints = function (window) {
        //if (window && window.location && window.location.pathname && window.location.pathname.indexOf('/CallSummary/') === 0) {
        //    return [callSearch, jobSearch];
        //} else {
        //    return [jobSearch, callSearch];
        //}
        return [jobSearch];
    };

    return search.init({
        searchBarId: '#searchBar',
        endpoints: getEndpoints(window)
    });
});
