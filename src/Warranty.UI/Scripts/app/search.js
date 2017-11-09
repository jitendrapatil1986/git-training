define(['require', 'jquery', 'underscore', 'app/searchValidate', 'typeahead', 'bloodhound', 'handlebars'],
    function (require, $, _, validate) {
        Handlebars.registerHelper('if', function(conditional, options) {
            if (conditional) {
                return options.fn(this);
            }
        });
    
        var minCharsToSearch = 2,
            init = function (searchConfig) {
                var searchBar = $(searchConfig.searchBarId),
                    searchingFor = _.first(searchConfig.endpoints).display,
                    addOnMap = [],
                    isPurchaseOrder = (searchConfig.isFromSearchPurchaseOrder !== undefined && searchConfig.isFromSearchPurchaseOrder == true) ? true : false,
                    options = searchConfig;

                var start = function (config) {
                    $('.search-selector').on('click', function () {
                        searchingFor = $(this).text();
                        updateSearchWatermark("Search " + searchingFor, $(this).prop('rel'));
                        setupSearch();
                        // if the text box is not empty, auto search on existing text
                        if (searchBar.val().length > minCharsToSearch) {
                            searchBar.trigger('keyup');
                        }
                    });
                };
                
                var updateSearchWatermark = function (watermark, searchBoxSelector) {
                    var element = (searchBoxSelector && $(searchBoxSelector)) || searchBar;
                    element.val('').attr('placeholder', watermark);
                };

                var getOptions = function () {
                    return _.findWhere(options.endpoints, { display: searchingFor });
                };

                var setupSearch = function () {
                    var config = getOptions(),
                        src = config.engine;
                    
                    searchBar.typeahead('destroy');
                    src.initialize().done(function () {
                        searchBar.typeahead(null, {
                            displayKey: config.key,
                            minLength: minCharsToSearch,
                            source: src.ttAdapter(),
                            highlight: true,
                            templates: {
                                empty: [
                                '<div class="empty-message">',
                                    'no results found',
                                '</div>'
                                ].join('\n'),
                                suggestion: Handlebars.compile(config.itemTemplate),
                            },
                        }).on('typeahead:selected typeahead:autocompleted', function (e, datum) {
                            if (config.target) {
                                config.target(datum);
                            }
                            if (config.targetUrl && !isPurchaseOrder) {
                                window.location = config.targetUrl + '/' + datum.Id;
                            }
                        });
                    });
                };

                if (validate.isValid(searchConfig)) {
                    start(searchConfig);
                    if(getOptions().updateWatermark){
                        updateSearchWatermark("Search " + searchingFor);
                    }
                    setupSearch();
                }
            };

        return {
            init: init
        };
    });
