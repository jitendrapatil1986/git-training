﻿define(['urls', 'text!templates/customer-search-item.html'], function (urls, template) {
    return {
        display: 'Customers',
        key: 'HomeOwnerName',
        itemTemplate: template,
        targetUrl: urls.ServiceCall.VerifyCustomer,
        emptyText: 'No customers found.',
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.Customers + '?query=%QUERY'
        })
    };
});
