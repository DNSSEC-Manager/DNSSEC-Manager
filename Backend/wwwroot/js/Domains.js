$('body').on('click', '.js-cleanup-all-domains', function () {
    $.ajax({
        url: '/Domains/CleanupAllDomains',
        type: 'POST',
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            }
        }
    });
});

$('body').on('click', '.js-unsign-all-domains', function () {
    $.ajax({
        url: '/Domains/UnsignAllDomains',
        type: 'POST',
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            }
        }
    });
});

$('body').on('click', '.js-sign-all-domains', function () {
    $.ajax({
        url: '/Domains/SignAllDomains',
        type: 'POST',
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            }
        }
    });
});

$('body').on('click', '.js-rollover-all-domains', function () {
    $.ajax({
        url: '/Domains/RolloverAllDomains',
        type: 'POST',
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            }
        }
    });
});


$('body').on('click', '.js-create-domain-save', function () {
    var name = $('#CreateDomainInput').val();
    var dnsId = $('#selectDnsId').val();
    console.log(dnsId);
    $.ajax({
        url: '/Domains/CreateDomain',
        type: 'POST',
        data: {
            DnsId: dnsId,
            Name: name
        },
        success: function (data) {
            console.log(data);
            if (data.startsWith('success: ')) {
                console.log(data.split(' '));
                var spl = data.split(' ');
                window.location.href = '/domains/details/' + spl[1];
            }
        },
        error: function (data) {
            console.log(data);
        }
    });
});
//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIkRvbWFpbnMuanMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSIsImZpbGUiOiJEb21haW5zLmpzIiwic291cmNlc0NvbnRlbnQiOlsiJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtY2xlYW51cC1hbGwtZG9tYWlucycsIGZ1bmN0aW9uICgpIHtcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvQ2xlYW51cEFsbERvbWFpbnMnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtdW5zaWduLWFsbC1kb21haW5zJywgZnVuY3Rpb24gKCkge1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvRG9tYWlucy9VbnNpZ25BbGxEb21haW5zJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgIGlmIChkYXRhID09PSAnc3VjY2VzcycpIHtcclxuICAgICAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZWxvYWQoKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXNpZ24tYWxsLWRvbWFpbnMnLCBmdW5jdGlvbiAoKSB7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9Eb21haW5zL1NpZ25BbGxEb21haW5zJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgIGlmIChkYXRhID09PSAnc3VjY2VzcycpIHtcclxuICAgICAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZWxvYWQoKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXJvbGxvdmVyLWFsbC1kb21haW5zJywgZnVuY3Rpb24gKCkge1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvRG9tYWlucy9Sb2xsb3ZlckFsbERvbWFpbnMnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xyXG5cclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWNyZWF0ZS1kb21haW4tc2F2ZScsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBuYW1lID0gJCgnI0NyZWF0ZURvbWFpbklucHV0JykudmFsKCk7XHJcbiAgICB2YXIgZG5zSWQgPSAkKCcjc2VsZWN0RG5zSWQnKS52YWwoKTtcclxuICAgIGNvbnNvbGUubG9nKGRuc0lkKTtcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvQ3JlYXRlRG9tYWluJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICBEbnNJZDogZG5zSWQsXHJcbiAgICAgICAgICAgIE5hbWU6IG5hbWVcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YS5zdGFydHNXaXRoKCdzdWNjZXNzOiAnKSkge1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YS5zcGxpdCgnICcpKTtcclxuICAgICAgICAgICAgICAgIHZhciBzcGwgPSBkYXRhLnNwbGl0KCcgJyk7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24uaHJlZiA9ICcvZG9tYWlucy9kZXRhaWxzLycgKyBzcGxbMV07XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7Il19
