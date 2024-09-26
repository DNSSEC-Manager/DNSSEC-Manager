$('body').on('click', '.js-edit-kind', function() {
    $('#editKindModal').modal('show');

});

$('body').on('click', '.js-saveKindConfirm', function () {
    var id = $(this).data('id');
    var kind = $("#editKindType :selected").val();

    $.ajax({
        url: '/Domains/EditKind',
        type: 'POST',
        data: {
            id: id,
            kind: kind
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            } else {
                $('.js-editKindError').removeClass('disnone');
                AlertChange('.js-editKindError', 'Error returned from the server: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $('.js-editKindError').removeClass('disnone');
            AlertChange('.js-editKindError', 'Error returned from the server: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-edit-ips', function () {
    $('#editIpsModal').modal('show');
});

$('body').on('click', '.js-saveIpsConfirm', function () {
    var id = $(this).data('id');
    var ips = $("#editIpsTxb").val();
    console.log(id);
    console.log(ips);
    $.ajax({
        url: '/Domains/EditIps',
        type: 'POST',
        data: {
            id: id,
            ips: ips
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            } else {
                $('.js-editIpsError').removeClass('disnone');
                AlertChange('.js-editIpsError', 'Error returned from the server: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $('.js-editIpsError').removeClass('disnone');
            AlertChange('.js-editIpsError', 'Error returned from the server: ' + data, 'danger');
        }
    });
});