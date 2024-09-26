$('body').on('click', '.js-btn-sign', function () {
    var elem = '.js-signError';
    $(elem).removeClass('disnone');
    $.ajax({
        url: '/Domains/Sign',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Domain will soon be signed by the scheduler', 'success');
                window.location.reload();
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});


$('body').on('click', '.js-btn-unsign', function () {
    var elem = '.js-signError';
    $(elem).removeClass('disnone');
    $.ajax({
        url: '/Domains/Unsign',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Domain will soon be unsigned by the scheduler', 'success');
                window.location.reload();
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-rollover', function () {
    var elem = '.js-signError';
    $(elem).removeClass('disnone');
    $.ajax({
        url: '/Domains/Rollover',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Key Rollover process will be started by the scheduler', 'success');
                window.location.reload();
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});