define(['urls', 'text!templates/backcharge-vendor-search-item.html', 'bloodhound'], function (urls, template, bloodhound) {
    return {
        display: 'Vendors',
        key: 'VendorName',
        itemTemplate: template,
        target: function (item) {
            $('#backcharge-vendor-search').attr('data-vendor-number', item.VendorNumber);
            $('#backcharge-vendor-search').attr('data-vendor-name', item.VendorName);
            $('#backcharge-vendor-search').attr('data-vendor-on-hold', item.VendorOnHold);
            $(document).trigger('backcharge-vendor-number-selected');
        },
        emptyText: 'No vendors found.',
        engine: new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: urls.QuickSearch.Vendors + '?query=%QUERY&cityCode=' + $('#backcharge-vendor-search').attr('data-city-code')
        })
    };
});
