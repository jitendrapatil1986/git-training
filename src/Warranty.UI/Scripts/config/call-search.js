define(['urls', 'text!templates/call-search-item.html', 'bloodhound'], function (urls, template, bloodhound) {
    return {
        display: 'Service Calls',
        key: 'HomeOwnerName',
        updateWatermark: true,
        itemTemplate: template,
        targetUrl: urls.ServiceCall.CallSummary,
        emptyText: 'No calls found.',
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.Calls + '?query=%QUERY'
        })
    };
});
