define(['require', 'jquery', 'underscore', 'app/searchValidate', 'bootstrap', 'bootbox'],
    function (require, $, _, validate) {

        var minCharsToSearch = 2,
            currentSearchText = '',
            init = function (searchConfig) {

            var searchBar = $(searchConfig.searchBarId),
                searchingFor = _.first(searchConfig.endpoints).display,
                addOnMap = [],
                options = searchConfig;

            var start = function(config) {

                buildSearchTypes(config.endpoints);
                buildAddOns(config.endpoints);

                $('.search-selector').on('click', function() {
                    searchingFor = $(this).text();
                    updateSearchWatermark("Search " + searchingFor, $(this).prop('rel'));
                    updateAddOns(addOnMap, searchingFor);

                    // if the text box is not empty, auto search on existing text
                    if (searchBar.val().length > minCharsToSearch) {
                        searchBar.trigger('keyup');
                    }
                });
            };

            var updateAddOns = function(addOns, name) {
                _.each(addOns, function(ao) {
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

            var buildSearchTypes = function(endpoints) {

                var searchTypeGroup = searchBar.next();
                if (endpoints.length == 1) {
                    searchTypeGroup.remove();
                } else {
                    var list = $('ul', searchTypeGroup);
                    _.each(endpoints, function(ep) {
                        list.append('<li><a class="search-selector" rel="' + searchConfig.searchBarId + '" href="#">' + ep.display + '</a></li>');
                    });
                }
            };

            var buildAddOns = function(endpoints) {
                var addOnContainer = searchBar.prev();
                _.each(endpoints, function(ep) {
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

            var withBoundProperties = function (result, item) {
                for (var name in result) {
                    if (result.hasOwnProperty(name)) {
                        item.find('.si-' + name).text(result[name]);
                    }
                }
                return item;
            };

            var defaultTarget = function (item, opts) {
                if (item.id > 0) {
                    window.location = opts.targetUrl + '/' + item.id;
                }
            };

            var getOptions = function() {
                return _.findWhere(options.endpoints, { display: searchingFor });
            };

            var setupSearch = function () {
                var src = _.debounce(function (query, process) {
                    currentSearchText = query;

                    var self = this,
                        searchOptions = getOptions(),
                        extraData = {};

                    _.each(searchOptions.addOns, function(ao) {
                        var aoData = {};
                        ao.type == 'checkbox' && (aoData[ao.queryParam] = $('#' + ao.id).prop('checked'));
                        ao.type == 'hidden' && (aoData[ao.queryParam] = ao.value);
                        _.extend(extraData, aoData);
                    });

                    return $.ajax({
                        url: searchOptions.apiUrl,
                        cache: false,
                        data: _.extend({ query: query }, extraData),
                        dataType: 'json',
                    }).fail(function(jqXhr, textStatus, errorThrown) {
                        var valueHasBeenSelected = !!self.selectedValue; // "not not" turns a truthy/falsey value into an explicit boolean value
                        if (!self.isShowingConfirmWindow && !valueHasBeenSelected) {
                            self.isShowingConfirmWindow = true;
                            bootbox.confirm(errorThrown + " (Your session may have timed out)." + "<br/><br/><p>Click <strong>OK</strong> to refresh this page.</p>", function(answer) {
                                self.isShowingConfirmWindow = false;
                                if (answer) {
                                    location.reload(true);
                                }
                            });
                        }
                    }).done(function(response) {
                        // Note: Position is used here because bootstrap-typeahead will display the results "under" the target
                        //       element. In this case, that element is the "searchBar" variable. If the user has already chosen
                        //       an item from the results, but there are pending requests coming back from the server. The menu
                        //       will be re-displayed below the target element. Since the user has selected an element, our code
                        //       has hidden this element, resulting in a position of { top: 0, left: 0 }, and placing the menu
                        //       in the top left corner of the page. This check will disregard any results if the searchBar has
                        //       been hidden due to user selection.
                        var position = searchBar.position();
                        if (position.top === 0 && position.left === 0)
                            return null;

                        if (response.Query !== currentSearchText)
                            return null;

                        var hasResults = response && response.TotalMatches && response.TotalMatches > 0;
                        if (!hasResults)
                            return process([JSON.stringify({ id: 0 })]);
                        var items = $.map(response.Data, function(value, key) {
                            return value;
                        });
                        self.selectedValue = items;
                        return process(items);
                    });
                }, 250);
                searchBar.typeahead({
                    minLength: minCharsToSearch,
                    selectedValue: null,
                    isShowingConfirmWindow: false,
                    source: src,
                    matcher: function (obj) {
                        var item = JSON.parse(obj);
                        return !(item.id < 0);
                    },
                    updater: function (item) {
                        var selectedItem = JSON.parse(item);
                        if (selectedItem.id != 0) {
                            var opts = getOptions(),
                                target = opts.target || defaultTarget;
                            target(selectedItem, opts);
                        }
                    },

                    highlighter: function(searchItem) {
                        var selectedOptions = getOptions(),
                            selectedItem = JSON.parse(searchItem),
                            item = $(selectedOptions.itemTemplate);

                        if (searchItem && selectedItem.id == 0)
                            return '<div><div class="typeahead"><span>No ' + selectedOptions.display + ' found</span></div></div>';

                        selectedOptions.beforeBind && selectedOptions.beforeBind(selectedItem, item);
                        return withBoundProperties(selectedItem, item).html();
                    }
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
