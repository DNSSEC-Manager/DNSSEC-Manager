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
//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIlNldHRpbmdzLmpzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EiLCJmaWxlIjoiU2V0dGluZ3MuanMiLCJzb3VyY2VzQ29udGVudCI6WyIkKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1zYXZlLWNoYW5nZXMnLCBmdW5jdGlvbiAoKSB7XHJcbiAgICAkKCcuanMtc2F2aW5nLXByb2dyZXNzJykuYWRkQ2xhc3MoJ2FjdGl2ZScpLmh0bWwoJ1NhdmluZycpO1xyXG4gICAgdmFyIGZvcm1EYXRhID0gc2VyaWFsaXplRm9ybURhdGEoKTtcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL1NldHRpbmdzL1NhdmVDaGFuZ2VzJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YTogZm9ybURhdGEsXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLXNhdmluZy1wcm9ncmVzcycpLmNzcygnY29sb3InLCAnIzE1NTcyNCcpLmh0bWwoJ1NldHRpbmdzIGFyZSBzYXZlZCcpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLXNhdmluZy1wcm9ncmVzcycpLmNzcygnY29sb3InLCAnIzcyMWMyNCcpLmh0bWwoJ1NvbWV0aGluZyB3ZW50IHdyb25nLCB0cnkgYWdhaW4nKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAkKCcuanMtc2F2aW5nLXByb2dyZXNzJykucmVtb3ZlQ2xhc3MoJ2FjdGl2ZScpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uKGRhdGEpIHtcclxuICAgICAgICAgICAgJCgnLmpzLXNhdmluZy1wcm9ncmVzcycpLmNzcygnY29sb3InLCAnIzcyMWMyNCcpLmh0bWwoJ1NvbWV0aGluZyB3ZW50IHdyb25nLCB0cnkgYWdhaW4nKTtcclxuICAgICAgICAgICAgJCgnLmpzLXNhdmluZy1wcm9ncmVzcycpLnJlbW92ZUNsYXNzKCdhY3RpdmUnKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7XHJcblxyXG5cclxuZnVuY3Rpb24gc2VyaWFsaXplRm9ybURhdGEoKSB7XHJcbiAgICB2YXIgcmV0dXJuRGF0YSA9ICQoJyNzZXR0aW5nc0Zvcm0nKS5zZXJpYWxpemVBcnJheSgpLnJlZHVjZShmdW5jdGlvbiAob2JqLCBpdGVtKSB7XHJcbiAgICAgICAgb2JqW2l0ZW0ubmFtZV0gPSBpdGVtLnZhbHVlO1xyXG4gICAgICAgIHJldHVybiBvYmo7XHJcbiAgICB9LCB7fSk7XHJcbiAgICBpZiAocmV0dXJuRGF0YS5oYXNPd25Qcm9wZXJ0eSgnYXV0b21hdGljU2lnbicpKSB7XHJcbiAgICAgICAgcmV0dXJuRGF0YS5hdXRvbWF0aWNTaWduID0gJ3RydWUnO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgICByZXR1cm5EYXRhLmF1dG9tYXRpY1NpZ24gPSAnZmFsc2UnO1xyXG4gICAgfVxyXG5cclxuICAgIGlmIChyZXR1cm5EYXRhLmhhc093blByb3BlcnR5KCdhdXRvbWF0aWNGaXgnKSkge1xyXG4gICAgICAgIHJldHVybkRhdGEuYXV0b21hdGljRml4ID0gJ3RydWUnO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgICByZXR1cm5EYXRhLmF1dG9tYXRpY0ZpeCA9ICdmYWxzZSc7XHJcbiAgICB9XHJcblxyXG4gICAgaWYgKHJldHVybkRhdGEuaGFzT3duUHJvcGVydHkoJ2F1dG9tYXRpY0tleVJvbGxvdmVyJykpIHtcclxuICAgICAgICByZXR1cm5EYXRhLmF1dG9tYXRpY0tleVJvbGxvdmVyID0gJ3RydWUnO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgICByZXR1cm5EYXRhLmF1dG9tYXRpY0tleVJvbGxvdmVyID0gJ2ZhbHNlJztcclxuICAgIH1cclxuXHJcbiAgICBjb25zb2xlLmxvZyhyZXR1cm5EYXRhKTtcclxuXHJcbiAgICByZXR1cm4gcmV0dXJuRGF0YTtcclxufSJdfQ==
