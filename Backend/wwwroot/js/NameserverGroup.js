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
//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIk5hbWVzZXJ2ZXJHcm91cC5qcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSIsImZpbGUiOiJOYW1lc2VydmVyR3JvdXAuanMiLCJzb3VyY2VzQ29udGVudCI6WyIkKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1hZGQtbmFtZXNlcnZlcicsIGZ1bmN0aW9uICgpIHtcclxuICAgIGNvcHlOYW1lc2VydmVyKCk7XHJcbn0pO1xyXG4kKCdib2R5Jykub24oJ3Byb3BlcnR5Y2hhbmdlIGlucHV0JywgJy5qcy1uYW1lU2VydmVySW5wdXQnLCBmdW5jdGlvbiAoZSkge1xyXG4gICAgdmFyIHBhcmVudCA9ICQodGhpcykuY2xvc2VzdCgnLmpzLW5hbWVzZXJ2ZXJGcm9tZ3JvdXAnKTtcclxuICAgIGlmICgkKHBhcmVudCkuaXMoJzpsYXN0LWNoaWxkJykpIHtcclxuICAgICAgICBjb3B5TmFtZXNlcnZlcigpO1xyXG4gICAgfVxyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWRlbGV0ZS1uYW1lc2VydmVyJywgZnVuY3Rpb24gKCkge1xyXG4gICAgY29uc29sZS5sb2coJCgnLmpzLW5hbWVzZXJ2ZXJzIC5qcy1uYW1lc2VydmVyRnJvbWdyb3VwJykubGVuZ3RoKTtcclxuICAgIGlmICgkKCcuanMtbmFtZXNlcnZlcnMgLmpzLW5hbWVzZXJ2ZXJGcm9tZ3JvdXAnKS5sZW5ndGggPiAxKSB7XHJcbiAgICAgICAgJCh0aGlzKS5jbG9zZXN0KCcuanMtbmFtZXNlcnZlckZyb21ncm91cCcpLnJlbW92ZSgpO1xyXG4gICAgfVxyXG4gICAgZml4TnJzKCk7XHJcblxyXG59KTtcclxuXHJcbmZ1bmN0aW9uIGNvcHlOYW1lc2VydmVyKCkge1xyXG4gICAgJCgnLmpzLWNvcHlOYW1lc2VydmVyJykuY2xvbmUoKS5hcHBlbmRUbygnLmpzLW5hbWVzZXJ2ZXJzJykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKS5yZW1vdmVDbGFzcygnanMtY29weU5hbWVzZXJ2ZXInKTtcclxuICAgIGZpeE5ycygpO1xyXG59XHJcblxyXG5mdW5jdGlvbiBmaXhOcnMoKSB7XHJcbiAgICB2YXIgY291bnQgPSAxO1xyXG4gICAgJCgnLmpzLW5hbWVzZXJ2ZXItbnInKS5lYWNoKGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAkKHRoaXMpLmh0bWwoY291bnQpO1xyXG4gICAgICAgIGNvdW50Kys7XHJcbiAgICB9KTtcclxufSJdfQ==
