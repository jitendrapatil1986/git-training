define(['toastr'], function(toastr) {
    var validate = function (configuration) {
        var isValid, message = '';
        if (!configuration.searchBarId) {
            message += 'Invalid <span class="badge badge-warning">searchBarId</span> property found on configuration.<br />';
        }
        _.each(configuration.endpoints, function (ep) {
            var display = ep.display || "unknown (no display property set)";
            if (!ep.display) {
                message += 'Invalid <span class="badge badge-warning">display</span> property for ' + display + ' search configuration.<br />';
            }
            if (!ep.apiUrl) {
                message += 'Invalid <span class="badge badge-warning">apiUrl</span> property for ' + display + ' search configuration.<br />';
            }
            if (!ep.itemTemplate) {
                message += 'Invalid <span class="badge badge-warning">itemTemplate</span> property for ' + display + ' search configuration.<br />';
            }
            if (!(ep.target || ep.targetUrl)) {
                message += 'Invalid <span class="badge badge-warning">target</span> function or <span class="badge badge-warning">targetUrl</span> property for ' + display + ' search configuration.<br />';
            }
        });
        isValid = message.length == 0;
        if (!isValid) {
            toastr.info(message);
        }
        return isValid;
    };

    return {
        isValid: validate
    };
});
