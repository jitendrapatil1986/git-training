define(['urls', 'text!templates/job-search-item.html', 'bloodhound'], function (urls, template, bloodhound) {
    return {
        display: 'Jobs',
        key: 'HomeOwnerName',
        itemTemplate: template,
        targetUrl: urls.Job.JobSummary,
        emptyText: 'No jobs found.',
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.Jobs + '?query=%QUERY'
        })
    };
});
