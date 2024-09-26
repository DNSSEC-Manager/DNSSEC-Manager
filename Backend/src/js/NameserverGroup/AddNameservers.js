$('body').on('click', '.js-add-nameserver', function () {
    copyNameserver();
});
$('body').on('propertychange input', '.js-nameServerInput', function (e) {
    var parent = $(this).closest('.js-nameserverFromgroup');
    if ($(parent).is(':last-child')) {
        copyNameserver();
    }
});

$('body').on('click', '.js-delete-nameserver', function () {
    console.log($('.js-nameservers .js-nameserverFromgroup').length);
    if ($('.js-nameservers .js-nameserverFromgroup').length > 1) {
        $(this).closest('.js-nameserverFromgroup').remove();
    }
    fixNrs();

});

function copyNameserver() {
    $('.js-copyNameserver').clone().appendTo('.js-nameservers').removeClass('disnone').removeClass('js-copyNameserver');
    fixNrs();
}

function fixNrs() {
    var count = 1;
    $('.js-nameserver-nr').each(function () {
        $(this).html(count);
        count++;
    });
}