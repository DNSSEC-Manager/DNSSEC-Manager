$('body').on('click', '.js-btn-notify', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Notifying domain');

    $.ajax({
        url: '/Domains/Notify',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Notify succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});


$('body').on('click', '.js-btn-verify', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Verifying domain');

    $.ajax({
        url: '/Domains/Verify',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Verify succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-retrieve', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Retrieving domain');

    $.ajax({
        url: '/Domains/Retrieve',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Retrieve succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});


$('body').on('click', '.alertCross__cross', function () {
    $(this).closest('.alertCross').addClass('disnone'); 
});

$('body').on('click', '.js-btn-rectify', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Notifying domain');
    $.ajax({
        url: '/Domains/Rectify',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Rectify succesfull!', 'success');
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-delete-dns', function () {
    var elem = '.js-notifyRectifyError';

    $.ajax({
        url: '/Domains/DeleteDomainFromDns',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                window.location.reload();
            } else {
                $(this).closest('.modal').modal('hide');
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $(this).closest('.modal').modal('hide');
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-delete-db', function () {
    var elem = '.js-notifyRectifyError';

    $.ajax({
        url: '/Domains/DeleteDomainFromDb',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.href='/Domains';
            } else {
                $(this).closest('.modal').modal('hide');
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $(this).closest('.modal').modal('hide');
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-checkNow', function () {
    var elem = '.js-notifyRectifyError';
    $('.js-notifyRectifyError').removeClass('disnone');
    AlertChange(elem, 'Checking the domain now', 'secondary');
    $.ajax({
        url: '/Domains/CheckDomainNow',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            } else {
                $(this).closest('.modal').modal('hide');
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $(this).closest('.modal').modal('hide');
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-add-rrset', function () {
    $('#addRrsetModal').modal('show');
});

$('body').on('click', '.add-rrset-confirm', function () {
    var name = $('#inpName').val();
    var content = $('#inpContent').val();
    var type = $('#inpType').val();
    var ttl = $('#inpTtl').val();
    var id = $(this).data('id');
    console.log('name: ' + name + ', content: ' + content + ', type: ' + type + ', ttl: ' + ttl);

    $.ajax({
        url: '/Domains/AddRrset',
        type: 'POST',
        data: {
            id: id,
            name: name,
            type: type,
            content: content,
            ttl: ttl
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                window.location.reload();
            }
            else if (data === 'ttl replaced') {
                $('.js-addRrsetError').removeClass('disnone');
                $('#replace-confirm').removeClass('disnone');
                AlertChange('.js-addRrsetError',
                    'The TTL of every type: "' + type + '" will be replaced with: ' + ttl + '. Are you sure?',
                    'danger');

                document.addEventListener('click',
                    function (e) {
                        if (e.target && e.target.id === 'replace-confirm') {
                            console.log('??');
                            $.ajax({
                                url: '/Domains/AddRrset',
                                type: 'POST',
                                data: {
                                    id: id,
                                    name: name,
                                    type: type,
                                    content: content,
                                    ttl: ttl,
                                    replace: true
                                },
                                success: function (data) {
                                    if (data === 'success' || data === 'ttl replaced') {
                                        window.location.reload();
                                    } else {
                                        $('.js-addRrsetError').removeClass('disnone');
                                        $('#replace-confirm').addClass('disnone');
                                        AlertChange('.js-addRrsetError', data, 'danger');
                                    }
                                }
                            });
                        }
                    }
                );
            } else {
                $('.js-addRrsetError').removeClass('disnone');
                $('#replace-confirm').addClass('disnone');
                AlertChange('.js-addRrsetError', data, 'danger');
            }
        },

        error: function (data) {
            AlertChange('.js-addRrsetError', data, 'danger');
        }
    });
});
$('body').on('click', '.js-delete-rrset', function () {
    var thisElem = $(this);
    var id = $(this).data('id');
    var name = $(this).data('name');
    var type = $(this).data('type');
    var content = $(this).data('content');
    document.addEventListener('click', function (e) {
        if (e.target && e.target.id == 'delete-rrset-confirm') {
            $.ajax({
                url: '/Domains/DeleteRrset',
                type: 'POST',
                data: {
                    id: id,
                    name: name,
                    type: type,
                    content: content
                },
                success: function (data) {
                    $(thisElem).closest('tr').remove();
                    $('#deleteRrsetModal').modal('hide');

                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
    $('#deleteRrsetModal').modal('show');
});
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
$('#toggleExcludeSigning').change(function () {
    var val = $(this).prop('checked');
    var domainId = $(this).data('domainid');
    var elem = '.js-notifyRectifyError';
    $.ajax({
        url: '/Domains/ChangeExcludeSigning',
        type: 'POST',
        data: {
            domainId: domainId,
            excludeSigning: val
        },
        success: function (data) {
            console.log(data);
            if (data === 'success') {
            } else {
                $('.js-notifyRectifyError').removeClass('disnone');
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            console.log(data);
            $('.js-notifyRectifyError').removeClass('disnone');
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
})
function AlertChange(elem, body, type = 'secondary') {
    console.log(type);
    switch (type) {
    case 'danger':
        $(elem).removeClass('alert-secondary').removeClass('alert-success').addClass('alert-danger');
        break;
    case 'success':
        $(elem).removeClass('alert-secondary').removeClass('alert-danger').addClass('alert-success');
        break;
    default:
        $(elem).removeClass('alert-danger').removeClass('alert-success').addClass('alert-secondary');
        break;
    }
    $(elem + ' .alertCross__text').html(body);
}
$('body').on('click', '.js-btn-sign', function () {
    var elem = '.js-signError';
    $(elem).removeClass('disnone');
    $.ajax({
        url: '/Domains/Sign',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Domain will soon be signed by the scheduler', 'success');
                window.location.reload();
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});


$('body').on('click', '.js-btn-unsign', function () {
    var elem = '.js-signError';
    $(elem).removeClass('disnone');
    $.ajax({
        url: '/Domains/Unsign',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Domain will soon be unsigned by the scheduler', 'success');
                window.location.reload();
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

$('body').on('click', '.js-btn-rollover', function () {
    var elem = '.js-signError';
    $(elem).removeClass('disnone');
    $.ajax({
        url: '/Domains/Rollover',
        type: 'POST',
        data: {
            Id: $(this).data('id')
        },
        success: function (data) {
            if (data === 'success') {
                AlertChange(elem, 'Key Rollover process will be started by the scheduler', 'success');
                window.location.reload();
            } else {
                AlertChange(elem, 'Server returned the error: ' + data, 'danger');
            }
        },
        error: function (data) {
            AlertChange(elem, 'Server returned the error: ' + data, 'danger');
        }
    });
});

//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIkRvbWFpbkRldGFpbHMuanMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSIsImZpbGUiOiJEb21haW5EZXRhaWxzLmpzIiwic291cmNlc0NvbnRlbnQiOlsiJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtYnRuLW5vdGlmeScsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBlbGVtID0gJy5qcy1ub3RpZnlSZWN0aWZ5RXJyb3InO1xyXG4gICAgJCgnLmpzLW5vdGlmeVJlY3RpZnlFcnJvcicpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICBBbGVydENoYW5nZShlbGVtLCAnTm90aWZ5aW5nIGRvbWFpbicpO1xyXG5cclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvTm90aWZ5JyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICBJZDogJCh0aGlzKS5kYXRhKCdpZCcpXHJcbiAgICAgICAgfSxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICBBbGVydENoYW5nZShlbGVtLCAnTm90aWZ5IHN1Y2Nlc2Z1bGwhJywgJ3N1Y2Nlc3MnKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcjogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KTtcclxuXHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1idG4tdmVyaWZ5JywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIGVsZW0gPSAnLmpzLW5vdGlmeVJlY3RpZnlFcnJvcic7XHJcbiAgICAkKCcuanMtbm90aWZ5UmVjdGlmeUVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdWZXJpZnlpbmcgZG9tYWluJyk7XHJcblxyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvRG9tYWlucy9WZXJpZnknLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIElkOiAkKHRoaXMpLmRhdGEoJ2lkJylcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGlmIChkYXRhID09PSAnc3VjY2VzcycpIHtcclxuICAgICAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdWZXJpZnkgc3VjY2VzZnVsbCEnLCAnc3VjY2VzcycpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtYnRuLXJldHJpZXZlJywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIGVsZW0gPSAnLmpzLW5vdGlmeVJlY3RpZnlFcnJvcic7XHJcbiAgICAkKCcuanMtbm90aWZ5UmVjdGlmeUVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdSZXRyaWV2aW5nIGRvbWFpbicpO1xyXG5cclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvUmV0cmlldmUnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIElkOiAkKHRoaXMpLmRhdGEoJ2lkJylcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGlmIChkYXRhID09PSAnc3VjY2VzcycpIHtcclxuICAgICAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdSZXRyaWV2ZSBzdWNjZXNmdWxsIScsICdzdWNjZXNzJyk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICBBbGVydENoYW5nZShlbGVtLCAnU2VydmVyIHJldHVybmVkIHRoZSBlcnJvcjogJyArIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBBbGVydENoYW5nZShlbGVtLCAnU2VydmVyIHJldHVybmVkIHRoZSBlcnJvcjogJyArIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7XHJcblxyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuYWxlcnRDcm9zc19fY3Jvc3MnLCBmdW5jdGlvbiAoKSB7XHJcbiAgICAkKHRoaXMpLmNsb3Nlc3QoJy5hbGVydENyb3NzJykuYWRkQ2xhc3MoJ2Rpc25vbmUnKTsgXHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtYnRuLXJlY3RpZnknLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgZWxlbSA9ICcuanMtbm90aWZ5UmVjdGlmeUVycm9yJztcclxuICAgICQoJy5qcy1ub3RpZnlSZWN0aWZ5RXJyb3InKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ05vdGlmeWluZyBkb21haW4nKTtcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvUmVjdGlmeScsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IHtcclxuICAgICAgICAgICAgSWQ6ICQodGhpcykuZGF0YSgnaWQnKVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1JlY3RpZnkgc3VjY2VzZnVsbCEnLCAnc3VjY2VzcycpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtYnRuLWRlbGV0ZS1kbnMnLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgZWxlbSA9ICcuanMtbm90aWZ5UmVjdGlmeUVycm9yJztcclxuXHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9Eb21haW5zL0RlbGV0ZURvbWFpbkZyb21EbnMnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIElkOiAkKHRoaXMpLmRhdGEoJ2lkJylcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGlmIChkYXRhID09PSAnc3VjY2VzcycpIHtcclxuICAgICAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZWxvYWQoKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQodGhpcykuY2xvc2VzdCgnLm1vZGFsJykubW9kYWwoJ2hpZGUnKTtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1ub3RpZnlSZWN0aWZ5RXJyb3InKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgJCh0aGlzKS5jbG9zZXN0KCcubW9kYWwnKS5tb2RhbCgnaGlkZScpO1xyXG4gICAgICAgICAgICAkKCcuanMtbm90aWZ5UmVjdGlmeUVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtYnRuLWRlbGV0ZS1kYicsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBlbGVtID0gJy5qcy1ub3RpZnlSZWN0aWZ5RXJyb3InO1xyXG5cclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvRGVsZXRlRG9tYWluRnJvbURiJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICBJZDogJCh0aGlzKS5kYXRhKCdpZCcpXHJcbiAgICAgICAgfSxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLmhyZWY9Jy9Eb21haW5zJztcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQodGhpcykuY2xvc2VzdCgnLm1vZGFsJykubW9kYWwoJ2hpZGUnKTtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1ub3RpZnlSZWN0aWZ5RXJyb3InKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgJCh0aGlzKS5jbG9zZXN0KCcubW9kYWwnKS5tb2RhbCgnaGlkZScpO1xyXG4gICAgICAgICAgICAkKCcuanMtbm90aWZ5UmVjdGlmeUVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtYnRuLWNoZWNrTm93JywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIGVsZW0gPSAnLmpzLW5vdGlmeVJlY3RpZnlFcnJvcic7XHJcbiAgICAkKCcuanMtbm90aWZ5UmVjdGlmeUVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdDaGVja2luZyB0aGUgZG9tYWluIG5vdycsICdzZWNvbmRhcnknKTtcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvQ2hlY2tEb21haW5Ob3cnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIElkOiAkKHRoaXMpLmRhdGEoJ2lkJylcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAkKHRoaXMpLmNsb3Nlc3QoJy5tb2RhbCcpLm1vZGFsKCdoaWRlJyk7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtbm90aWZ5UmVjdGlmeUVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcjogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgICQodGhpcykuY2xvc2VzdCgnLm1vZGFsJykubW9kYWwoJ2hpZGUnKTtcclxuICAgICAgICAgICAgJCgnLmpzLW5vdGlmeVJlY3RpZnlFcnJvcicpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KTtcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1idG4tYWRkLXJyc2V0JywgZnVuY3Rpb24gKCkge1xyXG4gICAgJCgnI2FkZFJyc2V0TW9kYWwnKS5tb2RhbCgnc2hvdycpO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmFkZC1ycnNldC1jb25maXJtJywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIG5hbWUgPSAkKCcjaW5wTmFtZScpLnZhbCgpO1xyXG4gICAgdmFyIGNvbnRlbnQgPSAkKCcjaW5wQ29udGVudCcpLnZhbCgpO1xyXG4gICAgdmFyIHR5cGUgPSAkKCcjaW5wVHlwZScpLnZhbCgpO1xyXG4gICAgdmFyIHR0bCA9ICQoJyNpbnBUdGwnKS52YWwoKTtcclxuICAgIHZhciBpZCA9ICQodGhpcykuZGF0YSgnaWQnKTtcclxuICAgIGNvbnNvbGUubG9nKCduYW1lOiAnICsgbmFtZSArICcsIGNvbnRlbnQ6ICcgKyBjb250ZW50ICsgJywgdHlwZTogJyArIHR5cGUgKyAnLCB0dGw6ICcgKyB0dGwpO1xyXG5cclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvQWRkUnJzZXQnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIGlkOiBpZCxcclxuICAgICAgICAgICAgbmFtZTogbmFtZSxcclxuICAgICAgICAgICAgdHlwZTogdHlwZSxcclxuICAgICAgICAgICAgY29udGVudDogY29udGVudCxcclxuICAgICAgICAgICAgdHRsOiB0dGxcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgZWxzZSBpZiAoZGF0YSA9PT0gJ3R0bCByZXBsYWNlZCcpIHtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1hZGRScnNldEVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgICAgICQoJyNyZXBsYWNlLWNvbmZpcm0nKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoJy5qcy1hZGRScnNldEVycm9yJyxcclxuICAgICAgICAgICAgICAgICAgICAnVGhlIFRUTCBvZiBldmVyeSB0eXBlOiBcIicgKyB0eXBlICsgJ1wiIHdpbGwgYmUgcmVwbGFjZWQgd2l0aDogJyArIHR0bCArICcuIEFyZSB5b3Ugc3VyZT8nLFxyXG4gICAgICAgICAgICAgICAgICAgICdkYW5nZXInKTtcclxuXHJcbiAgICAgICAgICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKCdjbGljaycsXHJcbiAgICAgICAgICAgICAgICAgICAgZnVuY3Rpb24gKGUpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGUudGFyZ2V0ICYmIGUudGFyZ2V0LmlkID09PSAncmVwbGFjZS1jb25maXJtJykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coJz8/Jyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHVybDogJy9Eb21haW5zL0FkZFJyc2V0JyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZDogaWQsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG5hbWU6IG5hbWUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IHR5cGUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnRlbnQ6IGNvbnRlbnQsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR0bDogdHRsLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXBsYWNlOiB0cnVlXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnIHx8IGRhdGEgPT09ICd0dGwgcmVwbGFjZWQnKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKCcuanMtYWRkUnJzZXRFcnJvcicpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKCcjcmVwbGFjZS1jb25maXJtJykuYWRkQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKCcuanMtYWRkUnJzZXRFcnJvcicsIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1hZGRScnNldEVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgICAgICQoJyNyZXBsYWNlLWNvbmZpcm0nKS5hZGRDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoJy5qcy1hZGRScnNldEVycm9yJywgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKCcuanMtYWRkUnJzZXRFcnJvcicsIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7XG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1kZWxldGUtcnJzZXQnLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgdGhpc0VsZW0gPSAkKHRoaXMpO1xyXG4gICAgdmFyIGlkID0gJCh0aGlzKS5kYXRhKCdpZCcpO1xyXG4gICAgdmFyIG5hbWUgPSAkKHRoaXMpLmRhdGEoJ25hbWUnKTtcclxuICAgIHZhciB0eXBlID0gJCh0aGlzKS5kYXRhKCd0eXBlJyk7XHJcbiAgICB2YXIgY29udGVudCA9ICQodGhpcykuZGF0YSgnY29udGVudCcpO1xyXG4gICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcignY2xpY2snLCBmdW5jdGlvbiAoZSkge1xyXG4gICAgICAgIGlmIChlLnRhcmdldCAmJiBlLnRhcmdldC5pZCA9PSAnZGVsZXRlLXJyc2V0LWNvbmZpcm0nKSB7XHJcbiAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICB1cmw6ICcvRG9tYWlucy9EZWxldGVScnNldCcsXHJcbiAgICAgICAgICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWQ6IGlkLFxyXG4gICAgICAgICAgICAgICAgICAgIG5hbWU6IG5hbWUsXHJcbiAgICAgICAgICAgICAgICAgICAgdHlwZTogdHlwZSxcclxuICAgICAgICAgICAgICAgICAgICBjb250ZW50OiBjb250ZW50XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICAgICAkKHRoaXNFbGVtKS5jbG9zZXN0KCd0cicpLnJlbW92ZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoJyNkZWxldGVScnNldE1vZGFsJykubW9kYWwoJ2hpZGUnKTtcclxuXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgZXJyb3I6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG4gICAgJCgnI2RlbGV0ZVJyc2V0TW9kYWwnKS5tb2RhbCgnc2hvdycpO1xyXG59KTtcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWVkaXQta2luZCcsIGZ1bmN0aW9uKCkge1xyXG4gICAgJCgnI2VkaXRLaW5kTW9kYWwnKS5tb2RhbCgnc2hvdycpO1xyXG5cclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1zYXZlS2luZENvbmZpcm0nLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgaWQgPSAkKHRoaXMpLmRhdGEoJ2lkJyk7XHJcbiAgICB2YXIga2luZCA9ICQoXCIjZWRpdEtpbmRUeXBlIDpzZWxlY3RlZFwiKS52YWwoKTtcclxuXHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9Eb21haW5zL0VkaXRLaW5kJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICBpZDogaWQsXHJcbiAgICAgICAgICAgIGtpbmQ6IGtpbmRcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtZWRpdEtpbmRFcnJvcicpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAgICAgICAgICAgICBBbGVydENoYW5nZSgnLmpzLWVkaXRLaW5kRXJyb3InLCAnRXJyb3IgcmV0dXJuZWQgZnJvbSB0aGUgc2VydmVyOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcjogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgICQoJy5qcy1lZGl0S2luZEVycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgQWxlcnRDaGFuZ2UoJy5qcy1lZGl0S2luZEVycm9yJywgJ0Vycm9yIHJldHVybmVkIGZyb20gdGhlIHNlcnZlcjogJyArIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1lZGl0LWlwcycsIGZ1bmN0aW9uICgpIHtcclxuICAgICQoJyNlZGl0SXBzTW9kYWwnKS5tb2RhbCgnc2hvdycpO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXNhdmVJcHNDb25maXJtJywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIGlkID0gJCh0aGlzKS5kYXRhKCdpZCcpO1xyXG4gICAgdmFyIGlwcyA9ICQoXCIjZWRpdElwc1R4YlwiKS52YWwoKTtcclxuICAgIGNvbnNvbGUubG9nKGlkKTtcclxuICAgIGNvbnNvbGUubG9nKGlwcyk7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9Eb21haW5zL0VkaXRJcHMnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIGlkOiBpZCxcclxuICAgICAgICAgICAgaXBzOiBpcHNcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtZWRpdElwc0Vycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKCcuanMtZWRpdElwc0Vycm9yJywgJ0Vycm9yIHJldHVybmVkIGZyb20gdGhlIHNlcnZlcjogJyArIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICAkKCcuanMtZWRpdElwc0Vycm9yJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgQWxlcnRDaGFuZ2UoJy5qcy1lZGl0SXBzRXJyb3InLCAnRXJyb3IgcmV0dXJuZWQgZnJvbSB0aGUgc2VydmVyOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KTtcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWVkaXQtcnJzZXQnLCBmdW5jdGlvbiAoKSB7XHJcblxyXG4gICAgdmFyIHRyRWxlbSA9ICQodGhpcykuY2xvc2VzdCgndHInKTtcclxuXHJcbiAgICAkKHRyRWxlbSkuZmluZCgnLmpzLWVkaXQtdHhiICcpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAkKHRyRWxlbSkuZmluZCgnLmpzLWVkaXQtdGV4dCAnKS5hZGRDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgJCh0ckVsZW0pLmZpbmQoJy5qcy1lZGl0LXJyc2V0JykuYWRkQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICQodHJFbGVtKS5maW5kKCcuanMtZGVsZXRlLXJyc2V0JykuYWRkQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICQodHJFbGVtKS5maW5kKCcuanMtc2F2ZS1ycnNldC1lZGl0ICcpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAkKHRyRWxlbSkuZmluZCgnLmpzLXN0b3AtZWRpdC1ycnNldCcpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtc3RvcC1lZGl0LXJyc2V0JywgZnVuY3Rpb24gKCkge1xyXG5cclxuICAgIHZhciB0ckVsZW0gPSAkKHRoaXMpLmNsb3Nlc3QoJ3RyJyk7XHJcblxyXG4gICAgJCh0ckVsZW0pLmZpbmQoJy5qcy1lZGl0LXR4YiAnKS5hZGRDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgJCh0ckVsZW0pLmZpbmQoJy5qcy1lZGl0LXRleHQgJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICQodHJFbGVtKS5maW5kKCcuanMtZWRpdC1ycnNldCcpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAkKHRyRWxlbSkuZmluZCgnLmpzLWRlbGV0ZS1ycnNldCcpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAkKHRyRWxlbSkuZmluZCgnLmpzLXNhdmUtcnJzZXQtZWRpdCAnKS5hZGRDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgJCh0ckVsZW0pLmZpbmQoJy5qcy1zdG9wLWVkaXQtcnJzZXQnKS5hZGRDbGFzcygnZGlzbm9uZScpO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWVkaXQtZXJyb3ItZGlzbWlzcycsIGZ1bmN0aW9uKCkge1xyXG4gICAgJCh0aGlzKS5jbG9zZXN0KCd0cicpLmFkZENsYXNzKCdkaXNub25lJyk7XHJcbn0pO1xyXG5cclxuZnVuY3Rpb24gY29tcGFyZU9sZEFuZE5ldyhkYXRhKSB7XHJcbiAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgIGlmIChkYXRhLm9sZE5hbWUgPT09IGRhdGEubmV3TmFtZSAmJiBkYXRhLm9sZFR5cGUgPT09IGRhdGEubmV3VHlwZSAmJiBkYXRhLm9sZENvbnRlbnQgPT09IGRhdGEubmV3Q29udGVudCAmJiBkYXRhLm9sZFR0bCA9PSBkYXRhLm5ld1R0bCApIHtcclxuICAgICAgICByZXR1cm4gdHJ1ZTtcclxuICAgIH1cclxuICAgIHJldHVybiBmYWxzZTtcclxufVxyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtc2F2ZS1ycnNldC1lZGl0JywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIHRyRWxlbSA9ICQodGhpcykuY2xvc2VzdCgndHInKTtcclxuICAgIHZhciBkYXRhdG9TZW5kID1cclxuICAgIHtcclxuICAgICAgICBpZDogJCh0aGlzKS5kYXRhKCdpZCcpLFxyXG4gICAgICAgIG9sZE5hbWU6ICQodGhpcykuZGF0YSgnbmFtZScpLFxyXG4gICAgICAgIG9sZFR5cGU6ICQodGhpcykuZGF0YSgndHlwZScpLFxyXG4gICAgICAgIG9sZENvbnRlbnQ6ICQodGhpcykuZGF0YSgnY29udGVudCcpLFxyXG4gICAgICAgIG9sZFR0bDogJCh0aGlzKS5kYXRhKCd0dGwnKSxcclxuICAgICAgICBuZXdOYW1lOiAkKHRyRWxlbSkuZmluZCgnLmpzLWVkaXQtdHhiW25hbWU9ZWRpdE5hbWVdJykudmFsKCksXHJcbiAgICAgICAgbmV3Q29udGVudDogJCh0ckVsZW0pLmZpbmQoJy5qcy1lZGl0LXR4YltuYW1lPWVkaXRDb250ZW50XScpLnZhbCgpLFxyXG4gICAgICAgIG5ld1R0bDogJCh0ckVsZW0pLmZpbmQoJy5qcy1lZGl0LXR4YltuYW1lPWVkaXRUdGxdJykudmFsKClcclxuICAgIH1cclxuICAgIGlmIChkYXRhdG9TZW5kLm5ld1R0bCA8IDYwKSB7XHJcbiAgICAgICAgJCh0ckVsZW0pLm5leHQoJ3RyJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAkKCcuanMtY29uZmlybS1lZGl0LXR0bCcpLmFkZENsYXNzKCdkaXNub25lJyk7XHJcbiAgICAgICAgJCh0ckVsZW0pLm5leHQoJ3RyJykuZmluZCgndGQ6Zmlyc3QtY2hpbGQoKScpLmh0bWwoJ1RoZSBUaW1lIFRvIExpdmUgc2hvdWxkIG5vdCBiZSBsb3dlciB0aGFuIDYwJyk7XHJcbiAgICAgICAgcmV0dXJuO1xyXG4gICAgfSBcclxuICAgIGNvbnNvbGUubG9nKGNvbXBhcmVPbGRBbmROZXcoZGF0YXRvU2VuZCkpO1xyXG4gICAgaWYgKGNvbXBhcmVPbGRBbmROZXcoZGF0YXRvU2VuZCkpIHtcclxuICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICAgIGNvbnNvbGUubG9nKGRhdGF0b1NlbmQpO1xyXG4gICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgIHVybDogJy9Eb21haW5zL0VkaXRScnNldCcsXHJcbiAgICAgICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICAgICAgZGF0YTogZGF0YXRvU2VuZCxcclxuICAgICAgICAgICAgc3VjY2VzczogZnVuY3Rpb24oZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIGlmIChkYXRhID09PSAndHRsIHJlcGxhY2VkJykge1xyXG4gICAgICAgICAgICAgICAgICAgICQoJy5qcy1jb25maXJtLWVkaXQtdHRsJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgICAgICAgICAkKHRyRWxlbSkubmV4dCgndHInKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgICAgICAgICAgICAgICAgICQodHJFbGVtKS5uZXh0KCd0cicpLmZpbmQoJ3RkOmZpcnN0LWNoaWxkKCknKS5odG1sKFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAnSWYgeW91IGNoYW5nZSB0aGUgdGltZSB0byBsaXZlIGZyb20gdGhpcyByZWNvcmQgZXZlcnkgcmVjb3JkIG9mIHR5cGU6IFwiJyArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRhdGF0b1NlbmQubmV3VHlwZSArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICdcIiB3aWxsIGdldCB0aGUgc2FtZSB0aW1lIHRvIGxpdmUnKTtcclxuICAgICAgICAgICAgICAgICAgICBkYXRhdG9TZW5kLnJlcGxhY2UgPSB0cnVlO1xyXG4gICAgICAgICAgICAgICAgICAgIHZhciBlbGVtRXZlbnRMaXN0ZW5lciA9ICQodHJFbGVtKS5uZXh0KCd0cicpLmZpbmQoJy5qcy1jb25maXJtLWVkaXQtdHRsJyk7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coZWxlbUV2ZW50TGlzdGVuZXIpO1xyXG4gICAgICAgICAgICAgICAgICAgIGVsZW1FdmVudExpc3RlbmVyWzBdLmFkZEV2ZW50TGlzdGVuZXIoJ2NsaWNrJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgZnVuY3Rpb24oZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coJ3Nob3VsZCBkbyBzb21ldGhpbmc/Jyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHVybDogJy9Eb21haW5zL0VkaXRScnNldCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IGRhdGF0b1NlbmQsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VjY2VzczogZnVuY3Rpb24oZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJyB8fCBkYXRhID09PSAndHRsIHJlcGxhY2VkJykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlbG9hZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICApO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICAkKCcuanMtY29uZmlybS1lZGl0LXR0bCcpLmFkZENsYXNzKCdkaXNub25lJyk7XHJcbiAgICAgICAgICAgICAgICAgICAgJCh0ckVsZW0pLm5leHQoJ3RyJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKTtcclxuICAgICAgICAgICAgICAgICAgICAkKHRyRWxlbSkubmV4dCgndHInKS5maW5kKCd0ZDpmaXJzdC1jaGlsZCgpJykuaHRtbCgnRXJyb3IgcmV0dXJuZWQgZnJvbSB0aGUgc2VydmVyOiAnICsgZGF0YSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIGVycm9yOiBmdW5jdGlvbihkYXRhKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG59KTtcbiQoJyN0b2dnbGVFeGNsdWRlU2lnbmluZycpLmNoYW5nZShmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgdmFsID0gJCh0aGlzKS5wcm9wKCdjaGVja2VkJyk7XHJcbiAgICB2YXIgZG9tYWluSWQgPSAkKHRoaXMpLmRhdGEoJ2RvbWFpbmlkJyk7XHJcbiAgICB2YXIgZWxlbSA9ICcuanMtbm90aWZ5UmVjdGlmeUVycm9yJztcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL0RvbWFpbnMvQ2hhbmdlRXhjbHVkZVNpZ25pbmcnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgIGRvbWFpbklkOiBkb21haW5JZCxcclxuICAgICAgICAgICAgZXhjbHVkZVNpZ25pbmc6IHZhbFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgIGlmIChkYXRhID09PSAnc3VjY2VzcycpIHtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1ub3RpZnlSZWN0aWZ5RXJyb3InKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgJCgnLmpzLW5vdGlmeVJlY3RpZnlFcnJvcicpLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KVxuZnVuY3Rpb24gQWxlcnRDaGFuZ2UoZWxlbSwgYm9keSwgdHlwZSA9ICdzZWNvbmRhcnknKSB7XHJcbiAgICBjb25zb2xlLmxvZyh0eXBlKTtcclxuICAgIHN3aXRjaCAodHlwZSkge1xyXG4gICAgY2FzZSAnZGFuZ2VyJzpcclxuICAgICAgICAkKGVsZW0pLnJlbW92ZUNsYXNzKCdhbGVydC1zZWNvbmRhcnknKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKTtcclxuICAgICAgICBicmVhaztcclxuICAgIGNhc2UgJ3N1Y2Nlc3MnOlxyXG4gICAgICAgICQoZWxlbSkucmVtb3ZlQ2xhc3MoJ2FsZXJ0LXNlY29uZGFyeScpLnJlbW92ZUNsYXNzKCdhbGVydC1kYW5nZXInKS5hZGRDbGFzcygnYWxlcnQtc3VjY2VzcycpO1xyXG4gICAgICAgIGJyZWFrO1xyXG4gICAgZGVmYXVsdDpcclxuICAgICAgICAkKGVsZW0pLnJlbW92ZUNsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpLmFkZENsYXNzKCdhbGVydC1zZWNvbmRhcnknKTtcclxuICAgICAgICBicmVhaztcclxuICAgIH1cclxuICAgICQoZWxlbSArICcgLmFsZXJ0Q3Jvc3NfX3RleHQnKS5odG1sKGJvZHkpO1xyXG59XG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1idG4tc2lnbicsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBlbGVtID0gJy5qcy1zaWduRXJyb3InO1xyXG4gICAgJChlbGVtKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvRG9tYWlucy9TaWduJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICBJZDogJCh0aGlzKS5kYXRhKCdpZCcpXHJcbiAgICAgICAgfSxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICBBbGVydENoYW5nZShlbGVtLCAnRG9tYWluIHdpbGwgc29vbiBiZSBzaWduZWQgYnkgdGhlIHNjaGVkdWxlcicsICdzdWNjZXNzJyk7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICBBbGVydENoYW5nZShlbGVtLCAnU2VydmVyIHJldHVybmVkIHRoZSBlcnJvcjogJyArIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KTtcclxuXHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1idG4tdW5zaWduJywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIGVsZW0gPSAnLmpzLXNpZ25FcnJvcic7XHJcbiAgICAkKGVsZW0pLnJlbW92ZUNsYXNzKCdkaXNub25lJyk7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9Eb21haW5zL1Vuc2lnbicsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IHtcclxuICAgICAgICAgICAgSWQ6ICQodGhpcykuZGF0YSgnaWQnKVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ0RvbWFpbiB3aWxsIHNvb24gYmUgdW5zaWduZWQgYnkgdGhlIHNjaGVkdWxlcicsICdzdWNjZXNzJyk7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVsb2FkKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICBBbGVydENoYW5nZShlbGVtLCAnU2VydmVyIHJldHVybmVkIHRoZSBlcnJvcjogJyArIGRhdGEsICdkYW5nZXInKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZXJyb3I6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWJ0bi1yb2xsb3ZlcicsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBlbGVtID0gJy5qcy1zaWduRXJyb3InO1xyXG4gICAgJChlbGVtKS5yZW1vdmVDbGFzcygnZGlzbm9uZScpO1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvRG9tYWlucy9Sb2xsb3ZlcicsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IHtcclxuICAgICAgICAgICAgSWQ6ICQodGhpcykuZGF0YSgnaWQnKVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ0tleSBSb2xsb3ZlciBwcm9jZXNzIHdpbGwgYmUgc3RhcnRlZCBieSB0aGUgc2NoZWR1bGVyJywgJ3N1Y2Nlc3MnKTtcclxuICAgICAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZWxvYWQoKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgIEFsZXJ0Q2hhbmdlKGVsZW0sICdTZXJ2ZXIgcmV0dXJuZWQgdGhlIGVycm9yOiAnICsgZGF0YSwgJ2RhbmdlcicpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcjogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgQWxlcnRDaGFuZ2UoZWxlbSwgJ1NlcnZlciByZXR1cm5lZCB0aGUgZXJyb3I6ICcgKyBkYXRhLCAnZGFuZ2VyJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0pO1xuIl19
