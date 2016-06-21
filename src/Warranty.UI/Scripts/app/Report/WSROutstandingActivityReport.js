require(['/Scripts/app/main.js'], function() {
    require(['jquery'], function ($) {
        $(function() {
            $("#expandAll").click(function() {
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-right");
                $(this).children(".glyphicon").toggleClass("glyphicon-chevron-down");

                if ($(this).children(".glyphicon").hasClass("glyphicon-chevron-down")) {
                    $(this).children(".text").text("Hide All");
                    $(".panel-body").removeClass("hidden");
                    $(".panel").removeClass("hidden-print");
                    $(".activityToggle").removeClass("glyphicon-chevron-right");
                    $(".activityToggle").addClass("glyphicon-chevron-down");
                } else {
                    $(this).children(".text").text("Expand All");
                    $(".panel-body").addClass("hidden");
                    $(".panel").addClass("hidden-print");
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
        });
    });
});