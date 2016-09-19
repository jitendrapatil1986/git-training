define(['urls', 'text!templates/project-coordinator-search-item.html', 'bloodhound'], function (urls, template, bloodhound) {
    return {
        display: 'ProjectCoordinators',
        key: 'Name',
        itemTemplate: template,
        target: function (item) {
            $('#pc-search').attr('data-pc-email', item.Email);
            $('#pc-search').attr('data-pc-name', item.Name);
            $(document).trigger('pc-selected');
        },
        emptyText: 'No PCs found.',
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.ProjectCoordinators + '?query=%QUERY'
        })
    };
});
