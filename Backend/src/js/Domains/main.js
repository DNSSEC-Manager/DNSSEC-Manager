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