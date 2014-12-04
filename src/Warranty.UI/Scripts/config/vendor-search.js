﻿define(['urls', 'text!templates/vendor-search-item.html', 'bloodhound'], function (urls, template, bloodhound) {
    return {
        display: 'Vendors',
        key: 'VendorName',
        itemTemplate: template,
        target: function (item) {
            $('#vendor-search').attr('data-vendor-number', item.VendorNumber);
            $('#vendor-search').attr('data-vendor-name', item.VendorName);
            $(document).trigger('vendor-number-selected');
        },
        emptyText: 'No vendors found.',
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.Vendors + '?query=%QUERY&cityCode=' + $('#vendor-search').attr('data-city-code')
        })
    };
});
