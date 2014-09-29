require(['/Scripts/app/main.js'], function () {
    require(['jquery', 'x-editable', 'enumerations/phone-number-type', 'jquery.maskedinput'], function ($, xeditable, phoneNumberTypeEnum) {
        $(function () {
            $.fn.editable.defaults.mode = 'inline';
            $.fn.editable.defaults.emptytext = 'Add';
            $.fn.editableform.buttons =
                '<button type="submit" class="btn btn-primary editable-submit btn-xs"><i class="icon-ok icon-white"></i>Save</button>';


            $("#Employee_List").editable({
                type: 'select',
            });

            $("#Home_Phone").editable({
                params: { phoneNumberTypeValue: phoneNumberTypeEnum.Home.Value }
            });

            $("#Mobile_Phone").editable({
                params: { phoneNumberTypeValue: phoneNumberTypeEnum.Mobile.Value }
            });

            $("#Email").editable({
            });

            $(".phone-number-with-extension").on('shown', function () {
                $(this).data('editable').input.$input.mask('?(999)-999-9999 **********', { placeholder: " " });
            });

        });
    });
});