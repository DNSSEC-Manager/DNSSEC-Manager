$('body').on('click', '.js-test-connection', function () {
    var myData = { id: $(this).data("id") };
    console.log(myData);
    $('.js-connection-error').removeClass('alert-danger').removeClass('alert-success').removeClass("d-none");
    $('.js-connection-error .js-error-message').html('Establishing connection');
    $.ajax({
        url: '/DnsServers/CheckConnection',
        type: 'POST',
        data: myData,
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                $('.js-connection-error .js-error-message').html('Connection succesfully established');
                $(".js-connection-error").addClass('alert-success');

            } else {
                $('.js-connection-error .js-error-message').html('An error was returned: ' + data);
                $(".js-connection-error").addClass('alert-danger');
            }
        },
        error: function (data) {
            console.log(data);
            $('.js-connection-error .js-error-message').html('Failed to make a connection to the server');
            $(".js-connection-error").addClass('alert-danger');
        }
    });
});

$('body').on('click', '.js-hide-error', function () {
    $(this).closest('.alert').addClass('d-none');
//    $('.js-connection-error').addClass('d-none');
});