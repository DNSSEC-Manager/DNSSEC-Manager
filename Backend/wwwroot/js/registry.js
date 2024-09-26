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
$('body').on('click', '.js-delete-tldregistry', function () {
    var tldid = $(this).data('tldid');
    var regid = $(this).data('regid');

    $('#deleteTldRegistryModal').data('tldid', tldid).data('regid', regid);
});

$('body').on('click', '.js-hide-error', function () {
    $(this).closest('.alert').addClass('d-none');
});

$('body').on('click', '.js-delete-tldregistry-confirm', function () {
    var data = {
        tldId: $('#deleteTldRegistryModal').data('tldid'),
        registryId: $('#deleteTldRegistryModal').data('regid')
    }
    console.log(data);

    $.ajax({
        url: '/Registries/DeleteTldRegistry',
        type: 'POST',
        data: data,
        success: function (resp) {
            console.log(resp);
            window.location.reload();
        }
    });
});

$('body').on('click', '.js-add-tldregistry-confirm', function () {
    var data = {
        registryId: $('#addTldRegistryModal').data('id'),
        newTld: $('#addTldRegistryInp').val()
    }
    console.log(data);

    $.ajax({
        url: '/Registries/AddTldRegistry',
        type: 'POST',
        data: data,
        success: function (resp) {
            console.log(resp);
            if (resp === 'success') {
                window.location.reload();
            } else {
                $('.js-addTldRegistry-error .js-error-message').html('Server returned an error: ' + resp);
                $('.js-addTldRegistry-error').addClass('alert-danger').removeClass('d-none');
            }

        },
        error: function(resp) {
            $('.js-addTldRegistry-error .js-error-message').html('Server returned an error: ' + resp);
            $('.js-addTldRegistry-error').addClass('alert-danger').removeClass('d-none');
        }
    });
});
//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInJlZ2lzdHJ5LmpzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSIsImZpbGUiOiJyZWdpc3RyeS5qcyIsInNvdXJjZXNDb250ZW50IjpbIiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXRlc3QtY29ubmVjdGlvbi1yZWdpc3RyeUlkJywgZnVuY3Rpb24gKCkge1xyXG4gICAgJCgnLmpzLWNvbm5lY3Rpb24tZXJyb3ItcmVnaXN0cnknKS5yZW1vdmVDbGFzcygnYWxlcnQtZGFuZ2VyJykucmVtb3ZlQ2xhc3MoJ2FsZXJ0LXN1Y2Nlc3MnKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpLnJlbW92ZUNsYXNzKFwiZGlzbm9uZVwiKTtcclxuICAgICQoJy5qcy1lcnJvci1tZXNzYWdlJykuaHRtbCgnRXN0YWJsaXNoaW5nIGNvbm5lY3Rpb24nKTtcclxuICAgIHZhciBkYXRhID0ge1xyXG4gICAgICAgIElkOiAkKHRoaXMpLmRhdGEoJ2lkJylcclxuICAgIH07XHJcblxyXG4gICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICBzZW5kQWpheChkYXRhKTtcclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy10ZXN0LWNvbm5lY3Rpb24tcmVnaXN0cnknLCBmdW5jdGlvbiAoKSB7XHJcbiAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeScpLnJlbW92ZUNsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpLnJlbW92ZUNsYXNzKCdhbGVydC1zdWNjZXNzJykucmVtb3ZlQ2xhc3MoXCJkaXNub25lXCIpO1xyXG4gICAgJCgnLmpzLWVycm9yLW1lc3NhZ2UnKS5odG1sKCdFc3RhYmxpc2hpbmcgY29ubmVjdGlvbicpO1xyXG5cclxuICAgIHZhciBkYXRhID0ge1xyXG4gICAgICAgIG5hbWU6ICQoJyNOYW1lJykudmFsKCksXHJcbiAgICAgICAgdXJsOiAkKCcjVXJsJykudmFsKCksXHJcbiAgICAgICAgcG9ydDogJCgnI1BvcnQnKS52YWwoKSxcclxuICAgICAgICB1c2VybmFtZTogJCgnI1VzZXJuYW1lJykudmFsKCksXHJcbiAgICAgICAgcGFzc3dvcmQ6ICQoJyNQYXNzd29yZCcpLnZhbCgpLFxyXG4gICAgICAgIHJlZ2lzdHJ5VHlwZTogJCgnI1JlZ2lzdHJ5VHlwZScpLnZhbCgpLFxyXG4gICAgICAgIGlkOiAkKCcjSWQnKS52YWwoKVxyXG4gICAgfTtcclxuXHJcbiAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgIHNlbmRBamF4KGRhdGEpO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWhpZGUtZXJyb3InLCBmdW5jdGlvbiAoKSB7XHJcbiAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeScpLmFkZENsYXNzKCdkaXNub25lJyk7XHJcbn0pO1xyXG5cclxuZnVuY3Rpb24gc2VuZEFqYXgoZGF0YSkge1xyXG4gICAgdmFyIHVybCA9ICcvUmVnaXN0cmllcy9UZXN0Q29ubmVjdGlvbic7XHJcblxyXG4gICAgaWYgKGRhdGEuSWQgIT0gbnVsbCkge1xyXG4gICAgICAgIHVybCA9ICcvUmVnaXN0cmllcy9UZXN0Q29ubmVjdGlvbklkJztcclxuICAgIH1cclxuXHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogdXJsLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiBkYXRhLFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChyZXNwKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKHJlc3ApO1xyXG4gICAgICAgICAgICBpZiAocmVzcCA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtZXJyb3ItbWVzc2FnZScpLmh0bWwoJ0Nvbm5lY3Rpb24gc3VjY2VzZnVsbHkgZXN0YWJsaXNoZWQnKTtcclxuICAgICAgICAgICAgICAgICQoXCIuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeVwiKS5hZGRDbGFzcygnYWxlcnQtc3VjY2VzcycpO1xyXG5cclxuICAgICAgICAgICAgfSBlbHNlIGlmIChyZXNwID09PSAnQ29tbWFuZCBzeW50YXggZXJyb3InKSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeScpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpO1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLWVycm9yLW1lc3NhZ2UnKS5odG1sKCdDb25uZWN0aW9uIGZhaWxlZCwgcmVzcG9uc2U6IFdyb25nIHVzZXJuYW1lL3Bhc3N3b3JkJyk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtZXJyb3ItbWVzc2FnZScpLmh0bWwoJ0FuIGVycm9yIHdhcyByZXR1cm5lZDogJyArIHJlc3ApO1xyXG4gICAgICAgICAgICAgICAgJChcIi5qcy1jb25uZWN0aW9uLWVycm9yLXJlZ2lzdHJ5XCIpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uIChyZXNwKSB7XHJcbiAgICAgICAgICAgICQoJy5qcy1lcnJvci1tZXNzYWdlJykuaHRtbCgnQW4gZXJyb3Igd2FzIHJldHVybmVkOiAnICsgcmVzcCk7XHJcbiAgICAgICAgICAgICQoXCIuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeVwiKS5hZGRDbGFzcygnZmFpbGVkIHRvIGNvbm5lY3QgdG8gdGhlIHNlcnZlcicpO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59XG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1kZWxldGUtdGxkcmVnaXN0cnknLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgdGxkaWQgPSAkKHRoaXMpLmRhdGEoJ3RsZGlkJyk7XHJcbiAgICB2YXIgcmVnaWQgPSAkKHRoaXMpLmRhdGEoJ3JlZ2lkJyk7XHJcblxyXG4gICAgJCgnI2RlbGV0ZVRsZFJlZ2lzdHJ5TW9kYWwnKS5kYXRhKCd0bGRpZCcsIHRsZGlkKS5kYXRhKCdyZWdpZCcsIHJlZ2lkKTtcclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1oaWRlLWVycm9yJywgZnVuY3Rpb24gKCkge1xyXG4gICAgJCh0aGlzKS5jbG9zZXN0KCcuYWxlcnQnKS5hZGRDbGFzcygnZC1ub25lJyk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtZGVsZXRlLXRsZHJlZ2lzdHJ5LWNvbmZpcm0nLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgZGF0YSA9IHtcclxuICAgICAgICB0bGRJZDogJCgnI2RlbGV0ZVRsZFJlZ2lzdHJ5TW9kYWwnKS5kYXRhKCd0bGRpZCcpLFxyXG4gICAgICAgIHJlZ2lzdHJ5SWQ6ICQoJyNkZWxldGVUbGRSZWdpc3RyeU1vZGFsJykuZGF0YSgncmVnaWQnKVxyXG4gICAgfVxyXG4gICAgY29uc29sZS5sb2coZGF0YSk7XHJcblxyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvUmVnaXN0cmllcy9EZWxldGVUbGRSZWdpc3RyeScsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKHJlc3ApIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2cocmVzcCk7XHJcbiAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZWxvYWQoKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1hZGQtdGxkcmVnaXN0cnktY29uZmlybScsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBkYXRhID0ge1xyXG4gICAgICAgIHJlZ2lzdHJ5SWQ6ICQoJyNhZGRUbGRSZWdpc3RyeU1vZGFsJykuZGF0YSgnaWQnKSxcclxuICAgICAgICBuZXdUbGQ6ICQoJyNhZGRUbGRSZWdpc3RyeUlucCcpLnZhbCgpXHJcbiAgICB9XHJcbiAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuXHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9SZWdpc3RyaWVzL0FkZFRsZFJlZ2lzdHJ5JyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YTogZGF0YSxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAocmVzcCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhyZXNwKTtcclxuICAgICAgICAgICAgaWYgKHJlc3AgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLWFkZFRsZFJlZ2lzdHJ5LWVycm9yIC5qcy1lcnJvci1tZXNzYWdlJykuaHRtbCgnU2VydmVyIHJldHVybmVkIGFuIGVycm9yOiAnICsgcmVzcCk7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtYWRkVGxkUmVnaXN0cnktZXJyb3InKS5hZGRDbGFzcygnYWxlcnQtZGFuZ2VyJykucmVtb3ZlQ2xhc3MoJ2Qtbm9uZScpO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uKHJlc3ApIHtcclxuICAgICAgICAgICAgJCgnLmpzLWFkZFRsZFJlZ2lzdHJ5LWVycm9yIC5qcy1lcnJvci1tZXNzYWdlJykuaHRtbCgnU2VydmVyIHJldHVybmVkIGFuIGVycm9yOiAnICsgcmVzcCk7XHJcbiAgICAgICAgICAgICQoJy5qcy1hZGRUbGRSZWdpc3RyeS1lcnJvcicpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnZC1ub25lJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pOyJdfQ==
