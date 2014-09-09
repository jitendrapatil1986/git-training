define(['jquery', 'urls', 'toastr'], function ($, urls, toastr) {
    $(function () {
        var $feedbackForm = $("#feedbackForm");
        var $feedbackModal = $("#feedbackModal");
        var $feedbackInputs = $('.feedbackInput');

        $feedbackForm.submit(function (event) {
            event.preventDefault();
            if (formIsValid()) {
                $.get(urls.Home.SendFeedback + "?" + $feedbackForm.serialize()).
                    done(function(data) {
                        if (data == "error") {
                            toastr.error('There was an error sending your feedback. Please try again.');
                        } else {
                            $feedbackInputs.val("");
                            $feedbackInputs.parents().removeClass('has-error');
                            $feedbackModal.modal('hide');
                            toastr.success('Success! Thank you for your feedback!');
                        }
                    });
            }
        });
        
        function formIsValid() {
            var isValid = true;
            $feedbackForm.find('.required').each(function () {
                var $this = $(this);
                if ($this.val() == '') {
                    $this.parent().addClass('has-error');
                    $this.focus();
                    isValid = false;
                    return;
                }
            });

            return isValid;
        }
    });
});
