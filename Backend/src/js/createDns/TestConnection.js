$('body').on('click', '.js-test-connection', function (e) {
    // Only handle this button on pages that have the live test inputs
    if ($('#BaseUrl').length === 0 || $('#AuthToken').length === 0) {
        return; // not the Create/Edit DNS Server page — let other handlers process
    }
    e.preventDefault();
    testConnection();
});

function testConnection() {
    var name = $('#Name').val();
    var url = $('#BaseUrl').val();
    var apiKey = $('#AuthToken').val();
    if (!url || !apiKey) {
        return; // missing inputs; avoid calling backend with undefined values
    }
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