define(['urls', 'text!templates/call-search-item.html'], function (urls, template) {
    return {
        display: 'Service Calls',
        key: 'HomeOwnerName',
        itemTemplate: template,
        targetUrl: urls.ServiceCall.CallSummary,
        emptyText: 'No calls found.',
        addOns: [{
            id: 'closed',
            title: 'Include Closed Calls',
            type: 'checkbox',
            queryParam: 'includeInactive'
        }],
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.Calls + '?query=%QUERY'
        })
    };
});
