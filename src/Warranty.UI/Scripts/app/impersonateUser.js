require(['jquery', 'underscore', 'urls', 'bootstrap'], function ($, _, urls) {
    var searchResultsUrl = urls.UserSecurity.SearchUsers;
    var resultMap = {};
    var searchResultTemplate = _.template(
        '<div class="typeahead impersonateUser-searchResult">' +
        '<%= Caption %>' +
        '<div class="title"><%= Title %></div>' +
        '</div>');

    $(document).ready(function () {
        $("#impersonateUserModal-nameInput").typeahead({
            higlight: true
        },
        {
            name: 'users',
            source: fetchSearchResults,
            async: true,
            matcher: matchSearchResult,
            highlighter: generateSearchResultRow,
            updater: relocateToSearchResultView
        });

        $("#impersonateUserModal").on('shown', function () {
            $("#impersonateUserModal-nameInput").focus();
        });

        $("#impersonateUserModal-loadingIndicator").hide();
    });

    function fetchSearchResults(query, undefined, processResultsCallback) {
        $.ajax({
            url: searchResultsUrl,
            data: { query: query },
            cache: false,
            dataType: "json"
        }).done(function (results) {
            var items = processItems(results);
            if (processResultsCallback)
                processResultsCallback(items);
        });
    }

    function processItems(results) {
        resultMap = {};
        return results.map(function (item) {
            var key = item.Caption;
            resultMap[key] = item;
            return key;
        });
    }

    function matchSearchResult(key) {
        // matching is being done server side, so every result should be a match
        return true;
    }

    function generateSearchResultRow(key) {
        var item = resultMap[key];
        return searchResultTemplate(item);
    }

    function relocateToSearchResultView(key) {
        var item = resultMap[key];

        $("#impersonateUserModal-loadingIndicator").show();
        $("#impersonateUserModal-loadingName").text(item.Caption);

        window.location = item.Url;
    }
});