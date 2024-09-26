$('body').on('click', '.js-test-connection', function () {
    testConnection();
});

function testConnection() {
    var name = $('#Name').val();
    var url = $('#BaseUrl').val();
    var apiKey = $('#AuthToken').val();
    var myData = { url: url, apiKey: apiKey };
    $('.js-connection-error').removeClass('alert-danger').removeClass('alert-success').html('Testing connection...');
    $.ajax({
        url: '/DnsServers/CheckConnectionLive',
        type: 'POST',
        data: myData,
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                $('.js-connection-error').addClass('alert-success').html('Connection succesfully established');
            } else {
                $('.js-connection-error').addClass('alert-danger').html('An error was returned: ' + data);
            }
        },
        error: function(data) {
            $('.js-connection-error').addClass('alert-danger').html('Failed to make a connection to the server');
        }
    });
}