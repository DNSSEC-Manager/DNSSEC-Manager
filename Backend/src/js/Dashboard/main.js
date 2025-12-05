var green = "40, 167, 69" //"#28a745"
var red = "220, 53, 69" //"#dc3545"
var yellow = "255, 193, 7" //"#ffc107"

$(document).ready(function () {
    // Only run on pages that actually contain the dashboard charts
    var signedCanvas = document.getElementById('signedChart');
    var activeCanvas = document.getElementById('activeChart');
    if (!signedCanvas || !activeCanvas || typeof Chart === 'undefined') {
        return;
    }

    var dataSignedChart = {
        labels: ['Signed', 'Unsigned'],
        datasets: [{
            label: 'Signed domains',
            data: [$('#signedChart').data('signed'), $('#signedChart').data('notsigned')],
            backgroundColor: [
                'rgba(' + green + ', 1)',
                'rgba(' + red + ', 1)'
            ],
            borderColor: [
                'rgba(' + green + ', 1)',
                'rgba(' + red + ', 1)'
            ],
            borderWidth: 1
        }]
    };
    var signedChartElem = signedCanvas.getContext('2d');

    var signedChart = new Chart(signedChartElem, {
        type: 'pie',
        data: dataSignedChart,
        options: {
            maintainAspectRatio: false
        }
    });

    var dataActiveChart = {
        labels: ['Removed from DNS', 'Found', 'Not found'],
        datasets: [{
            label: 'Active domains',
            data: [$('#activeChart').data('removedfromdns'), $('#activeChart').data('active'), $('#activeChart').data('inactive')],
            backgroundColor: [
                'rgba(' + red + ', 1)',
                'rgba(' + green + ', 1)',
                'rgba(' + yellow + ', 1)'
            ],
            borderColor: [
                'rgba(' + red + ', 1)',
                'rgba(' + green + ', 1',
                'rgba(' + yellow + ', 1)'
            ],
            borderWidth: 1
        }]
    };
    var activeChartElem = activeCanvas.getContext('2d');

    var activeChart = new Chart(activeChartElem, {
        type: 'pie',
        data: dataActiveChart,
        options: {
            maintainAspectRatio: false
        }
    });
});
