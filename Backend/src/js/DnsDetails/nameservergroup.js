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