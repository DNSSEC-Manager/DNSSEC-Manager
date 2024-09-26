$('body').on('click', '.js-save-changes', function () {
    $('.js-saving-progress').addClass('active').html('Saving');
    var formData = serializeFormData();
    $.ajax({
        url: '/Settings/SaveChanges',
        type: 'POST',
        data: formData,
        success: function (data) {
            if (data === 'success') {
                $('.js-saving-progress').css('color', '#155724').html('Settings are saved');
            } else {
                $('.js-saving-progress').css('color', '#721c24').html('Something went wrong, try again');
            }
            $('.js-saving-progress').removeClass('active');
        },
        error: function(data) {
            $('.js-saving-progress').css('color', '#721c24').html('Something went wrong, try again');
            $('.js-saving-progress').removeClass('active');
        }
    });
});


function serializeFormData() {
    var returnData = $('#settingsForm').serializeArray().reduce(function (obj, item) {
        obj[item.name] = item.value;
        return obj;
    }, {});
    if (returnData.hasOwnProperty('automaticSign')) {
        returnData.automaticSign = 'true';
    } else {
        returnData.automaticSign = 'false';
    }

    if (returnData.hasOwnProperty('automaticFix')) {
        returnData.automaticFix = 'true';
    } else {
        returnData.automaticFix = 'false';
    }

    if (returnData.hasOwnProperty('automaticKeyRollover')) {
        returnData.automaticKeyRollover = 'true';
    } else {
        returnData.automaticKeyRollover = 'false';
    }

    console.log(returnData);

    return returnData;
}