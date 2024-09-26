$('body').on('click', '.js-edit-rrset', function () {

    var trElem = $(this).closest('tr');

    $(trElem).find('.js-edit-txb ').removeClass('disnone');
    $(trElem).find('.js-edit-text ').addClass('disnone');
    $(trElem).find('.js-edit-rrset').addClass('disnone');
    $(trElem).find('.js-delete-rrset').addClass('disnone');
    $(trElem).find('.js-save-rrset-edit ').removeClass('disnone');
    $(trElem).find('.js-stop-edit-rrset').removeClass('disnone');
});

$('body').on('click', '.js-stop-edit-rrset', function () {

    var trElem = $(this).closest('tr');

    $(trElem).find('.js-edit-txb ').addClass('disnone');
    $(trElem).find('.js-edit-text ').removeClass('disnone');
    $(trElem).find('.js-edit-rrset').removeClass('disnone');
    $(trElem).find('.js-delete-rrset').removeClass('disnone');
    $(trElem).find('.js-save-rrset-edit ').addClass('disnone');
    $(trElem).find('.js-stop-edit-rrset').addClass('disnone');
});

$('body').on('click', '.js-edit-error-dismiss', function() {
    $(this).closest('tr').addClass('disnone');
});

function compareOldAndNew(data) {
    console.log(data);
    if (data.oldName === data.newName && data.oldType === data.newType && data.oldContent === data.newContent && data.oldTtl == data.newTtl ) {
        return true;
    }
    return false;
}

$('body').on('click', '.js-save-rrset-edit', function () {
    var trElem = $(this).closest('tr');
    var datatoSend =
    {
        id: $(this).data('id'),
        oldName: $(this).data('name'),
        oldType: $(this).data('type'),
        oldContent: $(this).data('content'),
        oldTtl: $(this).data('ttl'),
        newName: $(trElem).find('.js-edit-txb[name=editName]').val(),
        newContent: $(trElem).find('.js-edit-txb[name=editContent]').val(),
        newTtl: $(trElem).find('.js-edit-txb[name=editTtl]').val()
    }
    if (datatoSend.newTtl < 60) {
        $(trElem).next('tr').removeClass('disnone');
        $('.js-confirm-edit-ttl').addClass('disnone');
        $(trElem).next('tr').find('td:first-child()').html('The Time To Live should not be lower than 60');
        return;
    } 
    console.log(compareOldAndNew(datatoSend));
    if (compareOldAndNew(datatoSend)) {
        window.location.reload();
    } else {
        console.log(datatoSend);
        $.ajax({
            url: '/Domains/EditRrset',
            type: 'POST',
            data: datatoSend,
            success: function(data) {
                console.log(data);
                if (data === 'success') {
                    window.location.reload();
                } else if (data === 'ttl replaced') {
                    $('.js-confirm-edit-ttl').removeClass('disnone');
                    $(trElem).next('tr').removeClass('disnone');
                    $(trElem).next('tr').find('td:first-child()').html(
                        'If you change the time to live from this record every record of type: "' +
                        datatoSend.newType +
                        '" will get the same time to live');
                    datatoSend.replace = true;
                    var elemEventListener = $(trElem).next('tr').find('.js-confirm-edit-ttl');
                    console.log(elemEventListener);
                    elemEventListener[0].addEventListener('click',
                        function(e) {
                            console.log('should do something?');
                            $.ajax({
                                url: '/Domains/EditRrset',
                                type: 'POST',
                                data: datatoSend,
                                success: function(data) {
                                    console.log(data);
                                    if (data === 'success' || data === 'ttl replaced') {
                                        window.location.reload();
                                    }
                                }
                            });
                        }
                    );
                } else {
                    $('.js-confirm-edit-ttl').addClass('disnone');
                    $(trElem).next('tr').removeClass('disnone');
                    $(trElem).next('tr').find('td:first-child()').html('Error returned from the server: ' + data);
                }
            },
            error: function(data) {
                console.log(data);
            }
        });
    }
});