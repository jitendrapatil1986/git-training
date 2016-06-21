require(['/Scripts/app/main.js'], function() {
    require(['jquery'], function ($) {
        $(function() {
            $("#expandAll").click(function() {
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-right");
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-down");

                if ($(this).children(".glyphicon").hasClass("glyphicon-chevron-down")) {
                    $(this).children(".text").text("Hide All");
                    $(".panel-body").removeClass("hidden");
                    $(".panel").removeClass("no-print");
                    $(".activityToggle").removeClass("glyphicon-chevron-right");
                    $(".activityToggle").addClass("glyphicon-chevron-down");
                } else {
                    $(this).children(".text").text("Expand All");
                    $(".panel-body").addClass("hidden");
                    $(".panel").addClass("no-print");
                    $(".activityToggle").removeClass("glyphicon-chevron-down");
                    $(".activityToggle").addClass("glyphicon-chevron-right");
                }
            });

            $(".panel-heading").click(function () {
                $(this).children(".activityToggle").toggleClass("glyphicon-chevron-right");
                $(this).children(".activityToggle").toggleClass("glyphicon-chevron-down");
                $(this).closest(".panel").find(".panel-body").toggleClass("hidden");
                $(this).closest(".panel").toggleClass("hidden-print");
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