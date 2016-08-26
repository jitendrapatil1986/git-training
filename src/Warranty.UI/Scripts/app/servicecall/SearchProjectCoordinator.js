require(['/Scripts/app/main.js'], function () {
    require(['urls', 'app/search', 'config/project-coordinator-search'], function (urls, search, projectCoordinatorSearch) {

        search.init({
            searchBarId: '#pc-search',
            endpoints: [projectCoordinatorSearch]
        });
    });
});
