define(['urls', 'text!templates/call-search-item.html'], function (urls, template) {
    return {
        display: 'Service Calls',
        itemTemplate: template,
        apiUrl: urls.Api.SearchCalls,
        targetUrl: urls.ServiceCall.Index,
        addOns: [{
            id: 'closed',
            title: 'Include Closed Calls',
            type: 'checkbox',
            queryParam: 'includeInactive'
        }]
    };
});
