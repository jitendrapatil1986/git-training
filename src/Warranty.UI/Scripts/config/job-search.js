define(['urls', 'text!templates/job-search-item.html'], function (urls, template) {
    return {
        display: 'Jobs',
        key: 'HomeOwnerName',
        itemTemplate: template,
        targetUrl: urls.Job.JobSummary,
        emptyText: 'No jobs found.',
        addOns: [{
            id: 'completedAndTerminated2',
            title: 'Include Closed Jobs',
            type: 'checkbox',
            queryParam: 'includeInactive'
        }],
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.Jobs + '?query=%QUERY'
        })
    };
});
