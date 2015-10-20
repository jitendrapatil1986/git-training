require(['jquery', 'urls', 'handlebars', 'bootstrap', 'bloodhound', 'typeahead'], function ($, urls) {
    var searchResultsUrl = urls.UserSecurity.SearchUsers;
    var resultMap = {};

    $(document).ready(function () {
        $("#impersonateUserModal-nameInput").typeahead({
            higlight: true,
            hint: false
        },
        {
            name: 'users',
            source: fetchSearchResults,
            async: true,
            templates: {
                empty: [
                    '<div class="empty-message">',
                    'no users matched the query',
                    '</div>'
                ].join('\n'),
                suggestion: Handlebars.compile('<div><strong>{{Caption}}</strong> – {{Title}}</div>')
            }
        });

        $("#impersonateUserModal").on('shown.bs.modal', function () {
            $("#impersonateUserModal-nameInput").focus();
        });

        $("#impersonateUserModal-loadingIndicator").hide();

        $("#impersonateUserModal-nameInput").bind('typeahead:selected', function(obj, datum) {
            relocateToSearchResultView(datum);
        });
    });

    function fetchSearchResults(query, processResultsCallback) {
        return $.ajax({
            url: searchResultsUrl,
            data: { query: query },
            cache: false,
            dataType: "json"
        }).done(function (results) {
            var items = processItems(results);
            if (processResultsCallback)
                return processResultsCallback(items);
            return items;
        });
    }

    function processItems(results) {
        resultMap = {};
        var result =  results.map(function (item) {
            resultMap[item.Caption] = item;
            return item;
        });
        return result;
    }
    
    function relocateToSearchResultView(key) {
        var item = resultMap[key.Caption];
        $("#impersonateUserModal-loadingIndicator").show();
        $("#impersonateUserModal-loadingName").text(item.Caption);
        window.location = item.Url;
    }
});