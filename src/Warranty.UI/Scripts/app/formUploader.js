define(['jquery', 'formUploadData'], function ($, formUploadData) {
    $(function () {

        var cuteUifileUploaders = [];
        var uploadsInProgress = false;
        
        var uploader = document.getElementById(formUploadData.fileUploaderId);

        var form = $('#' + formUploadData.wrapperId).parents('form');
        registerCuteUiFileUploader(uploader, form);
        


        function registerCuteUiFileUploader(uploader, form) {
            cuteUifileUploaders.push(uploader);
            if (form.data('isCuteUiUploadsHandlerRegistered') != true) {
                form.data('isCuteUiUploadsHandlerRegistered', true);
                var submitButtons = form.find('.trigger-file-uploader');

                submitButtons.bind('click', function(event) {
                    event.preventDefault();
                    if (uploadsInProgress) {
                        alert('Your request is already being submitted. Please wait for any file uploads to finish processing ' +
                            ' before leaving this page. Depending on your connection speed this may take up to several minutes.');
                    } else {
                        uploadsInProgress = true;
                        var buttonName = $(this).attr('name');
                        var buttonValue = $(this).val();
                        form.append("<input type='hidden' name='" + buttonName + "' value='" + buttonValue + "' />");

                        handleCuteUiUploads(form);
                    }
                });
            }
        }

        RegisterCuteUiFileUploader = registerCuteUiFileUploader;

        function unregisterAllCuteUiUploaders() {
            cuteUifileUploaders = [];
            $('form').data('isCuteUiUploadsHandlerRegistered', false);
        }

        UnregisterAllCuteUiUploaders = unregisterAllCuteUiUploaders;

        function handleCuteUiUploads(form) {
            handleCuteUiUploader(0, form);
        }

        function handleCuteUiUploader(index, form) {
            if (index >= cuteUifileUploaders.length) {
                uploadsInProgress = false;
                if (form.hasClass('dialog-ajax-form')) {
                    submitAjaxDialogForm(form);
                } else {
                    form.submit();
                }
            }
            var uploader = cuteUifileUploaders[index];
            if (uploader == undefined)
                return;

            var hasFiles = (uploader.getqueuecount() > 0);

            if (hasFiles) {
                uploader.handlepostback = function() {
                    handleCuteUiUploader(index + 1, form);
                };
                uploader.startupload();
            } else {
                handleCuteUiUploader(index + 1, form);
            }
        }

        function submitAjaxDialogForm(form) {
            var action = form.attr("action");
            var formData = form.serialize();
            $.post(action, formData,
                function(data) {
                    if (typeof data == 'string') {
                        form.parents('.ui-dialog-content').html(data);
                    } else if (data.Url) {
                        location.href = data.Url;
                    }
                });
            return false;
        }
    }());

});
