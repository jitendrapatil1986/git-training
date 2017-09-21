require(['/Scripts/app/main.js'], function() {
    require(['jquery'], function ($) {
        $(function() {

            $("#expandAll").click(function () {
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-right");
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-down");

                if ($(this).children(".glyphicon").hasClass("glyphicon-chevron-down")) {
                    $(this).children(".text").text("Hide All");
                    $(".panel-body").removeClass("hidden");
                    $("#expandAllCalls").removeClass("hidden");
                    $(".employeeContainer").removeClass("no-print");
                    $(".activityToggle").removeClass("glyphicon-chevron-right");
                    $(".activityToggle").addClass("glyphicon-chevron-down");
                } else {
                    $(this).children(".text").text("Expand All");
                    $(".panel-body").addClass("hidden");
                    $("#expandAllCalls").addClass("hidden");
                    $(".employeeContainer").addClass("no-print");
                    $(".activityToggle").removeClass("glyphicon-chevron-down");
                    $(".activityToggle").addClass("glyphicon-chevron-right");
                }
            });

            $(".panel-heading").click(function () {
                $(this).children(".activityToggle").toggleClass("glyphicon-chevron-right");
                $(this).children(".activityToggle").toggleClass("glyphicon-chevron-down");
                $(this).closest(".employeeContainer").find(".panel-body").toggleClass("hidden");                
                $(this).closest(".employeeContainer").toggleClass("no-print");
            });

            $('.line-item-display').click(function () {
                if ($(this).hasClass("collapsed")) {
                    $(this).nextUntil('tr.line-item-display')                    
                    .find('td > div')
                    .slideDown("fast", function () {
                        var $set = $(this);
                        $set.replaceWith($set.contents());
                    });
                    $(this).removeClass("collapsed");
                } else {
                    $(this).nextUntil('tr.line-item-display')
                    .find('td')
                    .wrapInner('<div/>')
                    .parent()
                    .find('td > div')
                    .slideUp("fast");
                    $(this).addClass("collapsed");
                }
            });

            $('#expandAllCalls').click(function () {
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-right");
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-down");
                if ($(this).children(".glyphicon").hasClass("glyphicon-chevron-down")) {
                    $(this).children(".text").text("Hide All Calls");                  
                    $('div.edit').removeClass("edit");                  
                } else {
                    $(this).children(".text").text("Expand All Calls");                  
                    $('tr#collapseRow').find('td > div').addClass("edit"); 
                }
            });

            $("#notesToggle").click(function () {
                $(this).toggleClass("active");
                $(this).blur();
                $(".notes-container").toggleClass("hidden");
                $(".notes-container").toggleClass("no-print");
                
                if ($(this).hasClass("active"))
                    $(this).children(".text").text("Exclude Notes");
                else 
                    $(this).children(".text").text("Include Notes");
            });

            $("#printButton").click(function() {
                window.print();
            });
        });
    });
});