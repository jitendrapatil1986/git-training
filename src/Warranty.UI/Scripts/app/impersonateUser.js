require(['jquery', 'underscore', 'urls', 'bootstrap', 'bloodhound', 'typeahead'], function ($, _, urls) {
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
            highlighter: generateSearchResultRow
        });

        $("#impersonateUserModal").on('shown.bs.modal', function () {
            $("#impersonateUserModal-nameInput").focus();
        });

        $("#impersonateUserModal-loadingIndicator").hide();

        $("#impersonateUserModal-nameInput").bind('typeahead:selected', function(obj, datum, name) {
            relocateToSearchResultView(datum.value);
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
            var key = "<div><span>" + item.Caption + "</span><br /><span>" + item.Title + "</span></div>";
            resultMap[key] = item;
            return { value: key };
        });
        return result;
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