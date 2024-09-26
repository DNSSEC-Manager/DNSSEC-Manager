function randomString(len, beforestr = '', arraytocheck = null) {
    // Charset, every character in this string is an optional one it can use as a random character.
    var charSet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890';
    var randomString = '';
    for (var i = 0; i < len; i++) {
        // creates a random number between 0 and the charSet length. Rounds it down to a whole number
        var randomPoz = Math.floor(Math.random() * charSet.length);
        randomString += charSet.substring(randomPoz, randomPoz + 1);
    }
    // If an array is given it will check the array, and if the generated string exists in it it will create a new one until a unique one is found *WATCH OUT. If all available options are used it will cause a loop it cannot break out*
    if (arraytocheck == null) {
        return beforestr + randomString;
    } else {
        var isIn = $.inArray(beforestr + randomString, arraytocheck); // checks if the string is in the array, returns a position
        if (isIn > -1) {
            // if the position is not -1 (meaning, it is not in the array) it will start doing a loop
            var count = 0;
            do {
                randomString = '';
                for (var i = 0; i < len; i++) {
                    var randomPoz = Math.floor(Math.random() * charSet.length);
                    randomString += charSet.substring(randomPoz, randomPoz + 1);
                }
                isIn = $.inArray(beforestr + randomString, arraytocheck);
                count++;
            } while (isIn > -1);
            return beforestr + randomString;
        } else {
            return beforestr + randomString;
        }
    }
}

$('body').on('click', '.js-generate-auth', function () {
    var newAuth = randomString(30);
    $('.js-auth-token').val(newAuth);
});

$('body').on('click', '.js-copy-code', function () {
    //    var copyText = document.getElementById('AuthToken');
    //
    //    /* Select the text field */
    //    copyText.select();
    //
    //    /* Copy the text inside the text field */
    //    document.execCommand('copy');

    var auth = $('#AuthToken').val();
    //var str = 'api=yes\napi-key=' + auth + '\nwebserver-address=<your PowerDNS server IP>\nwebserver-port=8081\nwebserver-allow-from=<your backend IP, separate with comma if you have more IP’s to allow>';
    //console.log(str);
    copyStringToClipboard(auth);
});

function copyStringToClipboard(str) {
    // Create new element
    var el = document.createElement('textarea');
    // Set value (string to be copied)
    el.value = str;
    // Set non-editable to avoid focus and move outside of view
    el.setAttribute('readonly', '');
    el.style = { position: 'absolute', left: '-9999px' };
    document.body.appendChild(el);
    // Select text inside element
    el.select();
    // Copy text to clipboard
    document.execCommand('copy');
    // Remove temporary element
    document.body.removeChild(el);
}
$('body').on('click', '.js-test-connection', function () {
    testConnection();
});

function testConnection() {
    var name = $('#Name').val();
    var url = $('#BaseUrl').val();
    var apiKey = $('#AuthToken').val();
    var myData = { url: url, apiKey: apiKey };
    $('.js-connection-error').removeClass('alert-danger').removeClass('alert-success').html('Testing connection...');
    $.ajax({
        url: '/DnsServers/CheckConnectionLive',
        type: 'POST',
        data: myData,
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                $('.js-connection-error').addClass('alert-success').html('Connection succesfully established');
            } else {
                $('.js-connection-error').addClass('alert-danger').html('An error was returned: ' + data);
            }
        },
        error: function(data) {
            $('.js-connection-error').addClass('alert-danger').html('Failed to make a connection to the server');
        }
    });
}
//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImNyZWF0ZURucy5qcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EiLCJmaWxlIjoiY3JlYXRlRG5zLmpzIiwic291cmNlc0NvbnRlbnQiOlsiZnVuY3Rpb24gcmFuZG9tU3RyaW5nKGxlbiwgYmVmb3Jlc3RyID0gJycsIGFycmF5dG9jaGVjayA9IG51bGwpIHtcclxuICAgIC8vIENoYXJzZXQsIGV2ZXJ5IGNoYXJhY3RlciBpbiB0aGlzIHN0cmluZyBpcyBhbiBvcHRpb25hbCBvbmUgaXQgY2FuIHVzZSBhcyBhIHJhbmRvbSBjaGFyYWN0ZXIuXHJcbiAgICB2YXIgY2hhclNldCA9ICdBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWmFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6MTIzNDU2Nzg5MCc7XHJcbiAgICB2YXIgcmFuZG9tU3RyaW5nID0gJyc7XHJcbiAgICBmb3IgKHZhciBpID0gMDsgaSA8IGxlbjsgaSsrKSB7XHJcbiAgICAgICAgLy8gY3JlYXRlcyBhIHJhbmRvbSBudW1iZXIgYmV0d2VlbiAwIGFuZCB0aGUgY2hhclNldCBsZW5ndGguIFJvdW5kcyBpdCBkb3duIHRvIGEgd2hvbGUgbnVtYmVyXHJcbiAgICAgICAgdmFyIHJhbmRvbVBveiA9IE1hdGguZmxvb3IoTWF0aC5yYW5kb20oKSAqIGNoYXJTZXQubGVuZ3RoKTtcclxuICAgICAgICByYW5kb21TdHJpbmcgKz0gY2hhclNldC5zdWJzdHJpbmcocmFuZG9tUG96LCByYW5kb21Qb3ogKyAxKTtcclxuICAgIH1cclxuICAgIC8vIElmIGFuIGFycmF5IGlzIGdpdmVuIGl0IHdpbGwgY2hlY2sgdGhlIGFycmF5LCBhbmQgaWYgdGhlIGdlbmVyYXRlZCBzdHJpbmcgZXhpc3RzIGluIGl0IGl0IHdpbGwgY3JlYXRlIGEgbmV3IG9uZSB1bnRpbCBhIHVuaXF1ZSBvbmUgaXMgZm91bmQgKldBVENIIE9VVC4gSWYgYWxsIGF2YWlsYWJsZSBvcHRpb25zIGFyZSB1c2VkIGl0IHdpbGwgY2F1c2UgYSBsb29wIGl0IGNhbm5vdCBicmVhayBvdXQqXHJcbiAgICBpZiAoYXJyYXl0b2NoZWNrID09IG51bGwpIHtcclxuICAgICAgICByZXR1cm4gYmVmb3Jlc3RyICsgcmFuZG9tU3RyaW5nO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgICB2YXIgaXNJbiA9ICQuaW5BcnJheShiZWZvcmVzdHIgKyByYW5kb21TdHJpbmcsIGFycmF5dG9jaGVjayk7IC8vIGNoZWNrcyBpZiB0aGUgc3RyaW5nIGlzIGluIHRoZSBhcnJheSwgcmV0dXJucyBhIHBvc2l0aW9uXHJcbiAgICAgICAgaWYgKGlzSW4gPiAtMSkge1xyXG4gICAgICAgICAgICAvLyBpZiB0aGUgcG9zaXRpb24gaXMgbm90IC0xIChtZWFuaW5nLCBpdCBpcyBub3QgaW4gdGhlIGFycmF5KSBpdCB3aWxsIHN0YXJ0IGRvaW5nIGEgbG9vcFxyXG4gICAgICAgICAgICB2YXIgY291bnQgPSAwO1xyXG4gICAgICAgICAgICBkbyB7XHJcbiAgICAgICAgICAgICAgICByYW5kb21TdHJpbmcgPSAnJztcclxuICAgICAgICAgICAgICAgIGZvciAodmFyIGkgPSAwOyBpIDwgbGVuOyBpKyspIHtcclxuICAgICAgICAgICAgICAgICAgICB2YXIgcmFuZG9tUG96ID0gTWF0aC5mbG9vcihNYXRoLnJhbmRvbSgpICogY2hhclNldC5sZW5ndGgpO1xyXG4gICAgICAgICAgICAgICAgICAgIHJhbmRvbVN0cmluZyArPSBjaGFyU2V0LnN1YnN0cmluZyhyYW5kb21Qb3osIHJhbmRvbVBveiArIDEpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgaXNJbiA9ICQuaW5BcnJheShiZWZvcmVzdHIgKyByYW5kb21TdHJpbmcsIGFycmF5dG9jaGVjayk7XHJcbiAgICAgICAgICAgICAgICBjb3VudCsrO1xyXG4gICAgICAgICAgICB9IHdoaWxlIChpc0luID4gLTEpO1xyXG4gICAgICAgICAgICByZXR1cm4gYmVmb3Jlc3RyICsgcmFuZG9tU3RyaW5nO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIHJldHVybiBiZWZvcmVzdHIgKyByYW5kb21TdHJpbmc7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1nZW5lcmF0ZS1hdXRoJywgZnVuY3Rpb24gKCkge1xyXG4gICAgdmFyIG5ld0F1dGggPSByYW5kb21TdHJpbmcoMzApO1xyXG4gICAgJCgnLmpzLWF1dGgtdG9rZW4nKS52YWwobmV3QXV0aCk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtY29weS1jb2RlJywgZnVuY3Rpb24gKCkge1xyXG4gICAgLy8gICAgdmFyIGNvcHlUZXh0ID0gZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoJ0F1dGhUb2tlbicpO1xyXG4gICAgLy9cclxuICAgIC8vICAgIC8qIFNlbGVjdCB0aGUgdGV4dCBmaWVsZCAqL1xyXG4gICAgLy8gICAgY29weVRleHQuc2VsZWN0KCk7XHJcbiAgICAvL1xyXG4gICAgLy8gICAgLyogQ29weSB0aGUgdGV4dCBpbnNpZGUgdGhlIHRleHQgZmllbGQgKi9cclxuICAgIC8vICAgIGRvY3VtZW50LmV4ZWNDb21tYW5kKCdjb3B5Jyk7XHJcblxyXG4gICAgdmFyIGF1dGggPSAkKCcjQXV0aFRva2VuJykudmFsKCk7XHJcbiAgICAvL3ZhciBzdHIgPSAnYXBpPXllc1xcbmFwaS1rZXk9JyArIGF1dGggKyAnXFxud2Vic2VydmVyLWFkZHJlc3M9PHlvdXIgUG93ZXJETlMgc2VydmVyIElQPlxcbndlYnNlcnZlci1wb3J0PTgwODFcXG53ZWJzZXJ2ZXItYWxsb3ctZnJvbT08eW91ciBiYWNrZW5kIElQLCBzZXBhcmF0ZSB3aXRoIGNvbW1hIGlmIHlvdSBoYXZlIG1vcmUgSVDigJlzIHRvIGFsbG93Pic7XHJcbiAgICAvL2NvbnNvbGUubG9nKHN0cik7XHJcbiAgICBjb3B5U3RyaW5nVG9DbGlwYm9hcmQoYXV0aCk7XHJcbn0pO1xyXG5cclxuZnVuY3Rpb24gY29weVN0cmluZ1RvQ2xpcGJvYXJkKHN0cikge1xyXG4gICAgLy8gQ3JlYXRlIG5ldyBlbGVtZW50XHJcbiAgICB2YXIgZWwgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCd0ZXh0YXJlYScpO1xyXG4gICAgLy8gU2V0IHZhbHVlIChzdHJpbmcgdG8gYmUgY29waWVkKVxyXG4gICAgZWwudmFsdWUgPSBzdHI7XHJcbiAgICAvLyBTZXQgbm9uLWVkaXRhYmxlIHRvIGF2b2lkIGZvY3VzIGFuZCBtb3ZlIG91dHNpZGUgb2Ygdmlld1xyXG4gICAgZWwuc2V0QXR0cmlidXRlKCdyZWFkb25seScsICcnKTtcclxuICAgIGVsLnN0eWxlID0geyBwb3NpdGlvbjogJ2Fic29sdXRlJywgbGVmdDogJy05OTk5cHgnIH07XHJcbiAgICBkb2N1bWVudC5ib2R5LmFwcGVuZENoaWxkKGVsKTtcclxuICAgIC8vIFNlbGVjdCB0ZXh0IGluc2lkZSBlbGVtZW50XHJcbiAgICBlbC5zZWxlY3QoKTtcclxuICAgIC8vIENvcHkgdGV4dCB0byBjbGlwYm9hcmRcclxuICAgIGRvY3VtZW50LmV4ZWNDb21tYW5kKCdjb3B5Jyk7XHJcbiAgICAvLyBSZW1vdmUgdGVtcG9yYXJ5IGVsZW1lbnRcclxuICAgIGRvY3VtZW50LmJvZHkucmVtb3ZlQ2hpbGQoZWwpO1xyXG59XG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy10ZXN0LWNvbm5lY3Rpb24nLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB0ZXN0Q29ubmVjdGlvbigpO1xyXG59KTtcclxuXHJcbmZ1bmN0aW9uIHRlc3RDb25uZWN0aW9uKCkge1xyXG4gICAgdmFyIG5hbWUgPSAkKCcjTmFtZScpLnZhbCgpO1xyXG4gICAgdmFyIHVybCA9ICQoJyNCYXNlVXJsJykudmFsKCk7XHJcbiAgICB2YXIgYXBpS2V5ID0gJCgnI0F1dGhUb2tlbicpLnZhbCgpO1xyXG4gICAgdmFyIG15RGF0YSA9IHsgdXJsOiB1cmwsIGFwaUtleTogYXBpS2V5IH07XHJcbiAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvcicpLnJlbW92ZUNsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpLmh0bWwoJ1Rlc3RpbmcgY29ubmVjdGlvbi4uLicpO1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvRG5zU2VydmVycy9DaGVja0Nvbm5lY3Rpb25MaXZlJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YTogbXlEYXRhLFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvcicpLmFkZENsYXNzKCdhbGVydC1zdWNjZXNzJykuaHRtbCgnQ29ubmVjdGlvbiBzdWNjZXNmdWxseSBlc3RhYmxpc2hlZCcpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLWNvbm5lY3Rpb24tZXJyb3InKS5hZGRDbGFzcygnYWxlcnQtZGFuZ2VyJykuaHRtbCgnQW4gZXJyb3Igd2FzIHJldHVybmVkOiAnICsgZGF0YSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIGVycm9yOiBmdW5jdGlvbihkYXRhKSB7XHJcbiAgICAgICAgICAgICQoJy5qcy1jb25uZWN0aW9uLWVycm9yJykuYWRkQ2xhc3MoJ2FsZXJ0LWRhbmdlcicpLmh0bWwoJ0ZhaWxlZCB0byBtYWtlIGEgY29ubmVjdGlvbiB0byB0aGUgc2VydmVyJyk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn0iXX0=
