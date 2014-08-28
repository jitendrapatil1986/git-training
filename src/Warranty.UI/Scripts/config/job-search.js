define(['urls', 'text!templates/job-search-item.html'], function (urls, template) {
    return {
        display: 'Jobs',
        itemTemplate: template,
        targetUrl: urls.Job.Index,
        addOns: [{
            id: 'completedAndTerminated2',
            title: 'Include Closed Jobs',
            type: 'checkbox',
            queryParam: 'includeInactive'
        }],
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.Api.SearchJobs + '?query=%QUERY'})
    };
});
