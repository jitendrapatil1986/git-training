define(['app/search', 'config/job-search'], function (search, jobSearch) {
    var getEndpoints = function (window) {
        return [jobSearch];
    };

    return search.init({
        searchBarId: '#searchBar',
        endpoints: getEndpoints(window)
    });
});
