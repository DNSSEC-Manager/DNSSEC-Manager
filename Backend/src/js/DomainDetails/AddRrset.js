
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