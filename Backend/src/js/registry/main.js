$('body').on('click', '.js-test-connection-registryId', function () {
    $('.js-connection-error-registry').removeClass('alert-danger').removeClass('alert-success').removeClass('alert-success').removeClass("disnone");
    $('.js-error-message').html('Establishing connection');
    var data = {
        Id: $(this).data('id')
    };

    console.log(data);
    sendAjax(data);
});

$('body').on('click', '.js-test-connection-registry', function () {
    $('.js-connection-error-registry').removeClass('alert-danger').removeClass('alert-success').removeClass('alert-success').removeClass("disnone");
    $('.js-error-message').html('Establishing connection');

    var data = {
        name: $('#Name').val(),
        url: $('#Url').val(),
        port: $('#Port').val(),
        username: $('#Username').val(),
        password: $('#Password').val(),
        registryType: $('#RegistryType').val(),
        id: $('#Id').val()
    };

    console.log(data);
    sendAjax(data);
});

$('body').on('click', '.js-hide-error', function () {
    $('.js-connection-error-registry').addClass('disnone');
});

function sendAjax(data) {
    var url = '/Registries/TestConnection';

    if (data.Id != null) {
        url = '/Registries/TestConnectionId';
    }

    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: function (resp) {
            console.log(resp);
            if (resp === 'success') {
                $('.js-error-message').html('Connection succesfully established');
                $(".js-connection-error-registry").addClass('alert-success');

            } else if (resp === 'Command syntax error') {
                $('.js-connection-error-registry').addClass('alert-danger').removeClass('alert-success');
                $('.js-error-message').html('Connection failed, response: Wrong username/password');
            } else {
                $('.js-error-message').html('An error was returned: ' + resp);
                $(".js-connection-error-registry").addClass('alert-danger');
            }
        },
        error: function (resp) {
            $('.js-error-message').html('An error was returned: ' + resp);
            $(".js-connection-error-registry").addClass('failed to connect to the server');
        }
    });
}