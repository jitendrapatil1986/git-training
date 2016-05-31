require(['/Scripts/app/main.js'], function() {
    require(['jquery', 'ko', 'moment'], function ($, ko, moment) {
        $(function() {
            $("#expandAll").click(function() {
                $(this).children(".glyphicon").toggleClass("glyphicon-plus");
                $(this).children(".glyphicon").toggleClass("glyphicon-minus");

                if ($(this).children(".glyphicon").hasClass("glyphicon-minus")) {
                    $(this).children(".text").text("Hide All");
                    $(".activityContent").removeClass("hidden");
                    $(".activityToggle").children(".glyphicon").removeClass("glyphicon-plus");
                    $(".activityToggle").children(".glyphicon").addClass("glyphicon-minus");
                } else {
                    $(this).children(".text").text("Expand All");
                    $(".activityContent").addClass("hidden");
                    $(".activityToggle").children(".glyphicon").removeClass("glyphicon-minus");
                    $(".activityToggle").children(".glyphicon").addClass("glyphicon-plus");
                }
            });

            $(".activityToggle").click(function () {
                $(this).children(".glyphicon").toggleClass("glyphicon-plus");
                $(this).children(".glyphicon").toggleClass("glyphicon-minus");
                $(this).closest(".panel").find("div.activityContent").toggleClass("hidden");
            });
        });
    });
});