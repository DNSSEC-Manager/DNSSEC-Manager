$('#toggleExcludeSigning').change(function () {
    var val = $(this).prop('checked');
    var domainId = $(this).data('domainid');
    var elem = '.js-notifyRectifyError';
    $.ajax({
        url: '/Domains/ChangeExcludeSigning',
        type: 'POST',
        data: {
            domainId: domainId,
            excludeSigning: val
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
            } else {
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
})