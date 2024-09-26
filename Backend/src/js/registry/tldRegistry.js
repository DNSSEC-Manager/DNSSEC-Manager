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