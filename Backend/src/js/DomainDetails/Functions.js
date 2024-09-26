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