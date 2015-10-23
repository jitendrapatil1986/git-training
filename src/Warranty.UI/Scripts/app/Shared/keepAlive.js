define(['jquery', 'bootbox', 'urls'], function ($, bootbox, urls) {
    var previousPingTime;
    var minimumPingIntervalInMinutes = 1;//every 1 minute
    var promptToContinueTimout;
    var oneMinuteInMilliSeconds = 60000;
    var sessionTrackerContainer = document;
    var sessionTrackingEvents = "click.keepAlive keyup.keepAlive mousemove.keepAlive scroll.keepAlive touchend.keepAlive";

    //setListener();


    function setListener() {
        previousPingTime = new Date(1);
        makeRequest();
        $(sessionTrackerContainer).on(sessionTrackingEvents, function () {
            makeRequest();
        });
    }

    function makeRequest() {
        var now = new Date();
        if (now >= new Date(previousPingTime.getTime() + (minimumPingIntervalInMinutes * oneMinuteInMilliSeconds))) {
            previousPingTime = now;

            if (promptToContinueTimout)
                clearTimeout(promptToContinueTimout);
            $.ajax({
                type: 'GET',
                url: urls.UserSession.KeepAlive,
                cache: false,
                success: function (response) {
                    var tokenExpirationTime = new Date(response);

                    if (isNaN(tokenExpirationTime)) {
                        tokenExpirationTime = new Date(Date.parse(response));
                    }

                    if (tokenExpirationTime <= now) {   /* Should trigger if the session wasn't renewed in time. 
                                                        Note: this "now" is not a true now. Round trip to server makes this outdated, 
                                                        but for calculations purposes, this should work.*/
                        expireSession();
                        return;
                    }

                    var promptCountDown = (2 * oneMinuteInMilliSeconds); //show countdown for 2 minutes. But prompt 3 minutes before actual expiration
                    var whenToShowPrompt = tokenExpirationTime.getTime() - promptCountDown - oneMinuteInMilliSeconds; //promptCountDown + (oneMinuteInMilliSeconds) = 3 minutes //3 minutes before expiration
                    var milliSecondsUntilPromptAppears = whenToShowPrompt - now.getTime(); //promptCountDown + (oneMinuteInMilliSeconds) = 3 minutes

                    promptToContinueTimout = setTimeout(function () {
                        $(sessionTrackerContainer).off(sessionTrackingEvents);

                        if (tokenExpirationTime <= new Date()) { //if session expires overnight, this prevents the countdown from showing
                            expireSession();
                            return;
                        }

                        var interval;
                        bootbox.confirm("Your session will expire in <span id='session-timeout-minutes'>" + formatMinutes(new Date(promptCountDown)) + "</span> minutes. Click OK to continue. Click Cancel to let it expire.",
                            function (result) {
                                if (result) {
                                    setListener();
                                    clearInterval(interval);
                                }
                            });
                        interval = countDown(promptCountDown, tokenExpirationTime);
                    }, milliSecondsUntilPromptAppears);
                }
            });
        }
    }

    function countDown(time, tokenExpirationTime) {
        var interval = setInterval(function () {
            if (tokenExpirationTime <= new Date()) {    //this is so that if the user hibernates their machine during the countdown, 
                                                        //and resumes later, the countdown will stop and expire the session, 
                                                        //if the token has expired
                clearInterval(interval);
                expireSession();
                return;
            }

            var date = new Date(time);
            $("#session-timeout-minutes").text(formatMinutes(date));
            time = time - 1000;
            if (time < 0) {
                clearInterval(interval);
                expireSession();
            }
        }, 1000);
        return interval;
    }

    function expireSession() {
        $(sessionTrackerContainer).off(sessionTrackingEvents);
        bootbox.hideAll();
        bootbox.confirm("Your session has expired. Click OK to refresh the page", function (result) {
            if (result) {
                window.location = urls.Help.SignOut + "?redirect=" + window.location.pathname;
            }
        });
    }

    function formatMinutes(date) {
        return date.getMinutes() + ":" + ("0" + date.getSeconds()).slice(-2);
    }
});