function randomString(len, beforestr = '', arraytocheck = null) {
    // Charset, every character in this string is an optional one it can use as a random character.
    var charSet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
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
    var str = 'api=yes\napi-key=' + auth + '\nwebserver-address=<your PowerDNS server IP>\nwebserver-port=8081\nwebserver-allow-from=<your backend IP, separate with comma if you have more IP’s to allow>';
    console.log(str);
    copyStringToClipboard(str);
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
$(document).ready(function () {
    $('.js-connection-error').html('Checking for already filled in details...');
    $.ajax({
        url: '/Wizard/GetDnsServer',
        type: 'POST',
        success: function (data) {
            console.log(data);
            if (data !== 'Failed') {
                $('.js-connection-error').addClass('alert-success').html('Details found!');
                var stuff = JSON.parse(data);
                console.log(stuff);
                $('#Name').val(stuff.Name);
                $('#Url').val(stuff.BaseUrl);
                $('#AuthToken').val(stuff.AuthToken);
                $('.js-submit-dns').html('Save Changes & Next').removeClass('js-submit-dns').addClass('js-save-changes-dns');

                setTimeout(function () {
                    $('.js-connection-error').removeClass('alert-success').html('Connection has not been tested yet');
                }, 800);
            }
        }
    });

});

$('.submit').click(function () {
    return false;
});

$('body').on('click', '.js-test-connection', function () {
    testConnection();
});

$('body').on('click', '.js-submit-dns', function () {
    testConnection(true);
});

$('body').on('click', '.js-save-changes-dns', function () {
    testConnection(false, true);
});

function testConnection(submit = false, saveChangesE = false) {
    var name = $('#Name').val();
    var url = $('#Url').val();
    var apiKey = $('#AuthToken').val();
    var myData = { url: url, apiKey: apiKey }
    var token = $('input[name="__RequestVerificationToken"]', $('#msform')).val();
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });
    var myDataSubmit = { name: name, url: url, apiKey: apiKey }
    var dataSubmit = $.extend(myDataSubmit, { '__RequestVerificationToken': token });
    console.log(name)
    $('.js-connection-error').removeClass('alert-danger').removeClass('alert-success').html('Testing connection...');
    $.ajax({
        url: '/Wizard/CheckConnection',
        type: 'POST',
        data: dataWithAntiforgeryToken,
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                $('.js-connection-error').addClass('alert-success').html('Connection succesfully established');
                if (submit) {
                    submitDns(dataSubmit);
                }
                if (saveChangesE) {
                    saveChanges(dataSubmit);
                }
            } else {
                $('.js-connection-error').addClass('alert-danger').html('An error was returned: ' + data);
            }
        }
    });
}

function saveChanges(formData) {
    $.ajax({
        url: '/Wizard/SaveChangesDns',
        type: 'POST',
        data: formData,
        success: function (data) {
            console.log(data);
            if (data == 'success') {
                $('.js-submit-dns').html('Save Changes & Next').removeClass('js-submit-dns').addClass('js-save-changes-dns');
                var thisFs = $('.dnsFs')[0];
                var nextFs = $('.nameserverFs')[0];
                next(thisFs, nextFs);
            }
        }
    });
}

function submitDns(formData) {
    $.ajax({
        url: '/Wizard/CreateDns',
        type: 'POST',
        data: formData,
        success: function (data) {
            console.log(data);
            if (data == 'success') {
                $('.js-submit-dns').html('Save Changes & Next').removeClass('js-submit-dns').addClass('js-save-changes-dns');
                var thisFs = $('.dnsFs')[0];
                var nextFs = $('.nameserverFs')[0];
                next(thisFs, nextFs);
            }
        }
    });
}
$('body').on('click', '.js-plus-nameserver', function () {
    $('.js-nameserverGroupTocopy').clone().appendTo('.js-nameserverGroup-copyto').css('display', 'block');
    resetGroupNumbers();
});

$('body').on('click', '.js-submit-nameserver', function () {
    var num = $('.js-nameserverGroup').length;
    var count = 0;
    $('.js-nameserverGroup').each(function (index) {
        var id = $(this).data('id');
        
        var newObj = {
            name: $(this).find('.js-nameserverGroupName').val(),
            ns01: $(this).find('.js-nameServerGroupServer1').val(),
            ns02: $(this).find('.js-nameServerGroupServer2').val(),
            ns03: $(this).find('.js-nameServerGroupServer3').val(),
            ns04: $(this).find('.js-nameServerGroupServer4').val(),
            ns05: $(this).find('.js-nameServerGroupServer5').val(),
            ns06: $(this).find('.js-nameServerGroupServer6').val(),
            ns07: $(this).find('.js-nameServerGroupServer7').val(),
            ns08: $(this).find('.js-nameServerGroupServer8').val(),
            ns09: $(this).find('.js-nameServerGroupServer9').val(),
            ns10: $(this).find('.js-nameServerGroupServer10').val(),
            id: id
        };

        var token = $('input[name="__RequestVerificationToken"]', $('#msform')).val();
        var dataWithAntiforgeryToken = $.extend(newObj, { '__RequestVerificationToken': token });
        $.ajax({
            url: '/Wizard/CreateNameservers',
            type: 'POST',
            data: dataWithAntiforgeryToken,
            done: function(data) {
                
            },
            success: function (data) {
                console.log(data);
                count++;
                if (count === num) {
                    var thisFs = $('.nameserverFs')[0];
                    var nextFs = $('.registryFs')[0];
                    next(thisFs, nextFs);
                }
            },
            statusCode: {
                500: function () {
                    alert("Script exhausted");
                }
            }
        });
    });
});

function resetGroupNumbers() {
    $('.nameserverGroup__title').each(function (index) {
        $(this).html('Nameserver group ' + (index + 1));
    });
}

$(document).ready(function () {
    $.ajax({
        url: '/Wizard/GetNameservers',
        type: 'POST',
        success: function (response) {
            console.log(response);
            if (response !== 'failed') {
                var data = JSON.parse(response);
                data.map(function (elem, index) {
                    var copiedElem = $('.js-nameserverGroupTocopy').clone().appendTo('.js-nameserverGroup-copyto').removeClass('js-nameserverGroupTocopy').removeClass('disnone').addClass('js-nameserverGroup')[0];
                    $(copiedElem).data('id', elem.Id).attr('data-id', elem.Id);
                    $(copiedElem).find('.js-nameserverGroupName').val(elem.Name);
                    $(copiedElem).find('.js-nameServerGroupServer1').val(elem.Ns01);
                    $(copiedElem).find('.js-nameServerGroupServer2').val(elem.Ns02);
                    $(copiedElem).find('.js-nameServerGroupServer3').val(elem.Ns03);
                    $(copiedElem).find('.js-nameServerGroupServer4').val(elem.Ns04);
                    $(copiedElem).find('.js-nameServerGroupServer5').val(elem.Ns05);
                    $(copiedElem).find('.js-nameServerGroupServer6').val(elem.Ns06);
                    $(copiedElem).find('.js-nameServerGroupServer7').val(elem.Ns07);
                    $(copiedElem).find('.js-nameServerGroupServer8').val(elem.Ns08);
                    $(copiedElem).find('.js-nameServerGroupServer9').val(elem.Ns09);
                    $(copiedElem).find('.js-nameServerGroupServer10').val(elem.Ns10);
                    resetGroupNumbers();
                });
                $('.js-nameserverGroup:first-child()').remove();
            }
        }
    });

});
$(document).ready(function () {
    $('.js-connection-error-registry').html('Checking for already filled in details...');
    $.ajax({
        url: '/Wizard/GetRegistry',
        type: 'POST',
        success: function (data) {
            console.log(data);
            if (data !== 'Failed') {
                $('.js-connection-error-registry').addClass('alert-success').html('Details found!');
                var stuff = JSON.parse(data);
                console.log(stuff);
                $('#NameRegistry').val(stuff.Name);
                $('#UrlRegistry').val(stuff.Url);
                $('#PortRegistry').val(stuff.Port);
                $('#UserNameRegistry').val(stuff.Username);
                $('#PasswordRegistry').val(stuff.Password);
                $('.js-submit-registry').html('Save Changes & Next').removeClass('js-submit-registry').addClass('js-save-changes-registry');

                setTimeout(function () {
                    $('.js-connection-error-registry').removeClass('alert-success').html('Connection has not been tested yet');
                }, 800);
            }
        }
    });

});

$('body').on('click', '.js-test-connection-registry', function () {
    testConnectionRegistry();
});

$('body').on('click', '.js-submit-registry', function () {
    testConnectionRegistry(1);
});

$('body').on('click', '.js-save-changes-registry', function () {
    testConnectionRegistry(0, 1);
});


function submitRegistry(data) {
    $.ajax({
        url: '/Wizard/CreateRegistry',
        type: 'POST',
        data: data,
        success: function (data) {
            if (data === 'success') {
                var thisFs = $('.registryFs')[0];
                var nextFs = $('.tldFs')[0];
                next(thisFs, nextFs);
            }
        }
    });
}

function saveChangesRegistry(formData) {
    $.ajax({
        url: '/Wizard/SaveChangesRegistry',
        type: 'POST',
        data: formData,
        success: function (data) {
            console.log(data);
            if (data === 'success') {
                var thisFs = $('.registryFs')[0];
                var nextFs = $('.tldFs')[0];
                next(thisFs, nextFs);
            }
        }
    });
}

function testConnectionRegistry(submit = false, saveChanges = false) {
    var name = $('#NameRegistry').val();
    var url = $('#UrlRegistry').val();
    var port = $('#PortRegistry').val();
    var username = $('#UserNameRegistry').val();
    var password = $('#PasswordRegistry').val();
    var myData = {
        name: name,
        url: url,
        port: port,
        username: username,
        password: password
    }
    var token = $('input[name="__RequestVerificationToken"]', $('#msform')).val();
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });
    $('.js-connection-error-registry').removeClass('alert-success').removeClass('alert-danger')
        .html('Testing connection...');
    console.log(dataWithAntiforgeryToken);
    $.ajax({
        url: '/Wizard/CheckConnectionRegistry',
        type: 'POST',
        data: dataWithAntiforgeryToken,
        success: function (data) {
            console.log(data);
            
            if (data === 'success') {
                $('.js-connection-error-registry').addClass('alert-success').html('Connection succesfully established');
                if (submit) {
                    submitRegistry(dataWithAntiforgeryToken);
                }
                if (saveChanges) {
                    saveChangesRegistry(dataWithAntiforgeryToken);
                }

            } else {
                $('.js-connection-error-registry').addClass('alert-danger').html('An error was returned: ' + data);
            }
        }
    });
}

//jQuery time
var current_fs, next_fs, previous_fs; //fieldsets
var left, opacity, scale; //fieldset properties which we will animate
var animating; //flag to prevent quick multi-click glitches

$('.next').click(function () {
    current_fs = $(this).parent();
    next_fs = $(this).parent().next();
    next(current_fs, next_fs);
});

function next(currentFs, nextFs) {
    if (animating) return false;
    animating = true;

    //activate next step on progressbar using the index of next_fs
    $('#progressbar li').eq($('.multiStepForm__step').index(nextFs)).addClass('active');

    //show the next fieldset
    $(nextFs).show();
    //hide the current fieldset with style
    $(currentFs).animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale current_fs down to 80%
            scale = 1 - (1 - now) * 0.2;
            //2. bring next_fs from the right(50%)

            left = ((now * 50) * 10) + 'px';
            //3. increase opacity of next_fs to 1 as it moves in
            opacity = 1 - now;
            $(currentFs).css({
                'transform': 'scale(' + scale + ')',
                'position' : 'absolute'
            });
            $(nextFs).css({ 'transform': 'translateX( ' + left + ' )', 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            $(currentFs).hide();
            animating = false;
            $(currentFs).css('position', 'relative');
            $('#progressbar li').eq($('.multiStepForm__step').index(nextFs)).addClass('noDelay');
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
}

$('.previous').click(function () {
    if (animating) return false;
    animating = true;

    current_fs = $(this).closest('.multiStepForm__step');
    previous_fs = $(current_fs).prev();
    $(previous_fs).css('position', "absolute");

    //de-activate current step on progressbar
    $('.multiStepForm__progressbar-item').eq($('.multiStepForm__step').index(current_fs)).removeClass('active');

    //show the previous fieldset
    previous_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale previous_fs from 80% to 100%
            scale = 0.8 + (1 - now) * 0.2;
            //2. take current_fs to the right(50%) - from 0%
            left = (((1 - now) * 50) * 10) + 'px';
            //3. increase opacity of previous_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'transform': 'translateX( ' + left + ')' });
            previous_fs.css({ 'transform': 'scale(' + scale + ')', 'opacity': opacity });   
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
            $(previous_fs).css('position', 'relative');
            $('.multiStepForm__progressbar-item').eq($('.multiStepForm__step').index(current_fs)).removeClass('noDelay');
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
});

//# sourceMappingURL=data:application/json;charset=utf8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIldpemFyZC5qcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSIsImZpbGUiOiJXaXphcmQuanMiLCJzb3VyY2VzQ29udGVudCI6WyJmdW5jdGlvbiByYW5kb21TdHJpbmcobGVuLCBiZWZvcmVzdHIgPSAnJywgYXJyYXl0b2NoZWNrID0gbnVsbCkge1xyXG4gICAgLy8gQ2hhcnNldCwgZXZlcnkgY2hhcmFjdGVyIGluIHRoaXMgc3RyaW5nIGlzIGFuIG9wdGlvbmFsIG9uZSBpdCBjYW4gdXNlIGFzIGEgcmFuZG9tIGNoYXJhY3Rlci5cclxuICAgIHZhciBjaGFyU2V0ID0gJ0FCQ0RFRkdISUpLTE1OT1BRUlNUVVZXWFlaYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXonO1xyXG4gICAgdmFyIHJhbmRvbVN0cmluZyA9ICcnO1xyXG4gICAgZm9yICh2YXIgaSA9IDA7IGkgPCBsZW47IGkrKykge1xyXG4gICAgICAgIC8vIGNyZWF0ZXMgYSByYW5kb20gbnVtYmVyIGJldHdlZW4gMCBhbmQgdGhlIGNoYXJTZXQgbGVuZ3RoLiBSb3VuZHMgaXQgZG93biB0byBhIHdob2xlIG51bWJlclxyXG4gICAgICAgIHZhciByYW5kb21Qb3ogPSBNYXRoLmZsb29yKE1hdGgucmFuZG9tKCkgKiBjaGFyU2V0Lmxlbmd0aCk7XHJcbiAgICAgICAgcmFuZG9tU3RyaW5nICs9IGNoYXJTZXQuc3Vic3RyaW5nKHJhbmRvbVBveiwgcmFuZG9tUG96ICsgMSk7XHJcbiAgICB9XHJcbiAgICAvLyBJZiBhbiBhcnJheSBpcyBnaXZlbiBpdCB3aWxsIGNoZWNrIHRoZSBhcnJheSwgYW5kIGlmIHRoZSBnZW5lcmF0ZWQgc3RyaW5nIGV4aXN0cyBpbiBpdCBpdCB3aWxsIGNyZWF0ZSBhIG5ldyBvbmUgdW50aWwgYSB1bmlxdWUgb25lIGlzIGZvdW5kICpXQVRDSCBPVVQuIElmIGFsbCBhdmFpbGFibGUgb3B0aW9ucyBhcmUgdXNlZCBpdCB3aWxsIGNhdXNlIGEgbG9vcCBpdCBjYW5ub3QgYnJlYWsgb3V0KlxyXG4gICAgaWYgKGFycmF5dG9jaGVjayA9PSBudWxsKSB7XHJcbiAgICAgICAgcmV0dXJuIGJlZm9yZXN0ciArIHJhbmRvbVN0cmluZztcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgICAgdmFyIGlzSW4gPSAkLmluQXJyYXkoYmVmb3Jlc3RyICsgcmFuZG9tU3RyaW5nLCBhcnJheXRvY2hlY2spOyAvLyBjaGVja3MgaWYgdGhlIHN0cmluZyBpcyBpbiB0aGUgYXJyYXksIHJldHVybnMgYSBwb3NpdGlvblxyXG4gICAgICAgIGlmIChpc0luID4gLTEpIHtcclxuICAgICAgICAgICAgLy8gaWYgdGhlIHBvc2l0aW9uIGlzIG5vdCAtMSAobWVhbmluZywgaXQgaXMgbm90IGluIHRoZSBhcnJheSkgaXQgd2lsbCBzdGFydCBkb2luZyBhIGxvb3BcclxuICAgICAgICAgICAgdmFyIGNvdW50ID0gMDtcclxuICAgICAgICAgICAgZG8ge1xyXG4gICAgICAgICAgICAgICAgcmFuZG9tU3RyaW5nID0gJyc7XHJcbiAgICAgICAgICAgICAgICBmb3IgKHZhciBpID0gMDsgaSA8IGxlbjsgaSsrKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmFyIHJhbmRvbVBveiA9IE1hdGguZmxvb3IoTWF0aC5yYW5kb20oKSAqIGNoYXJTZXQubGVuZ3RoKTtcclxuICAgICAgICAgICAgICAgICAgICByYW5kb21TdHJpbmcgKz0gY2hhclNldC5zdWJzdHJpbmcocmFuZG9tUG96LCByYW5kb21Qb3ogKyAxKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIGlzSW4gPSAkLmluQXJyYXkoYmVmb3Jlc3RyICsgcmFuZG9tU3RyaW5nLCBhcnJheXRvY2hlY2spO1xyXG4gICAgICAgICAgICAgICAgY291bnQrKztcclxuICAgICAgICAgICAgfSB3aGlsZSAoaXNJbiA+IC0xKTtcclxuICAgICAgICAgICAgcmV0dXJuIGJlZm9yZXN0ciArIHJhbmRvbVN0cmluZztcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICByZXR1cm4gYmVmb3Jlc3RyICsgcmFuZG9tU3RyaW5nO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxufVxyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtZ2VuZXJhdGUtYXV0aCcsIGZ1bmN0aW9uICgpIHtcclxuICAgIHZhciBuZXdBdXRoID0gcmFuZG9tU3RyaW5nKDMwKTtcclxuICAgICQoJy5qcy1hdXRoLXRva2VuJykudmFsKG5ld0F1dGgpO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLWNvcHktY29kZScsIGZ1bmN0aW9uICgpIHtcclxuLy8gICAgdmFyIGNvcHlUZXh0ID0gZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoJ0F1dGhUb2tlbicpO1xyXG4vL1xyXG4vLyAgICAvKiBTZWxlY3QgdGhlIHRleHQgZmllbGQgKi9cclxuLy8gICAgY29weVRleHQuc2VsZWN0KCk7XHJcbi8vXHJcbi8vICAgIC8qIENvcHkgdGhlIHRleHQgaW5zaWRlIHRoZSB0ZXh0IGZpZWxkICovXHJcbi8vICAgIGRvY3VtZW50LmV4ZWNDb21tYW5kKCdjb3B5Jyk7XHJcblxyXG4gICAgdmFyIGF1dGggPSAkKCcjQXV0aFRva2VuJykudmFsKCk7XHJcbiAgICB2YXIgc3RyID0gJ2FwaT15ZXNcXG5hcGkta2V5PScgKyBhdXRoICsgJ1xcbndlYnNlcnZlci1hZGRyZXNzPTx5b3VyIFBvd2VyRE5TIHNlcnZlciBJUD5cXG53ZWJzZXJ2ZXItcG9ydD04MDgxXFxud2Vic2VydmVyLWFsbG93LWZyb209PHlvdXIgYmFja2VuZCBJUCwgc2VwYXJhdGUgd2l0aCBjb21tYSBpZiB5b3UgaGF2ZSBtb3JlIElQ4oCZcyB0byBhbGxvdz4nO1xyXG4gICAgY29uc29sZS5sb2coc3RyKTtcclxuICAgIGNvcHlTdHJpbmdUb0NsaXBib2FyZChzdHIpO1xyXG59KTtcclxuXHJcbmZ1bmN0aW9uIGNvcHlTdHJpbmdUb0NsaXBib2FyZChzdHIpIHtcclxuICAgIC8vIENyZWF0ZSBuZXcgZWxlbWVudFxyXG4gICAgdmFyIGVsID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgndGV4dGFyZWEnKTtcclxuICAgIC8vIFNldCB2YWx1ZSAoc3RyaW5nIHRvIGJlIGNvcGllZClcclxuICAgIGVsLnZhbHVlID0gc3RyO1xyXG4gICAgLy8gU2V0IG5vbi1lZGl0YWJsZSB0byBhdm9pZCBmb2N1cyBhbmQgbW92ZSBvdXRzaWRlIG9mIHZpZXdcclxuICAgIGVsLnNldEF0dHJpYnV0ZSgncmVhZG9ubHknLCAnJyk7XHJcbiAgICBlbC5zdHlsZSA9IHsgcG9zaXRpb246ICdhYnNvbHV0ZScsIGxlZnQ6ICctOTk5OXB4JyB9O1xyXG4gICAgZG9jdW1lbnQuYm9keS5hcHBlbmRDaGlsZChlbCk7XHJcbiAgICAvLyBTZWxlY3QgdGV4dCBpbnNpZGUgZWxlbWVudFxyXG4gICAgZWwuc2VsZWN0KCk7XHJcbiAgICAvLyBDb3B5IHRleHQgdG8gY2xpcGJvYXJkXHJcbiAgICBkb2N1bWVudC5leGVjQ29tbWFuZCgnY29weScpO1xyXG4gICAgLy8gUmVtb3ZlIHRlbXBvcmFyeSBlbGVtZW50XHJcbiAgICBkb2N1bWVudC5ib2R5LnJlbW92ZUNoaWxkKGVsKTtcclxufVxuJChkb2N1bWVudCkucmVhZHkoZnVuY3Rpb24gKCkge1xyXG4gICAgJCgnLmpzLWNvbm5lY3Rpb24tZXJyb3InKS5odG1sKCdDaGVja2luZyBmb3IgYWxyZWFkeSBmaWxsZWQgaW4gZGV0YWlscy4uLicpO1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvV2l6YXJkL0dldERuc1NlcnZlcicsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSAhPT0gJ0ZhaWxlZCcpIHtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1jb25uZWN0aW9uLWVycm9yJykuYWRkQ2xhc3MoJ2FsZXJ0LXN1Y2Nlc3MnKS5odG1sKCdEZXRhaWxzIGZvdW5kIScpO1xyXG4gICAgICAgICAgICAgICAgdmFyIHN0dWZmID0gSlNPTi5wYXJzZShkYXRhKTtcclxuICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKHN0dWZmKTtcclxuICAgICAgICAgICAgICAgICQoJyNOYW1lJykudmFsKHN0dWZmLk5hbWUpO1xyXG4gICAgICAgICAgICAgICAgJCgnI1VybCcpLnZhbChzdHVmZi5CYXNlVXJsKTtcclxuICAgICAgICAgICAgICAgICQoJyNBdXRoVG9rZW4nKS52YWwoc3R1ZmYuQXV0aFRva2VuKTtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1zdWJtaXQtZG5zJykuaHRtbCgnU2F2ZSBDaGFuZ2VzICYgTmV4dCcpLnJlbW92ZUNsYXNzKCdqcy1zdWJtaXQtZG5zJykuYWRkQ2xhc3MoJ2pzLXNhdmUtY2hhbmdlcy1kbnMnKTtcclxuXHJcbiAgICAgICAgICAgICAgICBzZXRUaW1lb3V0KGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvcicpLnJlbW92ZUNsYXNzKCdhbGVydC1zdWNjZXNzJykuaHRtbCgnQ29ubmVjdGlvbiBoYXMgbm90IGJlZW4gdGVzdGVkIHlldCcpO1xyXG4gICAgICAgICAgICAgICAgfSwgODAwKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG5cclxufSk7XHJcblxyXG4kKCcuc3VibWl0JykuY2xpY2soZnVuY3Rpb24gKCkge1xyXG4gICAgcmV0dXJuIGZhbHNlO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXRlc3QtY29ubmVjdGlvbicsIGZ1bmN0aW9uICgpIHtcclxuICAgIHRlc3RDb25uZWN0aW9uKCk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtc3VibWl0LWRucycsIGZ1bmN0aW9uICgpIHtcclxuICAgIHRlc3RDb25uZWN0aW9uKHRydWUpO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXNhdmUtY2hhbmdlcy1kbnMnLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB0ZXN0Q29ubmVjdGlvbihmYWxzZSwgdHJ1ZSk7XHJcbn0pO1xyXG5cclxuZnVuY3Rpb24gdGVzdENvbm5lY3Rpb24oc3VibWl0ID0gZmFsc2UsIHNhdmVDaGFuZ2VzRSA9IGZhbHNlKSB7XHJcbiAgICB2YXIgbmFtZSA9ICQoJyNOYW1lJykudmFsKCk7XHJcbiAgICB2YXIgdXJsID0gJCgnI1VybCcpLnZhbCgpO1xyXG4gICAgdmFyIGFwaUtleSA9ICQoJyNBdXRoVG9rZW4nKS52YWwoKTtcclxuICAgIHZhciBteURhdGEgPSB7IHVybDogdXJsLCBhcGlLZXk6IGFwaUtleSB9XHJcbiAgICB2YXIgdG9rZW4gPSAkKCdpbnB1dFtuYW1lPVwiX19SZXF1ZXN0VmVyaWZpY2F0aW9uVG9rZW5cIl0nLCAkKCcjbXNmb3JtJykpLnZhbCgpO1xyXG4gICAgdmFyIGRhdGFXaXRoQW50aWZvcmdlcnlUb2tlbiA9ICQuZXh0ZW5kKG15RGF0YSwgeyAnX19SZXF1ZXN0VmVyaWZpY2F0aW9uVG9rZW4nOiB0b2tlbiB9KTtcclxuICAgIHZhciBteURhdGFTdWJtaXQgPSB7IG5hbWU6IG5hbWUsIHVybDogdXJsLCBhcGlLZXk6IGFwaUtleSB9XHJcbiAgICB2YXIgZGF0YVN1Ym1pdCA9ICQuZXh0ZW5kKG15RGF0YVN1Ym1pdCwgeyAnX19SZXF1ZXN0VmVyaWZpY2F0aW9uVG9rZW4nOiB0b2tlbiB9KTtcclxuICAgIGNvbnNvbGUubG9nKG5hbWUpXHJcbiAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvcicpLnJlbW92ZUNsYXNzKCdhbGVydC1kYW5nZXInKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpLmh0bWwoJ1Rlc3RpbmcgY29ubmVjdGlvbi4uLicpO1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvV2l6YXJkL0NoZWNrQ29ubmVjdGlvbicsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IGRhdGFXaXRoQW50aWZvcmdlcnlUb2tlbixcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLWNvbm5lY3Rpb24tZXJyb3InKS5hZGRDbGFzcygnYWxlcnQtc3VjY2VzcycpLmh0bWwoJ0Nvbm5lY3Rpb24gc3VjY2VzZnVsbHkgZXN0YWJsaXNoZWQnKTtcclxuICAgICAgICAgICAgICAgIGlmIChzdWJtaXQpIHtcclxuICAgICAgICAgICAgICAgICAgICBzdWJtaXREbnMoZGF0YVN1Ym1pdCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBpZiAoc2F2ZUNoYW5nZXNFKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgc2F2ZUNoYW5nZXMoZGF0YVN1Ym1pdCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvcicpLmFkZENsYXNzKCdhbGVydC1kYW5nZXInKS5odG1sKCdBbiBlcnJvciB3YXMgcmV0dXJuZWQ6ICcgKyBkYXRhKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59XHJcblxyXG5mdW5jdGlvbiBzYXZlQ2hhbmdlcyhmb3JtRGF0YSkge1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvV2l6YXJkL1NhdmVDaGFuZ2VzRG5zJyxcclxuICAgICAgICB0eXBlOiAnUE9TVCcsXHJcbiAgICAgICAgZGF0YTogZm9ybURhdGEsXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgIGlmIChkYXRhID09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLXN1Ym1pdC1kbnMnKS5odG1sKCdTYXZlIENoYW5nZXMgJiBOZXh0JykucmVtb3ZlQ2xhc3MoJ2pzLXN1Ym1pdC1kbnMnKS5hZGRDbGFzcygnanMtc2F2ZS1jaGFuZ2VzLWRucycpO1xyXG4gICAgICAgICAgICAgICAgdmFyIHRoaXNGcyA9ICQoJy5kbnNGcycpWzBdO1xyXG4gICAgICAgICAgICAgICAgdmFyIG5leHRGcyA9ICQoJy5uYW1lc2VydmVyRnMnKVswXTtcclxuICAgICAgICAgICAgICAgIG5leHQodGhpc0ZzLCBuZXh0RnMpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIHN1Ym1pdERucyhmb3JtRGF0YSkge1xyXG4gICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6ICcvV2l6YXJkL0NyZWF0ZURucycsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IGZvcm1EYXRhLFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PSAnc3VjY2VzcycpIHtcclxuICAgICAgICAgICAgICAgICQoJy5qcy1zdWJtaXQtZG5zJykuaHRtbCgnU2F2ZSBDaGFuZ2VzICYgTmV4dCcpLnJlbW92ZUNsYXNzKCdqcy1zdWJtaXQtZG5zJykuYWRkQ2xhc3MoJ2pzLXNhdmUtY2hhbmdlcy1kbnMnKTtcclxuICAgICAgICAgICAgICAgIHZhciB0aGlzRnMgPSAkKCcuZG5zRnMnKVswXTtcclxuICAgICAgICAgICAgICAgIHZhciBuZXh0RnMgPSAkKCcubmFtZXNlcnZlckZzJylbMF07XHJcbiAgICAgICAgICAgICAgICBuZXh0KHRoaXNGcywgbmV4dEZzKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59XG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1wbHVzLW5hbWVzZXJ2ZXInLCBmdW5jdGlvbiAoKSB7XHJcbiAgICAkKCcuanMtbmFtZXNlcnZlckdyb3VwVG9jb3B5JykuY2xvbmUoKS5hcHBlbmRUbygnLmpzLW5hbWVzZXJ2ZXJHcm91cC1jb3B5dG8nKS5jc3MoJ2Rpc3BsYXknLCAnYmxvY2snKTtcclxuICAgIHJlc2V0R3JvdXBOdW1iZXJzKCk7XHJcbn0pO1xyXG5cclxuJCgnYm9keScpLm9uKCdjbGljaycsICcuanMtc3VibWl0LW5hbWVzZXJ2ZXInLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB2YXIgbnVtID0gJCgnLmpzLW5hbWVzZXJ2ZXJHcm91cCcpLmxlbmd0aDtcclxuICAgIHZhciBjb3VudCA9IDA7XHJcbiAgICAkKCcuanMtbmFtZXNlcnZlckdyb3VwJykuZWFjaChmdW5jdGlvbiAoaW5kZXgpIHtcclxuICAgICAgICB2YXIgaWQgPSAkKHRoaXMpLmRhdGEoJ2lkJyk7XHJcbiAgICAgICAgXHJcbiAgICAgICAgdmFyIG5ld09iaiA9IHtcclxuICAgICAgICAgICAgbmFtZTogJCh0aGlzKS5maW5kKCcuanMtbmFtZXNlcnZlckdyb3VwTmFtZScpLnZhbCgpLFxyXG4gICAgICAgICAgICBuczAxOiAkKHRoaXMpLmZpbmQoJy5qcy1uYW1lU2VydmVyR3JvdXBTZXJ2ZXIxJykudmFsKCksXHJcbiAgICAgICAgICAgIG5zMDI6ICQodGhpcykuZmluZCgnLmpzLW5hbWVTZXJ2ZXJHcm91cFNlcnZlcjInKS52YWwoKSxcclxuICAgICAgICAgICAgbnMwMzogJCh0aGlzKS5maW5kKCcuanMtbmFtZVNlcnZlckdyb3VwU2VydmVyMycpLnZhbCgpLFxyXG4gICAgICAgICAgICBuczA0OiAkKHRoaXMpLmZpbmQoJy5qcy1uYW1lU2VydmVyR3JvdXBTZXJ2ZXI0JykudmFsKCksXHJcbiAgICAgICAgICAgIG5zMDU6ICQodGhpcykuZmluZCgnLmpzLW5hbWVTZXJ2ZXJHcm91cFNlcnZlcjUnKS52YWwoKSxcclxuICAgICAgICAgICAgbnMwNjogJCh0aGlzKS5maW5kKCcuanMtbmFtZVNlcnZlckdyb3VwU2VydmVyNicpLnZhbCgpLFxyXG4gICAgICAgICAgICBuczA3OiAkKHRoaXMpLmZpbmQoJy5qcy1uYW1lU2VydmVyR3JvdXBTZXJ2ZXI3JykudmFsKCksXHJcbiAgICAgICAgICAgIG5zMDg6ICQodGhpcykuZmluZCgnLmpzLW5hbWVTZXJ2ZXJHcm91cFNlcnZlcjgnKS52YWwoKSxcclxuICAgICAgICAgICAgbnMwOTogJCh0aGlzKS5maW5kKCcuanMtbmFtZVNlcnZlckdyb3VwU2VydmVyOScpLnZhbCgpLFxyXG4gICAgICAgICAgICBuczEwOiAkKHRoaXMpLmZpbmQoJy5qcy1uYW1lU2VydmVyR3JvdXBTZXJ2ZXIxMCcpLnZhbCgpLFxyXG4gICAgICAgICAgICBpZDogaWRcclxuICAgICAgICB9O1xyXG5cclxuICAgICAgICB2YXIgdG9rZW4gPSAkKCdpbnB1dFtuYW1lPVwiX19SZXF1ZXN0VmVyaWZpY2F0aW9uVG9rZW5cIl0nLCAkKCcjbXNmb3JtJykpLnZhbCgpO1xyXG4gICAgICAgIHZhciBkYXRhV2l0aEFudGlmb3JnZXJ5VG9rZW4gPSAkLmV4dGVuZChuZXdPYmosIHsgJ19fUmVxdWVzdFZlcmlmaWNhdGlvblRva2VuJzogdG9rZW4gfSk7XHJcbiAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgdXJsOiAnL1dpemFyZC9DcmVhdGVOYW1lc2VydmVycycsXHJcbiAgICAgICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICAgICAgZGF0YTogZGF0YVdpdGhBbnRpZm9yZ2VyeVRva2VuLFxyXG4gICAgICAgICAgICBkb25lOiBmdW5jdGlvbihkYXRhKSB7XHJcbiAgICAgICAgICAgICAgICBcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICAgICAgY291bnQrKztcclxuICAgICAgICAgICAgICAgIGlmIChjb3VudCA9PT0gbnVtKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmFyIHRoaXNGcyA9ICQoJy5uYW1lc2VydmVyRnMnKVswXTtcclxuICAgICAgICAgICAgICAgICAgICB2YXIgbmV4dEZzID0gJCgnLnJlZ2lzdHJ5RnMnKVswXTtcclxuICAgICAgICAgICAgICAgICAgICBuZXh0KHRoaXNGcywgbmV4dEZzKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgc3RhdHVzQ29kZToge1xyXG4gICAgICAgICAgICAgICAgNTAwOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgYWxlcnQoXCJTY3JpcHQgZXhoYXVzdGVkXCIpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSk7XHJcbiAgICB9KTtcclxufSk7XHJcblxyXG5mdW5jdGlvbiByZXNldEdyb3VwTnVtYmVycygpIHtcclxuICAgICQoJy5uYW1lc2VydmVyR3JvdXBfX3RpdGxlJykuZWFjaChmdW5jdGlvbiAoaW5kZXgpIHtcclxuICAgICAgICAkKHRoaXMpLmh0bWwoJ05hbWVzZXJ2ZXIgZ3JvdXAgJyArIChpbmRleCArIDEpKTtcclxuICAgIH0pO1xyXG59XHJcblxyXG4kKGRvY3VtZW50KS5yZWFkeShmdW5jdGlvbiAoKSB7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9XaXphcmQvR2V0TmFtZXNlcnZlcnMnLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2cocmVzcG9uc2UpO1xyXG4gICAgICAgICAgICBpZiAocmVzcG9uc2UgIT09ICdmYWlsZWQnKSB7XHJcbiAgICAgICAgICAgICAgICB2YXIgZGF0YSA9IEpTT04ucGFyc2UocmVzcG9uc2UpO1xyXG4gICAgICAgICAgICAgICAgZGF0YS5tYXAoZnVuY3Rpb24gKGVsZW0sIGluZGV4KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmFyIGNvcGllZEVsZW0gPSAkKCcuanMtbmFtZXNlcnZlckdyb3VwVG9jb3B5JykuY2xvbmUoKS5hcHBlbmRUbygnLmpzLW5hbWVzZXJ2ZXJHcm91cC1jb3B5dG8nKS5yZW1vdmVDbGFzcygnanMtbmFtZXNlcnZlckdyb3VwVG9jb3B5JykucmVtb3ZlQ2xhc3MoJ2Rpc25vbmUnKS5hZGRDbGFzcygnanMtbmFtZXNlcnZlckdyb3VwJylbMF07XHJcbiAgICAgICAgICAgICAgICAgICAgJChjb3BpZWRFbGVtKS5kYXRhKCdpZCcsIGVsZW0uSWQpLmF0dHIoJ2RhdGEtaWQnLCBlbGVtLklkKTtcclxuICAgICAgICAgICAgICAgICAgICAkKGNvcGllZEVsZW0pLmZpbmQoJy5qcy1uYW1lc2VydmVyR3JvdXBOYW1lJykudmFsKGVsZW0uTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChjb3BpZWRFbGVtKS5maW5kKCcuanMtbmFtZVNlcnZlckdyb3VwU2VydmVyMScpLnZhbChlbGVtLk5zMDEpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoY29waWVkRWxlbSkuZmluZCgnLmpzLW5hbWVTZXJ2ZXJHcm91cFNlcnZlcjInKS52YWwoZWxlbS5OczAyKTtcclxuICAgICAgICAgICAgICAgICAgICAkKGNvcGllZEVsZW0pLmZpbmQoJy5qcy1uYW1lU2VydmVyR3JvdXBTZXJ2ZXIzJykudmFsKGVsZW0uTnMwMyk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChjb3BpZWRFbGVtKS5maW5kKCcuanMtbmFtZVNlcnZlckdyb3VwU2VydmVyNCcpLnZhbChlbGVtLk5zMDQpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoY29waWVkRWxlbSkuZmluZCgnLmpzLW5hbWVTZXJ2ZXJHcm91cFNlcnZlcjUnKS52YWwoZWxlbS5OczA1KTtcclxuICAgICAgICAgICAgICAgICAgICAkKGNvcGllZEVsZW0pLmZpbmQoJy5qcy1uYW1lU2VydmVyR3JvdXBTZXJ2ZXI2JykudmFsKGVsZW0uTnMwNik7XHJcbiAgICAgICAgICAgICAgICAgICAgJChjb3BpZWRFbGVtKS5maW5kKCcuanMtbmFtZVNlcnZlckdyb3VwU2VydmVyNycpLnZhbChlbGVtLk5zMDcpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoY29waWVkRWxlbSkuZmluZCgnLmpzLW5hbWVTZXJ2ZXJHcm91cFNlcnZlcjgnKS52YWwoZWxlbS5OczA4KTtcclxuICAgICAgICAgICAgICAgICAgICAkKGNvcGllZEVsZW0pLmZpbmQoJy5qcy1uYW1lU2VydmVyR3JvdXBTZXJ2ZXI5JykudmFsKGVsZW0uTnMwOSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChjb3BpZWRFbGVtKS5maW5kKCcuanMtbmFtZVNlcnZlckdyb3VwU2VydmVyMTAnKS52YWwoZWxlbS5OczEwKTtcclxuICAgICAgICAgICAgICAgICAgICByZXNldEdyb3VwTnVtYmVycygpO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtbmFtZXNlcnZlckdyb3VwOmZpcnN0LWNoaWxkKCknKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG5cclxufSk7XG4kKGRvY3VtZW50KS5yZWFkeShmdW5jdGlvbiAoKSB7XHJcbiAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeScpLmh0bWwoJ0NoZWNraW5nIGZvciBhbHJlYWR5IGZpbGxlZCBpbiBkZXRhaWxzLi4uJyk7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9XaXphcmQvR2V0UmVnaXN0cnknLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBzdWNjZXNzOiBmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICAgICAgaWYgKGRhdGEgIT09ICdGYWlsZWQnKSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeScpLmFkZENsYXNzKCdhbGVydC1zdWNjZXNzJykuaHRtbCgnRGV0YWlscyBmb3VuZCEnKTtcclxuICAgICAgICAgICAgICAgIHZhciBzdHVmZiA9IEpTT04ucGFyc2UoZGF0YSk7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhzdHVmZik7XHJcbiAgICAgICAgICAgICAgICAkKCcjTmFtZVJlZ2lzdHJ5JykudmFsKHN0dWZmLk5hbWUpO1xyXG4gICAgICAgICAgICAgICAgJCgnI1VybFJlZ2lzdHJ5JykudmFsKHN0dWZmLlVybCk7XHJcbiAgICAgICAgICAgICAgICAkKCcjUG9ydFJlZ2lzdHJ5JykudmFsKHN0dWZmLlBvcnQpO1xyXG4gICAgICAgICAgICAgICAgJCgnI1VzZXJOYW1lUmVnaXN0cnknKS52YWwoc3R1ZmYuVXNlcm5hbWUpO1xyXG4gICAgICAgICAgICAgICAgJCgnI1Bhc3N3b3JkUmVnaXN0cnknKS52YWwoc3R1ZmYuUGFzc3dvcmQpO1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLXN1Ym1pdC1yZWdpc3RyeScpLmh0bWwoJ1NhdmUgQ2hhbmdlcyAmIE5leHQnKS5yZW1vdmVDbGFzcygnanMtc3VibWl0LXJlZ2lzdHJ5JykuYWRkQ2xhc3MoJ2pzLXNhdmUtY2hhbmdlcy1yZWdpc3RyeScpO1xyXG5cclxuICAgICAgICAgICAgICAgIHNldFRpbWVvdXQoZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgICAgICQoJy5qcy1jb25uZWN0aW9uLWVycm9yLXJlZ2lzdHJ5JykucmVtb3ZlQ2xhc3MoJ2FsZXJ0LXN1Y2Nlc3MnKS5odG1sKCdDb25uZWN0aW9uIGhhcyBub3QgYmVlbiB0ZXN0ZWQgeWV0Jyk7XHJcbiAgICAgICAgICAgICAgICB9LCA4MDApO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcblxyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXRlc3QtY29ubmVjdGlvbi1yZWdpc3RyeScsIGZ1bmN0aW9uICgpIHtcclxuICAgIHRlc3RDb25uZWN0aW9uUmVnaXN0cnkoKTtcclxufSk7XHJcblxyXG4kKCdib2R5Jykub24oJ2NsaWNrJywgJy5qcy1zdWJtaXQtcmVnaXN0cnknLCBmdW5jdGlvbiAoKSB7XHJcbiAgICB0ZXN0Q29ubmVjdGlvblJlZ2lzdHJ5KDEpO1xyXG59KTtcclxuXHJcbiQoJ2JvZHknKS5vbignY2xpY2snLCAnLmpzLXNhdmUtY2hhbmdlcy1yZWdpc3RyeScsIGZ1bmN0aW9uICgpIHtcclxuICAgIHRlc3RDb25uZWN0aW9uUmVnaXN0cnkoMCwgMSk7XHJcbn0pO1xyXG5cclxuXHJcbmZ1bmN0aW9uIHN1Ym1pdFJlZ2lzdHJ5KGRhdGEpIHtcclxuICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiAnL1dpemFyZC9DcmVhdGVSZWdpc3RyeScsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgaWYgKGRhdGEgPT09ICdzdWNjZXNzJykge1xyXG4gICAgICAgICAgICAgICAgdmFyIHRoaXNGcyA9ICQoJy5yZWdpc3RyeUZzJylbMF07XHJcbiAgICAgICAgICAgICAgICB2YXIgbmV4dEZzID0gJCgnLnRsZEZzJylbMF07XHJcbiAgICAgICAgICAgICAgICBuZXh0KHRoaXNGcywgbmV4dEZzKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG59XHJcblxyXG5mdW5jdGlvbiBzYXZlQ2hhbmdlc1JlZ2lzdHJ5KGZvcm1EYXRhKSB7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9XaXphcmQvU2F2ZUNoYW5nZXNSZWdpc3RyeScsXHJcbiAgICAgICAgdHlwZTogJ1BPU1QnLFxyXG4gICAgICAgIGRhdGE6IGZvcm1EYXRhLFxyXG4gICAgICAgIHN1Y2Nlc3M6IGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRhdGEpO1xyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICB2YXIgdGhpc0ZzID0gJCgnLnJlZ2lzdHJ5RnMnKVswXTtcclxuICAgICAgICAgICAgICAgIHZhciBuZXh0RnMgPSAkKCcudGxkRnMnKVswXTtcclxuICAgICAgICAgICAgICAgIG5leHQodGhpc0ZzLCBuZXh0RnMpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIHRlc3RDb25uZWN0aW9uUmVnaXN0cnkoc3VibWl0ID0gZmFsc2UsIHNhdmVDaGFuZ2VzID0gZmFsc2UpIHtcclxuICAgIHZhciBuYW1lID0gJCgnI05hbWVSZWdpc3RyeScpLnZhbCgpO1xyXG4gICAgdmFyIHVybCA9ICQoJyNVcmxSZWdpc3RyeScpLnZhbCgpO1xyXG4gICAgdmFyIHBvcnQgPSAkKCcjUG9ydFJlZ2lzdHJ5JykudmFsKCk7XHJcbiAgICB2YXIgdXNlcm5hbWUgPSAkKCcjVXNlck5hbWVSZWdpc3RyeScpLnZhbCgpO1xyXG4gICAgdmFyIHBhc3N3b3JkID0gJCgnI1Bhc3N3b3JkUmVnaXN0cnknKS52YWwoKTtcclxuICAgIHZhciBteURhdGEgPSB7XHJcbiAgICAgICAgbmFtZTogbmFtZSxcclxuICAgICAgICB1cmw6IHVybCxcclxuICAgICAgICBwb3J0OiBwb3J0LFxyXG4gICAgICAgIHVzZXJuYW1lOiB1c2VybmFtZSxcclxuICAgICAgICBwYXNzd29yZDogcGFzc3dvcmRcclxuICAgIH1cclxuICAgIHZhciB0b2tlbiA9ICQoJ2lucHV0W25hbWU9XCJfX1JlcXVlc3RWZXJpZmljYXRpb25Ub2tlblwiXScsICQoJyNtc2Zvcm0nKSkudmFsKCk7XHJcbiAgICB2YXIgZGF0YVdpdGhBbnRpZm9yZ2VyeVRva2VuID0gJC5leHRlbmQobXlEYXRhLCB7ICdfX1JlcXVlc3RWZXJpZmljYXRpb25Ub2tlbic6IHRva2VuIH0pO1xyXG4gICAgJCgnLmpzLWNvbm5lY3Rpb24tZXJyb3ItcmVnaXN0cnknKS5yZW1vdmVDbGFzcygnYWxlcnQtc3VjY2VzcycpLnJlbW92ZUNsYXNzKCdhbGVydC1kYW5nZXInKVxyXG4gICAgICAgIC5odG1sKCdUZXN0aW5nIGNvbm5lY3Rpb24uLi4nKTtcclxuICAgIGNvbnNvbGUubG9nKGRhdGFXaXRoQW50aWZvcmdlcnlUb2tlbik7XHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogJy9XaXphcmQvQ2hlY2tDb25uZWN0aW9uUmVnaXN0cnknLFxyXG4gICAgICAgIHR5cGU6ICdQT1NUJyxcclxuICAgICAgICBkYXRhOiBkYXRhV2l0aEFudGlmb3JnZXJ5VG9rZW4sXHJcbiAgICAgICAgc3VjY2VzczogZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICBpZiAoZGF0YSA9PT0gJ3N1Y2Nlc3MnKSB7XHJcbiAgICAgICAgICAgICAgICAkKCcuanMtY29ubmVjdGlvbi1lcnJvci1yZWdpc3RyeScpLmFkZENsYXNzKCdhbGVydC1zdWNjZXNzJykuaHRtbCgnQ29ubmVjdGlvbiBzdWNjZXNmdWxseSBlc3RhYmxpc2hlZCcpO1xyXG4gICAgICAgICAgICAgICAgaWYgKHN1Ym1pdCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHN1Ym1pdFJlZ2lzdHJ5KGRhdGFXaXRoQW50aWZvcmdlcnlUb2tlbik7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBpZiAoc2F2ZUNoYW5nZXMpIHtcclxuICAgICAgICAgICAgICAgICAgICBzYXZlQ2hhbmdlc1JlZ2lzdHJ5KGRhdGFXaXRoQW50aWZvcmdlcnlUb2tlbik7XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJCgnLmpzLWNvbm5lY3Rpb24tZXJyb3ItcmVnaXN0cnknKS5hZGRDbGFzcygnYWxlcnQtZGFuZ2VyJykuaHRtbCgnQW4gZXJyb3Igd2FzIHJldHVybmVkOiAnICsgZGF0YSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9KTtcclxufVxuXG4vL2pRdWVyeSB0aW1lXHJcbnZhciBjdXJyZW50X2ZzLCBuZXh0X2ZzLCBwcmV2aW91c19mczsgLy9maWVsZHNldHNcclxudmFyIGxlZnQsIG9wYWNpdHksIHNjYWxlOyAvL2ZpZWxkc2V0IHByb3BlcnRpZXMgd2hpY2ggd2Ugd2lsbCBhbmltYXRlXHJcbnZhciBhbmltYXRpbmc7IC8vZmxhZyB0byBwcmV2ZW50IHF1aWNrIG11bHRpLWNsaWNrIGdsaXRjaGVzXHJcblxyXG4kKCcubmV4dCcpLmNsaWNrKGZ1bmN0aW9uICgpIHtcclxuICAgIGN1cnJlbnRfZnMgPSAkKHRoaXMpLnBhcmVudCgpO1xyXG4gICAgbmV4dF9mcyA9ICQodGhpcykucGFyZW50KCkubmV4dCgpO1xyXG4gICAgbmV4dChjdXJyZW50X2ZzLCBuZXh0X2ZzKTtcclxufSk7XHJcblxyXG5mdW5jdGlvbiBuZXh0KGN1cnJlbnRGcywgbmV4dEZzKSB7XHJcbiAgICBpZiAoYW5pbWF0aW5nKSByZXR1cm4gZmFsc2U7XHJcbiAgICBhbmltYXRpbmcgPSB0cnVlO1xyXG5cclxuICAgIC8vYWN0aXZhdGUgbmV4dCBzdGVwIG9uIHByb2dyZXNzYmFyIHVzaW5nIHRoZSBpbmRleCBvZiBuZXh0X2ZzXHJcbiAgICAkKCcjcHJvZ3Jlc3NiYXIgbGknKS5lcSgkKCcubXVsdGlTdGVwRm9ybV9fc3RlcCcpLmluZGV4KG5leHRGcykpLmFkZENsYXNzKCdhY3RpdmUnKTtcclxuXHJcbiAgICAvL3Nob3cgdGhlIG5leHQgZmllbGRzZXRcclxuICAgICQobmV4dEZzKS5zaG93KCk7XHJcbiAgICAvL2hpZGUgdGhlIGN1cnJlbnQgZmllbGRzZXQgd2l0aCBzdHlsZVxyXG4gICAgJChjdXJyZW50RnMpLmFuaW1hdGUoeyBvcGFjaXR5OiAwIH0sIHtcclxuICAgICAgICBzdGVwOiBmdW5jdGlvbiAobm93LCBteCkge1xyXG4gICAgICAgICAgICAvL2FzIHRoZSBvcGFjaXR5IG9mIGN1cnJlbnRfZnMgcmVkdWNlcyB0byAwIC0gc3RvcmVkIGluIFwibm93XCJcclxuICAgICAgICAgICAgLy8xLiBzY2FsZSBjdXJyZW50X2ZzIGRvd24gdG8gODAlXHJcbiAgICAgICAgICAgIHNjYWxlID0gMSAtICgxIC0gbm93KSAqIDAuMjtcclxuICAgICAgICAgICAgLy8yLiBicmluZyBuZXh0X2ZzIGZyb20gdGhlIHJpZ2h0KDUwJSlcclxuXHJcbiAgICAgICAgICAgIGxlZnQgPSAoKG5vdyAqIDUwKSAqIDEwKSArICdweCc7XHJcbiAgICAgICAgICAgIC8vMy4gaW5jcmVhc2Ugb3BhY2l0eSBvZiBuZXh0X2ZzIHRvIDEgYXMgaXQgbW92ZXMgaW5cclxuICAgICAgICAgICAgb3BhY2l0eSA9IDEgLSBub3c7XHJcbiAgICAgICAgICAgICQoY3VycmVudEZzKS5jc3Moe1xyXG4gICAgICAgICAgICAgICAgJ3RyYW5zZm9ybSc6ICdzY2FsZSgnICsgc2NhbGUgKyAnKScsXHJcbiAgICAgICAgICAgICAgICAncG9zaXRpb24nIDogJ2Fic29sdXRlJ1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgJChuZXh0RnMpLmNzcyh7ICd0cmFuc2Zvcm0nOiAndHJhbnNsYXRlWCggJyArIGxlZnQgKyAnICknLCAnb3BhY2l0eSc6IG9wYWNpdHkgfSk7XHJcbiAgICAgICAgfSxcclxuICAgICAgICBkdXJhdGlvbjogODAwLFxyXG4gICAgICAgIGNvbXBsZXRlOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICQoY3VycmVudEZzKS5oaWRlKCk7XHJcbiAgICAgICAgICAgIGFuaW1hdGluZyA9IGZhbHNlO1xyXG4gICAgICAgICAgICAkKGN1cnJlbnRGcykuY3NzKCdwb3NpdGlvbicsICdyZWxhdGl2ZScpO1xyXG4gICAgICAgICAgICAkKCcjcHJvZ3Jlc3NiYXIgbGknKS5lcSgkKCcubXVsdGlTdGVwRm9ybV9fc3RlcCcpLmluZGV4KG5leHRGcykpLmFkZENsYXNzKCdub0RlbGF5Jyk7XHJcbiAgICAgICAgfSxcclxuICAgICAgICAvL3RoaXMgY29tZXMgZnJvbSB0aGUgY3VzdG9tIGVhc2luZyBwbHVnaW5cclxuICAgICAgICBlYXNpbmc6ICdlYXNlSW5PdXRCYWNrJ1xyXG4gICAgfSk7XHJcbn1cclxuXHJcbiQoJy5wcmV2aW91cycpLmNsaWNrKGZ1bmN0aW9uICgpIHtcclxuICAgIGlmIChhbmltYXRpbmcpIHJldHVybiBmYWxzZTtcclxuICAgIGFuaW1hdGluZyA9IHRydWU7XHJcblxyXG4gICAgY3VycmVudF9mcyA9ICQodGhpcykuY2xvc2VzdCgnLm11bHRpU3RlcEZvcm1fX3N0ZXAnKTtcclxuICAgIHByZXZpb3VzX2ZzID0gJChjdXJyZW50X2ZzKS5wcmV2KCk7XHJcbiAgICAkKHByZXZpb3VzX2ZzKS5jc3MoJ3Bvc2l0aW9uJywgXCJhYnNvbHV0ZVwiKTtcclxuXHJcbiAgICAvL2RlLWFjdGl2YXRlIGN1cnJlbnQgc3RlcCBvbiBwcm9ncmVzc2JhclxyXG4gICAgJCgnLm11bHRpU3RlcEZvcm1fX3Byb2dyZXNzYmFyLWl0ZW0nKS5lcSgkKCcubXVsdGlTdGVwRm9ybV9fc3RlcCcpLmluZGV4KGN1cnJlbnRfZnMpKS5yZW1vdmVDbGFzcygnYWN0aXZlJyk7XHJcblxyXG4gICAgLy9zaG93IHRoZSBwcmV2aW91cyBmaWVsZHNldFxyXG4gICAgcHJldmlvdXNfZnMuc2hvdygpO1xyXG4gICAgLy9oaWRlIHRoZSBjdXJyZW50IGZpZWxkc2V0IHdpdGggc3R5bGVcclxuICAgIGN1cnJlbnRfZnMuYW5pbWF0ZSh7IG9wYWNpdHk6IDAgfSwge1xyXG4gICAgICAgIHN0ZXA6IGZ1bmN0aW9uIChub3csIG14KSB7XHJcbiAgICAgICAgICAgIC8vYXMgdGhlIG9wYWNpdHkgb2YgY3VycmVudF9mcyByZWR1Y2VzIHRvIDAgLSBzdG9yZWQgaW4gXCJub3dcIlxyXG4gICAgICAgICAgICAvLzEuIHNjYWxlIHByZXZpb3VzX2ZzIGZyb20gODAlIHRvIDEwMCVcclxuICAgICAgICAgICAgc2NhbGUgPSAwLjggKyAoMSAtIG5vdykgKiAwLjI7XHJcbiAgICAgICAgICAgIC8vMi4gdGFrZSBjdXJyZW50X2ZzIHRvIHRoZSByaWdodCg1MCUpIC0gZnJvbSAwJVxyXG4gICAgICAgICAgICBsZWZ0ID0gKCgoMSAtIG5vdykgKiA1MCkgKiAxMCkgKyAncHgnO1xyXG4gICAgICAgICAgICAvLzMuIGluY3JlYXNlIG9wYWNpdHkgb2YgcHJldmlvdXNfZnMgdG8gMSBhcyBpdCBtb3ZlcyBpblxyXG4gICAgICAgICAgICBvcGFjaXR5ID0gMSAtIG5vdztcclxuICAgICAgICAgICAgY3VycmVudF9mcy5jc3MoeyAndHJhbnNmb3JtJzogJ3RyYW5zbGF0ZVgoICcgKyBsZWZ0ICsgJyknIH0pO1xyXG4gICAgICAgICAgICBwcmV2aW91c19mcy5jc3MoeyAndHJhbnNmb3JtJzogJ3NjYWxlKCcgKyBzY2FsZSArICcpJywgJ29wYWNpdHknOiBvcGFjaXR5IH0pOyAgIFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZHVyYXRpb246IDgwMCxcclxuICAgICAgICBjb21wbGV0ZTogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICBjdXJyZW50X2ZzLmhpZGUoKTtcclxuICAgICAgICAgICAgYW5pbWF0aW5nID0gZmFsc2U7XHJcbiAgICAgICAgICAgICQocHJldmlvdXNfZnMpLmNzcygncG9zaXRpb24nLCAncmVsYXRpdmUnKTtcclxuICAgICAgICAgICAgJCgnLm11bHRpU3RlcEZvcm1fX3Byb2dyZXNzYmFyLWl0ZW0nKS5lcSgkKCcubXVsdGlTdGVwRm9ybV9fc3RlcCcpLmluZGV4KGN1cnJlbnRfZnMpKS5yZW1vdmVDbGFzcygnbm9EZWxheScpO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgLy90aGlzIGNvbWVzIGZyb20gdGhlIGN1c3RvbSBlYXNpbmcgcGx1Z2luXHJcbiAgICAgICAgZWFzaW5nOiAnZWFzZUluT3V0QmFjaydcclxuICAgIH0pO1xyXG59KTtcclxuIl19
