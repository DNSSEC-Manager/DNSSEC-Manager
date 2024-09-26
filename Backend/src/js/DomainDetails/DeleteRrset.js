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