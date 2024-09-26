$('body').on('click', '.js-btn-notify', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Notifying domain');

    $.ajax({
        url: '/Domains/Notify',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Notify succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});


$('body').on('click', '.js-btn-verify', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Verifying domain');

    $.ajax({
        url: '/Domains/Verify',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Verify succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-retrieve', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Retrieving domain');

    $.ajax({
        url: '/Domains/Retrieve',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Retrieve succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});


$('body').on('click', '.alertCross__cross', function () {
    $(this).closest('.alertCross').addClass('disnone'); 
});

$('body').on('click', '.js-btn-rectify', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Notifying domain');
    $.ajax({
        url: '/Domains/Rectify',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Rectify succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-delete-dns', function () {
    var elem = '.js-notifyRectifyError';

    $.ajax({
        url: '/Domains/DeleteDomainFromDns',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                window.location.reload();
            } else {
                $(this).closest('.modal').modal('hide');
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $(this).closest('.modal').modal('hide');
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-delete-db', function () {
    var elem = '.js-notifyRectifyError';

    $.ajax({
        url: '/Domains/DeleteDomainFromDb',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.href='/Domains';
            } else {
                $(this).closest('.modal').modal('hide');
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $(this).closest('.modal').modal('hide');
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-checkNow', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Checking the domain now', 'secondary');
    $.ajax({
        url: '/Domains/CheckDomainNow',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            } else {
                $(this).closest('.modal').modal('hide');
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $(this).closest('.modal').modal('hide');
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});