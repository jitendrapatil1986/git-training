define(['require', 'jquery', 'underscore', 'app/searchValidate', 'typeahead', 'bloodhound'],
    function (require, $, _, validate) {
        var minCharsToSearch = 2,
            init = function (searchConfig) {

                var searchBar = $(searchConfig.searchBarId),
                    searchingFor = _.first(searchConfig.endpoints).display,
                    addOnMap = [],
                    options = searchConfig;

                var start = function (config) {

                    buildSearchTypes(config.endpoints);
                    buildAddOns(config.endpoints);

                    $('.search-selector').on('click', function () {
                        searchingFor = $(this).text();
                        updateSearchWatermark("Search " + searchingFor, $(this).prop('rel'));
                        updateAddOns(addOnMap, searchingFor);

                        // if the text box is not empty, auto search on existing text
                        if (searchBar.val().length > minCharsToSearch) {
                            searchBar.trigger('keyup');
                        }
                    });
                };

                var updateAddOns = function (addOns, name) {
                    _.each(addOns, function (ao) {
                        var $item = $(ao.item);
                        $item.prop('disabled', true);
                        $item.removeProp('checked');
                    });
                    var addOn = _.findWhere(addOns, { name: name });
                    addOn && $(addOn.item).prop("disabled", false);
                };

                var updateSearchWatermark = function (watermark, searchBoxSelector) {
                    var element = (searchBoxSelector && $(searchBoxSelector)) || searchBar;
                    element.val('').attr('placeholder', watermark);
                };

                var buildSearchTypes = function (endpoints) {

                    var searchTypeGroup = searchBar.next();
                    if (endpoints.length == 1) {
                        searchTypeGroup.remove();
                    } else {
                        var list = $('ul', searchTypeGroup);
                        _.each(endpoints, function (ep) {
                            list.append('<li><a class="search-selector" rel="' + searchConfig.searchBarId + '" href="#">' + ep.display + '</a></li>');
                        });
                    }
                };

                var buildAddOns = function (endpoints) {
                    var addOnContainer = searchBar.prev();
                    _.each(endpoints, function (ep) {
                        _.each(ep.addOns, function (ao) {
                            var value = "";
                            if (ao.value) {
                                value = 'value="' + ao.value + '"';
                            }
                            var items = addOnContainer.append('<input id="' + ao.id + '" class="has-bottom-tooltip" title="' + ao.title + '" type="' + ao.type + '"' + value + '/>')[0];
                            var item = $(':last', items)[0];
                            addOnMap.push({ name: ep.display, addOn: ao, item: item });
                        });
                    });
                };

                var getOptions = function () {
                    return _.findWhere(options.endpoints, { display: searchingFor });
                };

                var setupSearch = function () {
                    var src = getOptions().engine;
                        
                    src.initialize().done(function() {
                        searchBar.typeahead(null, {
                            minLength: minCharsToSearch,
                            source: src.ttAdapter(),
                            highlight: true,
                        });
                    });
                };

                if (validate.isValid(searchConfig)) {
                    start(searchConfig);
                    updateSearchWatermark("Search " + searchingFor);
                    setupSearch();
                }
            };

        return {
            init: init
        };
    });
