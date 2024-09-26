$('body').on('click', '.js-add-nameservergroup', function () {
    var id = $(this).data('dnsid');
    var name = $('#nameserverGroupName').val();
    var nameservers = [];

    $('.js-nameserver').each(function (index) {
        nameservers.push($(this).val());
    });

    var data = {
        dnsServerId: id,
        name: name,
        nameServers: nameservers
    };

    console.log(data);
    $.ajax({
        url: '/NameServerGroups/CreateGroup',
        type: 'POST',
        data: data,
        success: function (resp) {
            console.log(resp);
            if (resp === 'success') {
                window.location.reload();
            } else {
                $('.js-nameservergroup-error .js-error-message').html('Server returned an error: ' + resp);
                $('.js-nameservergroup-error').addClass('alert-danger').removeClass('d-none');
            }

        },
        error: function (resp) {
            $('.js-nameservergroup-error .js-error-message').html('Server returned an error: ' + resp);
            $('.js-nameservergroup-error').addClass('alert-danger').removeClass('d-none');
        }
    });
});

$('body').on('click', '.js-add-nameserver', function () {
    $('.js-nameservers-container').append('<br><input class="form-control js-nameserver" />');
});

$('body').on('click', '.js-delete-nameservergroup', function () {
    var id = $(this).data('id');
    $('#deleteNameservergroupModal').data('id', id);

});

$('body').on('click', '.js-delete-nameservergroup-confirm', function () {
    var id = $('#deleteNameservergroupModal').data('id');
    $.ajax({
        url: '/NameServerGroups/Delete',
        type: 'POST',
        data: {id: id},
        success: function (resp) {
            console.log(resp);
            if (resp === 'success') {
                window.location.reload();
            } else {
                $('.js-nameservergroup-delete-error .js-error-message').html('Server returned an error: ' + resp);
                $('.js-nameservergroup-delete-error').addClass('alert-danger').removeClass('d-none');
            }

        },
        error: function (resp) {
            $('.js-nameservergroup-delete-error .js-error-message').html('Server returned an error: ' + resp);
            $('.js-nameservergroup-delete-error').addClass('alert-danger').removeClass('d-none');
        }
    });
    console.log(id);
});
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
//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIkRuc0RldGFpbHMuanMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBIiwiZmlsZSI6IkRuc0RldGFpbHMuanMiLCJzb3VyY2VzQ29udGVudCI6WyIkKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1hZGQtbmFtZXNlcnZlcmdyb3VwJywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIGlkID0gJCh0aGlzKS5kYXRhKCdkbnNpZCcpO1xyXG4gICAgdmFyIG5hbWUgPSAkKCcjbmFtZXNlcnZlckdyb3VwTmFtZScpLnZhbCgpO1xyXG4gICAgdmFyIG5hbWVzZXJ2ZXJzID0gW107XHJcblxyXG4gICAgJCgnLmpzLW5hbWVzZXJ2ZXInKS5lYWNoKGZ1bmN0aW9uIChpbmRleCkge1xyXG4gICAgICAgIG5hbWVzZXJ2ZXJzLnB1c2goJCh0aGlzKS52YWwoKSk7XHJcbiAgICB9KTtcclxuXHJcbiAgICB2YXIgZGF0YSA9IHtcclxuICAgICAgICBkbnNTZXJ2ZXJJZDogaWQsXHJcbiAgICAgICAgbmFtZTogbmFtZSxcclxuICAgICAgICBuYW1lU2VydmVyczogbmFtZXNlcnZlcnNcclxuICAgIH07XHJcblxyXG4gICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9OYW1lU2VydmVyR3JvdXBzL0NyZWF0ZUdyb3VwJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YTogZGF0YSxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAocmVzcCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhyZXNwKTtcclxuICAgICAgICAgICAgaWYgKHJlc3AgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLW5hbWVzZXJ2ZXJncm91cC1lcnJvciAuanMtZXJyb3ItbWVzc2FnZScpLmh0bWwoJ1NlcnZlciByZXR1cm5lZCBhbiBlcnJvcjogJyArIHJlc3ApO1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLW5hbWVzZXJ2ZXJncm91cC1lcnJvcicpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnZC1ub25lJyk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcjogZnVuY3Rpb24gKHJlc3ApIHtcclxuICAgICAgICAgICAgJCgnLmpzLW5hbWVzZXJ2ZXJncm91cC1lcnJvciAuanMtZXJyb3ItbWVzc2FnZScpLmh0bWwoJ1NlcnZlciByZXR1cm5lZCBhbiBlcnJvcjogJyArIHJlc3ApO1xyXG4gICAgICAgICAgICAkKCcuanMtbmFtZXNlcnZlcmdyb3VwLWVycm9yJykuYWRkQ2xhc3MoJ2FsZXJ0LWRhbmdlcicpLnJlbW92ZUNsYXNzKCdkLW5vbmUnKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1hZGQtbmFtZXNlcnZlcicsIGZ1bmN0aW9uICgpIHtcclxuICAgICQoJy5qcy1uYW1lc2VydmVycy1jb250YWluZXInKS5hcHBlbmQoJzxicj48aW5wdXQgY2xhc3M9XCJmb3JtLWNvbnRyb2wganMtbmFtZXNlcnZlclwiIC8+Jyk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtZGVsZXRlLW5hbWVzZXJ2ZXJncm91cCcsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBpZCA9ICQodGhpcykuZGF0YSgnaWQnKTtcclxuICAgICQoJyNkZWxldGVOYW1lc2VydmVyZ3JvdXBNb2RhbCcpLmRhdGEoJ2lkJywgaWQpO1xyXG5cclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1kZWxldGUtbmFtZXNlcnZlcmdyb3VwLWNvbmZpcm0nLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgaWQgPSAkKCcjZGVsZXRlTmFtZXNlcnZlcmdyb3VwTW9kYWwnKS5kYXRhKCdpZCcpO1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvTmFtZVNlcnZlckdyb3Vwcy9EZWxldGUnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7aWQ6IGlkfSxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAocmVzcCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhyZXNwKTtcclxuICAgICAgICAgICAgaWYgKHJlc3AgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLW5hbWVzZXJ2ZXJncm91cC1kZWxldGUtZXJyb3IgLmpzLWVycm9yLW1lc3NhZ2UnKS5odG1sKCdTZXJ2ZXIgcmV0dXJuZWQgYW4gZXJyb3I6ICcgKyByZXNwKTtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1uYW1lc2VydmVyZ3JvdXAtZGVsZXRlLWVycm9yJykuYWRkQ2xhc3MoJ2FsZXJ0LWRhbmdlcicpLnJlbW92ZUNsYXNzKCdkLW5vbmUnKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbiAocmVzcCkge1xyXG4gICAgICAgICAgICAkKCcuanMtbmFtZXNlcnZlcmdyb3VwLWRlbGV0ZS1lcnJvciAuanMtZXJyb3ItbWVzc2FnZScpLmh0bWwoJ1NlcnZlciByZXR1cm5lZCBhbiBlcnJvcjogJyArIHJlc3ApO1xyXG4gICAgICAgICAgICAkKCcuanMtbmFtZXNlcnZlcmdyb3VwLWRlbGV0ZS1lcnJvcicpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnZC1ub25lJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbiAgICBjb25zb2xlLmxvZyhpZCk7XHJcbn0pO1xuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtdGVzdC1jb25uZWN0aW9uJywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIG15RGF0YSA9IHsgaWQ6ICQodGhpcykuZGF0YShcImlkXCIpIH07XHJcbiAgICBjb25zb2xlLmxvZyhteURhdGEpO1xyXG4gICAgJCgnLmpzLWNvbm5lY3Rpb24tZXJyb3InKS5yZW1vdmVDbGFzcygnYWxlcnQtZGFuZ2VyJykucmVtb3ZlQ2xhc3MoJ2FsZXJ0LXN1Y2Nlc3MnKS5yZW1vdmVDbGFzcyhcImQtbm9uZVwiKTtcclxuICAgICQoJy5qcy1jb25uZWN0aW9uLWVycm9yIC5qcy1lcnJvci1tZXNzYWdlJykuaHRtbCgnRXN0YWJsaXNoaW5nIGNvbm5lY3Rpb24nKTtcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0Ruc1NlcnZlcnMvQ2hlY2tDb25uZWN0aW9uJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YTogbXlEYXRhLFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvciAuanMtZXJyb3ItbWVzc2FnZScpLmh0bWwoJ0Nvbm5lY3Rpb24gc3VjY2VzZnVsbHkgZXN0YWJsaXNoZWQnKTtcclxuICAgICAgICAgICAgICAgICQoXCIuanMtY29ubmVjdGlvbi1lcnJvclwiKS5hZGRDbGFzcygnYWxlcnQtc3VjY2VzcycpO1xyXG5cclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1jb25uZWN0aW9uLWVycm9yIC5qcy1lcnJvci1tZXNzYWdlJykuaHRtbCgnQW4gZXJyb3Igd2FzIHJldHVybmVkOiAnICsgZGF0YSk7XHJcbiAgICAgICAgICAgICAgICAkKFwiLmpzLWNvbm5lY3Rpb24tZXJyb3JcIikuYWRkQ2xhc3MoJ2FsZXJ0LWRhbmdlcicpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcjogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgICQoJy5qcy1jb25uZWN0aW9uLWVycm9yIC5qcy1lcnJvci1tZXNzYWdlJykuaHRtbCgnRmFpbGVkIHRvIG1ha2UgYSBjb25uZWN0aW9uIHRvIHRoZSBzZXJ2ZXInKTtcclxuICAgICAgICAgICAgJChcIi5qcy1jb25uZWN0aW9uLWVycm9yXCIpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1oaWRlLWVycm9yJywgZnVuY3Rpb24gKCkge1xyXG4gICAgJCh0aGlzKS5jbG9zZXN0KCcuYWxlcnQnKS5hZGRDbGFzcygnZC1ub25lJyk7XHJcbi8vICAgICQoJy5qcy1jb25uZWN0aW9uLWVycm9yJykuYWRkQ2xhc3MoJ2Qtbm9uZScpO1xyXG59KTsiXX0=
